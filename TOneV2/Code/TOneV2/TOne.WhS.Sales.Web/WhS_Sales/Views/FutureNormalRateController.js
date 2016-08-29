(function (appControllers) {

    'use strict';

    FutureNormalRateController.$inject = ['$scope', 'UtilsService', 'VRNavigationService'];

    function FutureNormalRateController($scope, UtilsService, VRNavigationService)
    {
        var zoneName;
        var futureNormalRate;

        loadParameters();
        defineScope();
        load();

        function loadParameters()
        {
            var parameters = VRNavigationService.getParameters($scope);
            
            if (parameters != undefined)
            {
                zoneName = parameters.zoneName;
                futureNormalRate = parameters.futureNormalRate;
            }
        }

        function defineScope()
        {
            $scope.scopeModel = {};

            $scope.title = 'Future Normal Rate of Zone ' + zoneName;

            if (futureNormalRate != undefined)
            {
                $scope.scopeModel.rate = futureNormalRate.Rate;
                $scope.scopeModel.rateBED = UtilsService.getShortDate(new Date(futureNormalRate.BED));
            }

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            
        }
    }

    appControllers.controller('WhS_Sales_FutureNormalRateController', FutureNormalRateController);

})(appControllers);