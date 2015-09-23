SelectiveSuppliersTemplateController.$inject = ['$scope', 'WhS_BE_CarrierAccountAPIService', 'UtilsService', 'VRNotificationService'];

function SelectiveSuppliersTemplateController($scope, WhS_BE_CarrierAccountAPIService, UtilsService, VRNotificationService) {

    defineScope();
    load();

    function defineScope() {

        $scope.suppliers = [];

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

            angular.forEach($scope.saleZoneGroups.data.SupplierIds, function (item) {
                var selectedSupplier = UtilsService.getItemByVal($scope.suppliers, item, "CarrierAccountId");
                $scope.selectedSuppliers.push(selectedSupplier);
            });
        }

        isFormLoaded = true;
    }

    function load() {
        WhS_BE_CarrierAccountAPIService.GetSuppliers().then(function (response) {

            angular.forEach(response, function (item) {
                $scope.suppliers.push(item);
            });

            loadForm();
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });;
        
    }
}
appControllers.controller('WhS_BE_SelectiveSuppliersTemplateController', SelectiveSuppliersTemplateController);
