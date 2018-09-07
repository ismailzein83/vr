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
        var allAPIs = [];
        var gridApi;
        var supplierAPI;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.dataItem = {};

            $scope.scopeModel.validatePercentage = function () {
                var somePercentage = parseInt(0);
                for (var i = 0; i < ctrl.columns.length; i++)
                    if (ctrl.columns[i].percentage != undefined)
                        somePercentage += parseInt(ctrl.columns[i].percentage)
                if (somePercentage != 0 && somePercentage != 100)
                    return "Some of the percentages most be equals to 100";
                if ($scope.scopeModel.isBlockedAccount != undefined && $scope.scopeModel.isBlockedAccount != true && ctrl.columns.length == 0) {
                    return "At least one option";
                }
                if (somePercentage == 0 || somePercentage == 100)
                    return null;
            };

            ctrl.columns = [];

            ctrl.addColumn = function () {
                addColumn();
            };

            ctrl.removeColumn = function (dataItem) {
                var index = UtilsService.getItemIndexByVal(ctrl.columns, dataItem.id, 'id');
                if (index > -1) {
                    ctrl.columns.splice(index, 1);
                }
            };

            ctrl.onGridReady = function (api) {
                gridApi = api;
            };

            function addColumn(data) {
                var promises = [];
                var supplierDirectiveDefferedReady = UtilsService.createPromiseDeferred();

                var routeDirectiveReadyDeffered = UtilsService.createPromiseDeferred();

                var selectdSupplierDeffered = UtilsService.createPromiseDeferred();
                var gridItem = {
                    id: ctrl.columns.length + 1
                };

                if (data != undefined) {
                    gridItem.columnName = data.ColumnName,
                    gridItem.sqlDataType = data.SQLDataType,
                    gridItem.selectedDataRecordTypeFieldName = data.ValueExpression,
                    gridItem.isUnique = data.IsUnique,
                    gridItem.isIdentity = data.IsIdentity,
                    gridItem.isDisabled = true
                }

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
                    var tab = [];
                    if (option != undefined) {
                        if (selectdSupplierDeffered != undefined) {
                            selectdSupplierDeffered.resolve();
                        }
                        else {
                            tab.push(option.CarrierAccountId);
                            var directivePayload = {

                                filter: {
                                    AssignableToCarrierAccountId: null,
                                    SupplierIds: tab
                                }
                            };
                            var modifiedByFieldSetLoader = function (value) { $scope.scopeModel.isModifiedBySelectorLoading = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.routeDirectiveAPI, directivePayload, modifiedByFieldSetLoader);
                        }
                    }
                    else {
                        gridItem.routeDirectiveAPI.clearDataSource();
                    }
                };

                gridItem.isPercentageRequired = function () {

                    for (var i = 0; i < ctrl.columns.length; i++) {
                        if (ctrl.columns[i].percentage != undefined)
                            return true;

                    }
                    return false;

                }

                promises.push(loadDirectiveSupplier());

                function loadDirectiveSupplier() {
                    return supplierDirectiveDefferedReady.promise.then(function () {
                        var directivePayload;
                        VRUIUtilsService.callDirectiveLoad(gridItem.supplierDirctiveAPI, directivePayload, undefined);
                    });
                }

                UtilsService.waitMultiplePromises(promises).then(function () {
                    selectdSupplierDeffered = undefined;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                 .finally(function () {

                 });
                ctrl.columns.push(gridItem);

            };

            defineAPI();
           
        };

        function defineAPI() {

            var api = {};
            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.scopeModel.isBlockedAccount = payload.IsBlockedAccount;
                    for (var i = payload.RouteOptionsToEdit.length - 1; i >= 0; i--)
                        addColumnOnEdit(payload.RouteOptionsToEdit[i]);
                }

            };

            api.getData = function () {
                return { GridData: ctrl.columns, IsBlockedAccount: $scope.scopeModel.isBlockedAccount };
            };
            function addColumnOnEdit(data) {
                var promises = [];
                var supplierDirectiveDefferedReady = UtilsService.createPromiseDeferred();


                var routeDirectiveReadyDeffered = UtilsService.createPromiseDeferred();

                var selectdSupplierDeffered = UtilsService.createPromiseDeferred();
                var gridItem = {
                    id: ctrl.columns.length + 1
                };


                gridItem.onSuppliersDirectiveReady = function (api) {
                    gridItem.supplierDirctiveAPI = api;

                    supplierDirectiveDefferedReady.resolve();
                };
                gridItem.onRouteDirectiveReady = function (api) {
                    gridItem.routeDirectiveAPI = api;

                    routeDirectiveReadyDeffered.resolve();
                };

                gridItem.percentage = (data.Percentage == 0) ? null : data.Percentage;

                gridItem.onSupplierSelectionChanged = function (option) {
                    var tab = [];
                    if (option != undefined) {
                        if (selectdSupplierDeffered != undefined) {

                            selectdSupplierDeffered.resolve();
                        }
                        else {
                            tab.push(option.CarrierAccountId);
                            var directivePayload = {

                                filter: {
                                    AssignableToCarrierAccountId: null,
                                    SupplierIds: tab


                                }, selectedIds: data.RouteId != 0 ? data.RouteId : undefined
                            }
                            var modifiedByFieldSetLoader = function (value) { $scope.scopeModel.isModifiedBySelectorLoading = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.routeDirectiveAPI, directivePayload, modifiedByFieldSetLoader);



                        }
                    }
                    else {
                        gridItem.routeDirectiveAPI.clearDataSource();

                    }


                };

                promises.push(loadDirectiveSupplier());
                function loadDirectiveSupplier() {
                    return supplierDirectiveDefferedReady.promise.then(function () {
                        var directivePayload;
                        if (data != undefined)
                            directivePayload = { selectedIds: data.SupplierId != 0 ? data.SupplierId : undefined }
                        VRUIUtilsService.callDirectiveLoad(gridItem.supplierDirctiveAPI, directivePayload, undefined);

                    })

                };
                UtilsService.waitMultiplePromises(promises).then(function () {
                    selectdSupplierDeffered = undefined;

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                 .finally(function () {

                 });
                ctrl.columns.push(gridItem);

            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                ctrl.onReady(api);
            }
        };

    };
    return directiveDefinitionObject;
}]);
