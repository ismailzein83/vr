(function (appControllers) {

    'use strict';

    FutureRateController.$inject = ['$scope', 'UtilsService', 'VRNavigationService'];

    function FutureRateController($scope, UtilsService, VRNavigationService)
    {
        var zoneName;
        var futureRate;

        loadParameters();
        defineScope();
        load();

        function loadParameters()
        {
            var parameters = VRNavigationService.getParameters($scope);
            
            if (parameters != undefined)
            {
                zoneName = parameters.zoneName;
                futureRate = parameters.futureRate;
            }
        }

        function defineScope()
        {
            $scope.scopeModel = {};

            $scope.title = 'Future Rate of Zone ' + zoneName;

            if (futureRate != undefined)
            {
                var rateType = (futureRate.RateTypeId != null) ? 'Other' : 'Normal';
                $scope.title = 'Future ' + rateType + ' Rate of Zone ' + zoneName;
                $scope.scopeModel.rate = futureRate.Rate;
                $scope.scopeModel.rateBED = UtilsService.getShortDate(new Date(futureRate.BED));
            }

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            
        }
    }

    appControllers.controller('WhS_Sales_FutureRateController', FutureRateController);

})(appControllers);