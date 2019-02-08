(function (appControllers) {

    'use strict';

    FutureSMSRateController.$inject = ['$scope', 'UtilsService', 'VRNavigationService'];

    function FutureSMSRateController($scope, UtilsService, VRNavigationService) {
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

            $scope.title = 'Future Rate of Mobile Network ' + mobileNetworkName;

            if (futureSMSRate != undefined) {
                $scope.title = 'Future  Rate of Mobile Network ' + mobileNetworkName;
                $scope.scopeModel.rate = futureSMSRate.Rate;
                $scope.scopeModel.rateBED = UtilsService.getShortDate(new Date(futureSMSRate.BED));
                $scope.scopeModel.rateEED = UtilsService.getShortDate(new Date(futureSMSRate.EED));
            }

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
        }
    }

    appControllers.controller('WhS_SMSBusinessEntity_FutureSMSRateController', FutureSMSRateController);

})(appControllers);