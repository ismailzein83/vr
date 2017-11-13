(function (appControllers) {

    'use strict';

    FutureRateController.$inject = ['$scope', 'UtilsService', 'VRNavigationService', 'WhS_BE_PrimarySaleEntityEnum', 'WhS_BE_RateChangeTypeEnum'];

    function FutureRateController($scope, UtilsService, VRNavigationService, WhS_BE_PrimarySaleEntityEnum, WhS_BE_RateChangeTypeEnum) {
        var zoneName;
        var futureRate;
        var primarySaleEntity;
        var ownerType;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                zoneName = parameters.zoneName;
                futureRate = parameters.futureRate;
                primarySaleEntity = parameters.primarySaleEntity;
                ownerType = parameters.ownerType;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.title = 'Future Rate of Zone ' + zoneName;

            if (futureRate != undefined) {
                var rateType = (futureRate.RateTypeId != null) ? 'Other' : 'Normal';
                $scope.title = 'Future ' + rateType + ' Rate of Zone ' + zoneName;
                $scope.scopeModel.rate = futureRate.Rate;
                $scope.scopeModel.rateBED = UtilsService.getShortDate(new Date(futureRate.BED));

                if (ownerType == WhS_BE_PrimarySaleEntityEnum.Customer.value) {

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
                }
                $scope.scopeModel.rateChangeType = UtilsService.getEnum(WhS_BE_RateChangeTypeEnum, 'value', futureRate.RateChange);
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