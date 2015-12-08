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
        var supplierSettingAPI;
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                supplierId = parameters.supplierId;
            }

            isEditMode = (supplierId != undefined);
        }

        function defineScope() {

            $scope.scopeModal = {};

            setDirectiveTabs();
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
                $scope.scopeModal.name = supplierEntity.Name;
            });
        }



        function buildSupplierObjFromScope() {
            var whsSupplier = {
                SupplierId: (supplierId != null) ? supplierId : 0,
                Name: $scope.scopeModal.name
            };
            var extendedSetting = [];
            for (var i = 0 ; i < $scope.directiveTabs.length ; i++) {
                if ($scope.directiveTabs[i].directiveAPI != undefined)
                    extendedSetting[extendedSetting.length] = $scope.directiveTabs[i].directiveAPI.getData();
            }
            whsSupplier.Settings = {
                ExtendedSettings: extendedSetting
            }
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
                if (VRNotificationService.notifyOnItemUpdated("Supplier", response, "Name")) {
                    if ($scope.onSupplierUpdated != undefined)
                        $scope.onSupplierUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        function setDirectiveTabs() {
            $scope.directiveTabs = [];
            var cliTab = {
                title: "CLI Tester",
                directive: "vr-qm-clitester-suppliersettings",
                loadDirective: function (api) {
                    supplierSettingAPI = api;
                    var payload = {
                        prefix: (supplierEntity!= undefined &&  supplierEntity.Settings != undefined) ? supplierEntity.Settings.Prefix : undefined
                    };
                    api.load(payload);
                },
                dontLoad:false
            };
            $scope.directiveTabs.push(cliTab);
        }
    }

    appControllers.controller('QM_BE_SupplierEditorController', supplierEditorController);
})(appControllers);
