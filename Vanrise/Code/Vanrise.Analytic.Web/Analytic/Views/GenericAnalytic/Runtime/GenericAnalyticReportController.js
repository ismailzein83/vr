(function (appControllers) {
    'use strict';

    genericAnalyticReportController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VR_Analytic_AnalyticReportAPIService', 'VR_Analytic_AnalyticConfigurationAPIService', 'VR_Analytic_AnalyticTypeEnum', 'VR_Analytic_AnalyticItemConfigAPIService'];

    function genericAnalyticReportController($scope, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService, VR_Analytic_AnalyticReportAPIService, VR_Analytic_AnalyticConfigurationAPIService, VR_Analytic_AnalyticTypeEnum, VR_Analytic_AnalyticItemConfigAPIService) {
        var analyticReportId;
        var viewEntity;
        var itemActionSettings;
        var autoSearch;
        var templates = [];
        var directiveAPI;
        var directiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != null) {
                analyticReportId = parameters.analyticReportId;
                itemActionSettings = parameters.settings;
                autoSearch = parameters.autoSearch;
                if (itemActionSettings != undefined )
                  itemActionSettings.AnalyticReportId = analyticReportId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                directiveReadyDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([getAnalyticReport]).then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([ loadTemplates, loadDirective]).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }

        }

        function loadDirective() {
            var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
            directiveReadyDeferred.promise.then(function () {
                var payLoad = {
                    settings: viewEntity.Settings,
                    itemActionSettings: itemActionSettings,
                    autoSearch: autoSearch
                };
                VRUIUtilsService.callDirectiveLoad(directiveAPI, payLoad, loadDirectivePromiseDeferred);
            });
            return loadDirectivePromiseDeferred.promise;
        }

        function loadTemplates()
        {
            VR_Analytic_AnalyticReportAPIService.GetAnalyticReportConfigTypes().then(function (response) {
                templates = response;
                $scope.scopeModel.selectedTemplate = UtilsService.getItemByVal(templates, viewEntity.Settings.ConfigId, "ExtensionConfigurationId");
            });
        }

        function getAnalyticReport() {
            return VR_Analytic_AnalyticReportAPIService.GetAnalyticReportById(analyticReportId).then(function (viewEntityObj) {
                viewEntity = viewEntityObj;
            });
        }
    }

    appControllers.controller('VR_Analytic_GenericAnalyticReportController', genericAnalyticReportController);

})(appControllers);
