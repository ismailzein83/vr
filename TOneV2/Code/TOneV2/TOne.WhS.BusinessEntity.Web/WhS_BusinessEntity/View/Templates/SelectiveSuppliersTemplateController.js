SelectiveSuppliersTemplateController.$inject = ['$scope', 'SaleZoneAPIService', 'UtilsService', 'VRNotificationService'];

function SelectiveSuppliersTemplateController($scope, SaleZoneAPIService, UtilsService, VRNotificationService) {

    defineScope();
    load();

    function defineScope() {

        $scope.saleZones = [];
        $scope.selectedSaleZones = [];

        $scope.saleZoneGroups.getData = function () {

            return {
                $type: "TOne.WhS.BusinessEntity.Entities.SelectiveSaleZonesSettings, TOne.WhS.BusinessEntity.Entities",
                ZoneIds: UtilsService.getPropValuesFromArray($scope.selectedSaleZones, "SaleZoneId")
            };
        };

        $scope.dataSourceAdapter.loadTemplateData = function () {
            loadForm();
        }

        $scope.dataSourceAdapter.resetSaleZoneSelection = function () {
            $scope.saleZones = [];
            $scope.selectedSaleZones = [];

            
        }
    }

    var isFormLoaded;
    function loadForm() {

        if ($scope.saleZoneGroups.data == undefined || isFormLoaded)
            return;

        var data = $scope.saleZoneGroups.data;
        if (data != null) {
            angular.forEach(response, function (item) {
                var saleZone = UtilsService.getItemByVal($scope.)
                $scope.selectedSaleZones.push()
            });
            $scope.selectedSaleZones = Utils. data.ConnectionString;
            $scope.query = argumentData.Query;
        }

        var adapterState = $scope.dataSourceAdapter.adapterState.data;
        if (adapterState != null) {
            $scope.lastImportedId = adapterState.LastImportedId;
        }

        isFormLoaded = true;
    }

    function load() {
        return SaleZoneAPIService.GetSaleZones($scope.saleZoneGroups.saleZonePackageId).then(function (response){
            angular.forEach(response, function (item) {
                $scope.saleZones.push(item);
                loadForm();
            });
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
        
        getSaleZones().then(function())
        {

        
    }

    function getSaleZones()
    {
        
    }


}
appControllers.controller('WhS_BusinessEntity_SelectiveSuppliersTemplateController', SelectiveSuppliersTemplateController);
