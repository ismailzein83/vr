(function (appControllers) {

    "use strict";

    genericAnalyticGridSettingsController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function genericAnalyticGridSettingsController($scope, VRNavigationService, UtilsService, VRUIUtilsService) {

        var genericSettingsAPI;
        var genericSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var measureThresholds;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                measureThresholds = parameters.measureThresholds;
            }
        }

        function defineScope() {

            $scope.onGenericSettingsReady = function (api) {
                genericSettingsAPI = api;
                genericSettingsReadyPromiseDeferred.resolve();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.saveSettings = function () {
                $scope.onSaveSettings(genericSettingsAPI.getData());
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoadingGenericSettings = true;
            loadingGenericSettings()
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                    $scope.isLoadingGenericSettings = false;
                })
                .finally(function () {
                    $scope.isLoadingGenericSettings = false;
                });
        }

        function loadingGenericSettings() {
            var loadGenericSettingsPromiseDeferred = UtilsService.createPromiseDeferred();

            genericSettingsReadyPromiseDeferred.promise.then(function () {
                var payload = loadPayload();
                console.log(payload);
                VRUIUtilsService.callDirectiveLoad(genericSettingsAPI, payload, loadGenericSettingsPromiseDeferred);
            });
            return loadGenericSettingsPromiseDeferred.promise;
        }

        function loadPayload() {
            var payload = {};
            payload.measureThresholds = measureThresholds;
            return payload;
        }
    }
    appControllers.controller('WhS_Analytics_GenericAnalyticGridSettingsController', genericAnalyticGridSettingsController);

})(appControllers);