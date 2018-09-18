"use strict";
app.directive("npIvswitchSupplierRouteGrid", ["UtilsService", "VRNotificationService", "NP_IVSwitch_RouteTableAPIService", "NP_IVSwitch_RouteTableService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
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
        templateUrl: "/Client/Modules/NP_IVSwitch/Directives/RouteTableRoute/Templates/SupplierRouteGridTemplate.html"
    };

    function RouteTableGrid($scope, ctrl) {
        var gridApi;
        var gridApiDefferedReady = UtilsService.createPromiseDeferred();
        $scope.scopeModel = {};
        $scope.dataItem = {};
        $scope.scopeModel.columns = [];

        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel.showBackupTabs = false;

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                gridApiDefferedReady.resolve();
            };

            $scope.scopeModel.addColumn = function () {
                addColumn();
            };

            $scope.scopeModel.removeColumn = function (dataItem) {
                var gridItems = $scope.scopeModel.columns;
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.columns, dataItem.id, 'id');
                if (index > -1) 
                    $scope.scopeModel.columns.splice(index, 1);

                if ($scope.scopeModel.columns.length == 0)
                    $scope.scopeModel.showBackupTabs = false;
                var total = undefined;
                for (var x = 0; x < gridItems.length; x++) {
                    var gridItem = gridItems[x];
                    if (gridItem.percentage != undefined && gridItem.percentage != "") {
                        if (total == undefined)
                            total = 0;
                        total += parseFloat(gridItem.percentage);
                    }
                }
                if (total == undefined) {
                    $scope.scopeModel.showBackupTabs = false;
                    collapseRows();
                }

            };

            $scope.scopeModel.isGridValid = function () {
                if (!$scope.scopeModel.isBlockedAccount && $scope.scopeModel.columns.length == 0)
                    return "at least one choose option";
                return null;
            };

            $scope.scopeModel.showExpand = function (dataItem) {

                return $scope.scopeModel.showBackupTabs;
            };

            $scope.scopeModel.onPercentageValueChangedItems = function (dataItem) {
                var gridItems = $scope.scopeModel.columns;
                if (gridItems == undefined)
                    return;

                var total = undefined;
                for (var x = 0; x < gridItems.length; x++) {
                    var gridItem = gridItems[x];
                    if (gridItem.percentage != undefined && gridItem.percentage != "") {
                        if (total == undefined)
                            total = 0;

                        total += parseFloat(gridItem.percentage);
                    }
                }
                if (total == undefined)
                    $scope.scopeModel.onPercentageValueChanged();
            };

            $scope.scopeModel.onPercentageValueChanged = function () {
                var gridItems = $scope.scopeModel.columns;
                if (gridItems == undefined)
                    return;

                var total = undefined;
                for (var x = 0; x < gridItems.length; x++) {
                    var gridItem = gridItems[x];
                    if (gridItem.percentage != undefined && gridItem.percentage != "") {
                        if (total == undefined)
                            total = 0;

                        total += parseFloat(gridItem.percentage);
                    }
                }
                if (total != undefined) {
                    if (!$scope.scopeModel.showBackupTabs)
                        expandRows();
                    $scope.scopeModel.showBackupTabs = true;

                }
                else {
                    if ($scope.scopeModel.showBackupTabs)
                        collapseRows();
                    $scope.scopeModel.showBackupTabs = false;


                }

                if (total != undefined && total != 100) {
                    return "sum of percentages must be equals to 100 ";
                }
                return null;
            };

            function expandRow(option) {
                gridApi.expandRow(option);
            }

            function expandRows() {
                for (var i = 0; i < $scope.scopeModel.columns.length; i++) {
                    gridApi.expandRow($scope.scopeModel.columns[i]);
                }
            }

            function collapseRows() {
                for (var i = 0; i < $scope.scopeModel.columns.length; i++) {
                    gridApi.collapseRow($scope.scopeModel.columns[i]);
                }
            }

            function addColumn() {
                var promises = [];

                var supplierDirectiveDefferedReady = UtilsService.createPromiseDeferred();
                var routeDirectiveReadyDeffered = UtilsService.createPromiseDeferred();
                var selectdSupplierDeffered = UtilsService.createPromiseDeferred();
                var gridItem = {
                    id: $scope.scopeModel.columns.length + 1
                };

                gridItem.onSuppliersDirectiveReady = function (api) {
                    gridItem.supplierDirctiveAPI = api;
                    supplierDirectiveDefferedReady.resolve();
                };

                gridItem.onRouteDirectiveReady = function (api) {
                    gridItem.routeDirectiveAPI = api;
                    routeDirectiveReadyDeffered.resolve();
                };

                gridItem.percentage = $scope.dataItem.percentage;

                gridItem.onSupplierSelectionChanged = function (option) {
                    var supplierIds = [];
                    if (option != undefined && option.length > 0) {
                        if (selectdSupplierDeffered != undefined) {
                            selectdSupplierDeffered.resolve();
                        }
                        else {
                            gridItem.isRouteSelectorLoading = true;
                            for (var i = 0; i < option.length; i++)
                                supplierIds.push(option[i].CarrierAccountId);
                            var directivePayload = {

                                filter: {
                                    AssignableToCarrierAccountId: null,
                                    SupplierIds: supplierIds
                                }
                            };
                            var setLoader = function (value) { gridItem.isRouteSelectorLoading = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.routeDirectiveAPI, directivePayload, setLoader,undefined);

                            //var loadDeffered = UtilsService.createPromiseDeferred();
                            //VRUIUtilsService.callDirectiveLoad(gridItem.routeDirectiveAPI, directivePayload, loadDeffered)
                            //loadDeffered.promise.then(function () { gridItem.isRouteSelectorLoading = false; });

                        }
                    }
                    else {
                        gridItem.routeDirectiveAPI.clearDataSource();
                    }
                };

                gridItem.isPercentageRequired = function () {

                    for (var i = 0; i < $scope.scopeModel.columns.length; i++) {
                        if ($scope.scopeModel.columns[i].percentage != undefined)
                            return true;

                    }
                    return false;

                };

                gridItem.onBackupRouteGridDirectiveReady = function (api) {
                    gridItem.backupRouteGridDirectiveAPI = api;
                };

                supplierDirectiveDefferedReady.promise.then(function () {
                        var directivePayload;
                        VRUIUtilsService.callDirectiveLoad(gridItem.supplierDirctiveAPI, directivePayload, undefined);
                        selectdSupplierDeffered = undefined;
                    });

                $scope.scopeModel.columns.push(gridItem);
                if ($scope.scopeModel.showBackupTabs == true) {
                    expandRow(gridItem);
                }
            }

            defineAPI();
           
        }

        function defineAPI() {
            var atLeastOneBackup = false;
            var api = {};
            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    $scope.scopeModel.isBlockedAccount = payload.IsBlockedAccount;
                    if (payload.RouteOptionsToEdit != null)
                    {
                        var hasPercentage = false;
                        for (var i = 0; i < payload.RouteOptionsToEdit.length ; i++) {
                            var routeOptionToEdit = payload.RouteOptionsToEdit[i];
                            if (routeOptionToEdit.Percentage != 0)
                            {
                                hasPercentage = true;
                                break;
                            }
                        }

                        for (var i = 0; i < payload.RouteOptionsToEdit.length ; i++) {
                            var routeOptionToEdit = payload.RouteOptionsToEdit[i];
                            var gridItem = {
                                payload: routeOptionToEdit,
                                supplierReadyDeffered: UtilsService.createPromiseDeferred(),
                                supplierLoadDeffered: UtilsService.createPromiseDeferred(),
                                routeReadyDeffered: UtilsService.createPromiseDeferred(),
                                routeLoadDeffered: UtilsService.createPromiseDeferred(),
                                hasPercentage: hasPercentage,
                                backupRouteGridDirectiveReady: hasPercentage ? UtilsService.createPromiseDeferred() : undefined,
                                backupRouteGridDirectiveLoad: hasPercentage ? UtilsService.createPromiseDeferred() : undefined,
                            };
                            promises.push(gridItem.supplierLoadDeffered.promise);
                            promises.push(gridItem.routeLoadDeffered.promise);
                            if (hasPercentage)
                            {
                                promises.push(gridItem.backupRouteGridDirectiveLoad.promise);
                            }
                            addColumnOnEdit(gridItem);
                        }

                    }
                        

                }
                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var routeOptions = [];
                if ($scope.scopeModel.columns != null && $scope.scopeModel.columns.length > 0)
                    for (var i = 0; i < $scope.scopeModel.columns.length; i++) {
                        var routeOption = {
                            RouteId: $scope.scopeModel.columns[i].routeDirectiveAPI.getSelectedIds(),
                            Percentage: $scope.scopeModel.columns[i].percentage
                        };
                        if ($scope.scopeModel.columns[i].backupRouteGridDirectiveAPI != undefined) {
                            var backupRouteOptions = $scope.scopeModel.columns[i].backupRouteGridDirectiveAPI.getData();
                            var backupRouteIds = [];
                            if (backupRouteOptions != undefined && backupRouteOptions.length > 0)
                                for (var x = 0; x < backupRouteOptions.length; x++) {
                                    backupRouteIds.push({ BackupOptionRouteId: backupRouteOptions[x].backupOptionRouteDirectiveAPI.getSelectedIds() });

                                }
                            if (backupRouteIds.length > 0)
                                routeOption.BackupRouteIds = backupRouteIds;
                        }
                        routeOptions.push(routeOption);

                    }

                return { RouteOptions: routeOptions, IsBlockedAccount: $scope.scopeModel.isBlockedAccount };
            };


            function addColumnOnEdit(gridItem) {
                    var selectedSupplierDeffered = UtilsService.createPromiseDeferred();
                    var dataItem = {
                        id: $scope.scopeModel.columns.length + 1,
                        percentage: (gridItem.payload.Percentage == 0) ? null : gridItem.payload.Percentage
                    };

                    dataItem.onSuppliersDirectiveReady = function (api) {
                        dataItem.supplierDirctiveAPI = api;
                        gridItem.supplierReadyDeffered.resolve();
                    };

                    dataItem.onRouteDirectiveReady = function (api) {
                        dataItem.routeDirectiveAPI = api;
                        gridItem.routeReadyDeffered.resolve();
                    };

                    dataItem.onSupplierSelectionChanged = function (option) {
                        var supplierIds = [];
                        if (option != undefined && option.length > 0) {
                            if (selectedSupplierDeffered != undefined) {
                                selectedSupplierDeffered.resolve();
                            }
                            else {
                                for (var i = 0; i < option.length; i++)
                                    supplierIds.push(option[i].CarrierAccountId); var directivePayload = {
                                        filter: {
                                            AssignableToCarrierAccountId: null,
                                            SupplierIds: supplierIds
                                        }
                                    };
                                var modifiedByFieldSetLoader = function (value) { $scope.scopeModel.isModifiedBySelectorLoading = value; };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.routeDirectiveAPI, directivePayload, modifiedByFieldSetLoader);
                            }
                        }
                        else {
                            dataItem.routeDirectiveAPI.clearDataSource();
                        }
                    };
                
                    gridItem.supplierReadyDeffered.promise.then(function () {
                        var directivePayload;
                        if (gridItem.payload != undefined)
                            directivePayload = { selectedIds: gridItem.payload.SupplierId != 0 ? [gridItem.payload.SupplierId] : undefined };
                        VRUIUtilsService.callDirectiveLoad(dataItem.supplierDirctiveAPI, directivePayload, gridItem.supplierLoadDeffered);
                    });

                    UtilsService.waitMultiplePromises([gridItem.routeReadyDeffered.promise, selectedSupplierDeffered.promise]).then(function () {
                        var directivePayload = {
                            filter: {
                                AssignableToCarrierAccountId: null
                            }
                        };


                        if (gridItem.payload != undefined) {
                            directivePayload.selectedIds = gridItem.payload.RouteId != 0 ? gridItem.payload.RouteId : undefined;
                            directivePayload.filter.SupplierIds = gridItem.payload.SupplierId != null ? [gridItem.payload.SupplierId] : undefined;
                        }

                        VRUIUtilsService.callDirectiveLoad(dataItem.routeDirectiveAPI, directivePayload, gridItem.routeLoadDeffered);
                        selectedSupplierDeffered = undefined;
                    });

                    if (gridItem.hasPercentage)
                    {
                        dataItem.onBackupRouteGridDirectiveReady = function (api) {
                            dataItem.backupRouteGridDirectiveAPI = api;
                            gridItem.backupRouteGridDirectiveReady.resolve();
                        };

                        gridItem.backupRouteGridDirectiveReady.promise.then(function () {
                            var directivePayload;
                            if (gridItem.payload != undefined)
                                directivePayload = { backupOptions: gridItem.payload.BackupOptions };
                            VRUIUtilsService.callDirectiveLoad(dataItem.backupRouteGridDirectiveAPI, directivePayload, gridItem.backupRouteGridDirectiveLoad);
                        });
                    }

                $scope.scopeModel.columns.push(dataItem);

            }

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                ctrl.onReady(api);
            }
        }

    }
    return directiveDefinitionObject;
}]);
