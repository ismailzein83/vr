(function (appControllers) {

    'use strict';

    FutureSMSRateController.$inject = ['$scope', 'UtilsService', 'VRNavigationService', 'UISettingsService'];

    function FutureSMSRateController($scope, UtilsService, VRNavigationService, UISettingsService) {
        var mobileNetworkName;
        var futureSMSRate;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                mobileNetworkName = parameters.mobileNetworkName;
                futureSMSRate = parameters.futureSMSRate;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.longPrecision = UISettingsService.getLongPrecision();
            
            $scope.title = "Future Rate of Mobile Network " + mobileNetworkName;

            if (futureSMSRate != undefined) {
                $scope.title = "Future  Rate of Mobile Network '" + mobileNetworkName + "'";
                $scope.scopeModel.rate = futureSMSRate.Rate;
                $scope.scopeModel.currency = futureSMSRate.CurrencySymbol;
                $scope.scopeModel.rateBED = UtilsService.getShortDate(new Date(futureSMSRate.BED));
                $scope.scopeModel.rateEED = futureSMSRate.EED != null ? UtilsService.getShortDate(new Date(futureSMSRate.EED)) : undefined;
            }

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
        }
    }

    appControllers.controller('WhS_SMSBusinessEntity_FutureSMSSupplierRateController', FutureSMSRateController);

})(appControllers);