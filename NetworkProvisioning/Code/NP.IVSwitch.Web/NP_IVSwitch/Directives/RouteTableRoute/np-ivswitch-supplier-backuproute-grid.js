"use strict";
app.directive("npIvswitchSupplierBackuprouteGrid", ["UtilsService", "VRNotificationService", "NP_IVSwitch_RouteTableAPIService", "NP_IVSwitch_RouteTableService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, NP_IVSwitch_RouteTableAPIService, NP_IVSwitch_RouteTableService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var routeTableGrid = new RouteTableGrid($scope, ctrl, $attrs);
            routeTableGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/NP_IVSwitch/Directives/RouteTableRoute/Templates/SupplierBackupRouteGridTemplate.html"
    };

    function RouteTableGrid($scope, ctrl) {
        var supplierAPI;



        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.dataItem = {};
            $scope.scopeModel.columns = [];

            var gridApiDefferedReady = UtilsService.createPromiseDeferred();
            var backupGridApiDefferedReady = UtilsService.createPromiseDeferred();
            var percentageDefferedReady = UtilsService.createPromiseDeferred();

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                gridApiDefferedReady.resolve();
            };

            $scope.scopeModel.addColumn = function () {
                addBackupColumn();
            };

            $scope.scopeModel.removeBackupColumn = function (dataItem) {
                var gridItems = $scope.scopeModel.columns;
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.columns, dataItem.Entity.id, 'Entity.id');
                if (index > -1)
                    $scope.scopeModel.columns.splice(index, 1);
            };

            function addBackupColumn() {
                var dataItem = {
                    Entity: {}
                };

                var backupSupplierDirectiveDefferedReady = UtilsService.createPromiseDeferred();
                var backupRouteDirectiveDefferedReady = UtilsService.createPromiseDeferred();
                var selectedBackupSupplierDefferedReady = UtilsService.createPromiseDeferred();

                dataItem.Entity.onBackupOptionSuppliersDirectiveReady = function (api) {
                    dataItem.Entity.backupOptionSupplierDirctiveAPI = api;
                    backupSupplierDirectiveDefferedReady.resolve();
                };

                dataItem.Entity.onBackupOptionRouteDirectiveReady = function (api) {
                    dataItem.Entity.backupOptionRouteDirectiveAPI = api;
                    backupRouteDirectiveDefferedReady.resolve();
                };
                dataItem.Entity.onBackupOptionSupplierSelectionChanged = function (option) {
                    var backupSupplierIds = [];
                    if (option != undefined && option.length > 0) {
                        if (selectedBackupSupplierDefferedReady != undefined)
                            selectedBackupSupplierDefferedReady.resolve();
                        else {
                            for (var i = 0; i < option.length; i++)
                                backupSupplierIds.push(option[i].CarrierAccountId);
                            var directivePayload = {
                                filter: {
                                    AssignableToCarrierAccountId: null,
                                }
                            };
                            if (backupSupplierIds.length > 0)
                                directivePayload.filter.SupplierIds = backupSupplierIds;

                            var modifiedByFieldSetLoader = function (value) { $scope.scopeModel.isModifiedBySelectorLoading = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.Entity.backupOptionRouteDirectiveAPI, directivePayload, modifiedByFieldSetLoader);
                        }
                    }
                    else {
                        dataItem.Entity.backupOptionRouteDirectiveAPI.clearDataSource();
                    }
                };

                backupSupplierDirectiveDefferedReady.promise.then(function () {
                    var directivePayload;
                    var modifiedByFieldSetLoader = function (value) {  dataItem.isRouteSelectorLoading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.Entity.backupOptionSupplierDirctiveAPI, directivePayload, modifiedByFieldSetLoader,undefined);
                    selectedBackupSupplierDefferedReady = undefined;
                });

                $scope.scopeModel.columns.push(dataItem);
            }

            defineAPI();

        }


        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    if (payload.backupOptions != null) {
                        for (var i = 0; i < payload.backupOptions.length ; i++) {
                            var backupOption = payload.backupOptions[i];
                            var gridItem = {
                                payload: backupOption,
                                supplierBackUpReadyDeffered: UtilsService.createPromiseDeferred(),
                                supplierBackUpLoadDeffered: UtilsService.createPromiseDeferred(),
                                routeBackUpReadyDeffered: UtilsService.createPromiseDeferred(),
                                routeBackUpLoadDeffered: UtilsService.createPromiseDeferred(),
                            };
                            promises.push(gridItem.supplierBackUpLoadDeffered.promise);
                            promises.push(gridItem.routeBackUpLoadDeffered.promise);
                            addColumnOnEdit(gridItem);
                        }
                    }
                }
                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var backupOptions = [];
                if ($scope.scopeModel.columns.length > 0)
                {
                    for(var i= 0;i< $scope.scopeModel.columns.length;i++)
                    {
                        backupOptions.push($scope.scopeModel.columns[i].Entity);
                    }
                }
                return backupOptions;
            };


            function addColumnOnEdit(gridItem) {
                var dataItem = {
                    id: $scope.scopeModel.columns.length + 1,
                };
                var backupOptionselectdSupplierDeffered;

                if (gridItem.payload.BackupOptionSupplierId != undefined)
                    backupOptionselectdSupplierDeffered = UtilsService.createPromiseDeferred();

                dataItem.onBackupOptionSuppliersDirectiveReady = function (api) {
                    dataItem.backupOptionSupplierDirctiveAPI = api;
                    gridItem.supplierBackUpReadyDeffered.resolve();

                };

                dataItem.onBackupOptionRouteDirectiveReady = function (api) {
                    dataItem.backupOptionRouteDirectiveAPI = api;
                    gridItem.routeBackUpReadyDeffered.resolve();

                };

                gridItem.supplierBackUpReadyDeffered.promise.then(function () {
                    var directivePayload;
                    if (gridItem.payload.BackupOptionSupplierId != undefined)
                        directivePayload = { selectedIds: [gridItem.payload.BackupOptionSupplierId] };
                    VRUIUtilsService.callDirectiveLoad(dataItem.backupOptionSupplierDirctiveAPI, directivePayload, gridItem.supplierBackUpLoadDeffered);
                });

                dataItem.onBackupOptionSupplierSelectionChanged = function (option) {
                    if (option != undefined && option.length > 0) {
                        if (backupOptionselectdSupplierDeffered != undefined) {
                            backupOptionselectdSupplierDeffered.resolve();
                        }
                        else {
                            var directivePayload = {
                                AssignableToCarrierAccountId: null,
                                SupplierIds: dataItem.backupOptionSupplierDirctiveAPI.getSelectedIds()
                            };
                            var modifiedByFieldSetLoader = function (value) { $scope.scopeModel.isModifiedBySelectorLoading = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.backupOptionRouteDirectiveAPI, directivePayload, modifiedByFieldSetLoader);
                        }
                    }
                    else {
                        dataItem.backupOptionRouteDirectiveAPI.clearDataSource();
                    }

                };

                var promises = [];
                promises.push(gridItem.routeBackUpReadyDeffered.promise);
                if (gridItem.payload.BackupOptionSupplierId != undefined)
                    promises.push(backupOptionselectdSupplierDeffered.promise);
                UtilsService.waitMultiplePromises(promises).then(function () {
                    var directivePayload = {
                        filter: {
                            AssignableToCarrierAccountId: null,
                        }
                    };
                    if (gridItem.payload.BackupOptionRouteId != undefined) {
                        directivePayload.selectedIds = gridItem.payload.BackupOptionRouteId;
                        directivePayload.filter.SupplierIds = (gridItem.payload.BackupOptionSupplierId != null) ? [gridItem.payload.BackupOptionSupplierId] : undefined;
                    }
                    VRUIUtilsService.callDirectiveLoad(dataItem.backupOptionRouteDirectiveAPI, directivePayload, gridItem.routeBackUpLoadDeffered);
                    backupOptionselectdSupplierDeffered = undefined;
                });


                $scope.scopeModel.columns.push({ Entity: dataItem });

            }

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                ctrl.onReady(api);
            }
        }

    }
    return directiveDefinitionObject;
}]);
