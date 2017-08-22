(function (appControllers) {

    'use strict';

    FutureRateController.$inject = ['$scope', 'UtilsService', 'VRNavigationService', 'WhS_BE_PrimarySaleEntityEnum'];

    function FutureRateController($scope, UtilsService, VRNavigationService, WhS_BE_PrimarySaleEntityEnum)
    {
        var zoneName;
        var futureRate;
        var primarySaleEntity;

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
                primarySaleEntity = parameters.primarySaleEntity;
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

                if (primarySaleEntity == WhS_BE_PrimarySaleEntityEnum.SellingProduct.value) {
                    if (futureRate.IsRateEditable) {
                        $scope.scopeModel.futureRateIconType = 'explicit';
                        $scope.scopeModel.futureRateIconTooltip = 'Explicit';
                    }
                    
                }
                else if (!futureRate.IsRateEditable) {
                    $scope.scopeModel.futureRateIconType = 'inherited';
                    $scope.scopeModel.futureRateIconTooltip = 'Inherited';
                }
                
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