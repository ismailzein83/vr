(function (appControllers) {

    "use strict";

    selectiveSuppliersTemplateController.$inject = ['$scope', 'WhS_BE_CarrierAccountAPIService', 'UtilsService', 'VRNotificationService'];
    function selectiveSuppliersTemplateController($scope, WhS_BE_CarrierAccountAPIService, UtilsService, VRNotificationService) {
        defineScope();
        load();

        function defineScope() {

            $scope.suppliers = [];
            $scope.selectedSuppliers = [];
            $scope.supplierGroups.getData = function () {

                return {
                    $type: "TOne.WhS.BusinessEntity.Entities.SelectiveSuppliersSettings, TOne.WhS.BusinessEntity.Entities",
                    SupplierIds: UtilsService.getPropValuesFromArray($scope.selectedSuppliers, "CarrierAccountId")
                };
            };

            $scope.supplierGroups.loadTemplateData = function () {
                loadForm();
            }
        }

        var isFormLoaded;
        function loadForm() {

            if ($scope.supplierGroups.data == undefined || isFormLoaded)
                return;

            var data = $scope.supplierGroups.data;
            if (data != null) {
                if ($scope.suppliers.length != 0) {
                    angular.forEach($scope.supplierGroups.data.SupplierIds, function (item) {
                        var selectedSupplier = UtilsService.getItemByVal($scope.suppliers, item, "CarrierAccountId");
                        $scope.selectedSuppliers.push(selectedSupplier);
                    });
                }
            }

            isFormLoaded = true;
        }

        function load() {
            WhS_BE_CarrierAccountAPIService.GetCarrierAccounts(false, true).then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.suppliers.push(item);
                });

                loadForm();
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });;

        }
    }
    
    appControllers.controller('WhS_BE_SelectiveSuppliersTemplateController', selectiveSuppliersTemplateController);
})(appControllers);
