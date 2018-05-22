
"use strict";
app.directive("vrAnalyticAutomatedreportquerydefinitionSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var querydefinition = new Querydefinition($scope, ctrl, $attrs);
            querydefinition.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/AutomatedReport/Queries/Templates/AutomatedReportQueryDefinitionSettings.html"
    };


    function Querydefinition($scope, ctrl, $attrs) {
        this.initializeController = initializeController;


        var automatedReportQueryDefinitionSettingsDirectiveAPI;
        var automatedReportQueryDefinitionSettingsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onReadyAutomatedReportQueryDefinitionSelectorReady = function (api) {
                automatedReportQueryDefinitionSettingsDirectiveAPI = api;
                automatedReportQueryDefinitionSettingsSelectorReadyPromiseDeferred.resolve();

                var setLoader = function (value) {
                    $scope.scopeModel.isDirectiveLoading = value;
                };

                var directivePayload = {};
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, automatedReportQueryDefinitionSettingsDirectiveAPI, directivePayload, setLoader, automatedReportQueryDefinitionSettingsSelectorReadyPromiseDeferred);
            };
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var promises = [];
                var loadautomatedReportQueryDefinitionSettingsPromiseDeferred = UtilsService.createPromiseDeferred();

                automatedReportQueryDefinitionSettingsSelectorReadyPromiseDeferred.promise.then(function () {

                    var automatedReportSettingsPayload = {};

                    if (payload != undefined && payload.componentType != undefined) {
                        $scope.scopeModel.Name = payload.componentType.Name;
                        automatedReportSettingsPayload = payload.componentType.Settings;
                    }
                    VRUIUtilsService.callDirectiveLoad(automatedReportQueryDefinitionSettingsDirectiveAPI, automatedReportSettingsPayload, loadautomatedReportQueryDefinitionSettingsPromiseDeferred);
                });
                promises.push(loadautomatedReportQueryDefinitionSettingsPromiseDeferred.promise);

                return UtilsService.waitMultiplePromises(promises);
            };
            api.getData = function () {

                return {
                    Name: $scope.scopeModel.Name,
                    Settings: {
                        $type: "Vanrise.Analytic.Entities.VRAutomatedReportQueryDefinitionSettings,Vanrise.Analytic.Entities",
                        ExtendedSettings: automatedReportQueryDefinitionSettingsDirectiveAPI.getData(),
                    },
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}
]);