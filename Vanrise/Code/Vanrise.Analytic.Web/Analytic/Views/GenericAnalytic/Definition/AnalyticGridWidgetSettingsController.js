(function (appControllers) {

    "use strict";

    AnalyticGridWidgetSettingsController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_Analytic_AnalyticConfigurationAPIService', 'VR_Analytic_StyleCodeEnum'];

    function AnalyticGridWidgetSettingsController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Analytic_AnalyticConfigurationAPIService, VR_Analytic_StyleCodeEnum) {

        var measureStyleRules;
        var context = [];

        var measureStyleGridAPI;
        var measureStyleGridReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                context = parameters.context;
                measureStyleRules = parameters.measureStyleRules;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onMeasureStyleGridDirectiveReady = function (api) {
                measureStyleGridAPI = api;
                measureStyleGridReadyDeferred.resolve();
            };

            $scope.scopeModel.saveSettings = function () {
                    return save();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;

            loadAllControls();

            function loadAllControls() {

                function setTitle() {
                        $scope.title = "Grid Settings";
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadMeasureStyles]).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
        }

        function loadMeasureStyles() {
            var loadMeasureStyleGridDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
            measureStyleGridReadyDeferred.promise.then(function () {
                var payloadMeasureStyleGridDirective = {
                    context: context,

                    measureStyles: measureStyleRules != undefined ? measureStyleRules : undefined
                };
                VRUIUtilsService.callDirectiveLoad(measureStyleGridAPI, payloadMeasureStyleGridDirective, loadMeasureStyleGridDirectivePromiseDeferred);
            });
            return loadMeasureStyleGridDirectivePromiseDeferred.promise;
        }

        function buildSettingsObjectFromScope() {
            var obj = {
                MeasureStyleRules: measureStyleGridAPI != undefined ? measureStyleGridAPI.getData() : undefined,
            };
            return obj;
        }

        function save() {
            var settingsObj = buildSettingsObjectFromScope();
            if ($scope.onSaveSettings != undefined)
                $scope.onSaveSettings(settingsObj);
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('VR_Analytic_AnalyticGridWidgetSettingsController', AnalyticGridWidgetSettingsController);
})(appControllers);
