"use strict";

app.directive("vrQmBeSupplierGrid", ["UtilsService", "VRNotificationService", "QM_BE_SupplierAPIService", "QM_BE_SupplierService",
function (UtilsService, VRNotificationService, QM_BE_SupplierAPIService, QM_BE_SupplierService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
            //= (means function), @ (means attribute)
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var supplierGrid = new SupplierGrid($scope, ctrl, $attrs);
            supplierGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/QM_BusinessEntity/Directives/Supplier/Templates/SupplierGridTemplate.html"

    };

    function SupplierGrid($scope, ctrl, $attrs) {
        var gridAPI;
        var queryGrid;

        function initializeController() {
            $scope.suppliers = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        queryGrid = query;
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onSupplierAdded = function (supplierObject) {
                        gridAPI.itemAdded(supplierObject);
                    }

                    return directiveAPI;
                }

            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return QM_BE_SupplierAPIService.GetFilteredSuppliers(dataRetrievalInput)
                   .then(function (response) {
                       onResponseReady(response);
                   })
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editSupplier,
                haspermission: hasEditSupplierPermission
            },
            {
                name: "Delete",
                clicked: deleteSupplier,
                haspermission: hasDeleteSupplierPermission
            }
            ];
        }

        function hasEditSupplierPermission() {
            return QM_BE_SupplierAPIService.HasEditSupplierPermission();
        }

        function hasDeleteSupplierPermission() {
            return QM_BE_SupplierAPIService.HasDeleteSupplierPermission();
        }

        function editSupplier(supplier) {
            var onSupplierUpdated = function (updatedItem) {
                gridAPI.itemUpdated(updatedItem);
            };
            QM_BE_SupplierService.editSupplier(supplier.Entity.SupplierId, onSupplierUpdated);
        }


        function deleteSupplier(dataItem) {
            var onPermissionDeleted = function (entity) {
                var gridDataItem = { Entity: entity };
                gridDataItem.Entity.IsDeleted = true;
                gridAPI.itemDeleted(gridDataItem);
            };

            VRNotificationService.showConfirmation().then(function (confirmed) {
                if (confirmed) {
                    return QM_BE_SupplierAPIService.DeleteSupplier(dataItem.Entity).then(function () {
                        if (onPermissionDeleted && typeof onPermissionDeleted == 'function') {
                            onPermissionDeleted(dataItem.Entity);
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    });
                }
            });
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;

}]);
