(function (appControllers) {

    "use strict";

    supplierEditorController.$inject = ['$scope', 'QM_BE_SupplierAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function supplierEditorController($scope, QM_BE_SupplierAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

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

            $scope.SaveSupplier = function () {
                if (isEditMode) {
                    return updateSupplier();
                }
                else {
                    return insertSupplier();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                $scope.title = "Edit Supplier";
                getSupplier().then(function () {
                    $scope.isLoading = false;
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                $scope.title = "New Supplier";
                $scope.isLoading = false;
            }
        }


        function getSupplier() {
            return QM_BE_SupplierAPIService.GetSupplier(supplierId).then(function (whsSupplier) {
                supplierEntity = whsSupplier;
                $scope.name = supplierEntity.Name;
            });
        }



        function buildSupplierObjFromScope() {
            var whsSupplier = {
                SupplierId: (supplierId != null) ? supplierId : 0,
                Name: $scope.name
            };
            return whsSupplier;
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
                if (VRNotificationService.notifyOnItemUpdated("Supplier", response,"Name")) {
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
