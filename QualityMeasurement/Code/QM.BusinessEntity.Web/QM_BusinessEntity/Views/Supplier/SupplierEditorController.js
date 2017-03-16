(function (appControllers) {

    "use strict";

    supplierEditorController.$inject = ['$scope', 'QM_BE_SupplierAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function supplierEditorController($scope, QM_BE_SupplierAPIService, UtilsService, VRNotificationService, VRNavigationService) {

        var isEditMode;

        var supplierId;

        var supplierEntity;


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                supplierId = parameters.supplierId;
            }

            isEditMode = (supplierId != undefined);
        }

        function defineScope() {

            $scope.scopeModal = {};

            $scope.SaveSupplier = function () {
                if (isEditMode) {
                    return updateSupplier();
                }
                else {
                    return insertSupplier();
                }
            };
            $scope.hasSaveSupplierPermission = function () {
                if (isEditMode) {
                    return QM_BE_SupplierAPIService.HasEditSupplierPermission();
                }
                else {
                    return QM_BE_SupplierAPIService.HasAddSupplierPermission();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function load() {
            $scope.isLoading = true;
            if (isEditMode) {
                $scope.title = "Edit Supplier";
                getSupplier().then(function () {
                    loadAllControls();
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });

            }
            else {
                $scope.title = "New Supplier";
                loadAllControls();
            }
        }


        function loadAllControls() {
            if (supplierEntity != undefined) {
                $scope.scopeModal.name = supplierEntity.Name;
                $scope.scopeModal.prefix = supplierEntity.Settings.Prefix;
            }
                
            $scope.isLoading = false;
        }



        function getSupplier() {
            return QM_BE_SupplierAPIService.GetSupplier(supplierId).then(function (supplier) {
                supplierEntity = supplier;
            });
        }

        function buildSupplierObjFromScope() {
            var supplier = {
                SupplierId: (supplierId != null) ? supplierId : 0,
                Name: $scope.scopeModal.name
            };
            supplier.Settings = {
                Prefix: $scope.scopeModal.prefix
            };
            return supplier;
        }

        function insertSupplier() {
            var supplierObject = buildSupplierObjFromScope();
            return QM_BE_SupplierAPIService.AddSupplier(supplierObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Supplier", response, "Name")) {
                    if ($scope.onSupplierAdded != undefined)
                        $scope.onSupplierAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }


        function updateSupplier() {
            var supplierObject = buildSupplierObjFromScope();
            QM_BE_SupplierAPIService.UpdateSupplier(supplierObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Supplier", response, "Name")) {
                    if ($scope.onSupplierUpdated != undefined)
                        $scope.onSupplierUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('QM_BE_SupplierEditorController', supplierEditorController);
})(appControllers);
