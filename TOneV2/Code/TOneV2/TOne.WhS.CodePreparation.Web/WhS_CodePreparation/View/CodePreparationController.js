CodePreparationController.$inject = ['$scope', 'UtilsService', 'VRNavigationService', '$q', 'VRModalService', 'VRNotificationService', 'WhS_BE_SaleZonePackageAPIService'];

function CodePreparationController($scope, UtilsService, VRNavigationService, $q, VRModalService, VRNotificationService, WhS_BE_SaleZonePackageAPIService) {
    defineScope();
    loadParameters();
    load();
    function loadParameters() {
    }
    function defineScope() {
        $scope.showResult = false;
        $scope.saleZonePackages = [];
        $scope.selectedSaleZonePackages = [];
        $scope.onMainGridReady = function (api) {
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
        };

        $scope.getData = function () {
            return retrieveData();
        };
    }

    function retrieveData() {
    }
    function load() {
        loadSaleZonePackages();
    }


    function loadSaleZonePackages() {
        return WhS_BE_SaleZonePackageAPIService.GetSaleZonePackages().then(function (response) {
            for (var i = 0; i < response.length; i++)
                $scope.saleZonePackages.push(response[i]);
        });
    }


};



appControllers.controller('WhS_CodePreparation_CodePreparationController', CodePreparationController);