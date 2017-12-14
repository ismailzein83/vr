(function (app) {

    'use strict';

    DAProfCalcSettingsSelector.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function DAProfCalcSettingsSelector(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var daProfCalcSettings = new DAProfCalcSettings($scope, ctrl, $attrs);
                daProfCalcSettings.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/Common/Templates/DAProfCalcSettingsTemplate.html"

        };
        function DAProfCalcSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var context;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDAProfCalcSettingsSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectorPayload;

                    if (payload != undefined && payload.dataAnalysisDefinitionSettings != undefined) {
                        selectorPayload = { selectedIds: payload.dataAnalysisDefinitionSettings.DataRecordTypeId };
                        $scope.scopeModel.hideActionRuleRecordFilter = payload.dataAnalysisDefinitionSettings.HideActionRuleRecordFilter;
                    }

                    var daProfCalcSettingsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, daProfCalcSettingsSelectorLoadDeferred);

                    return daProfCalcSettingsSelectorLoadDeferred.promise;
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Analytic.Entities.DAProfCalcSettings, Vanrise.Analytic.Entities",
                        DataRecordTypeId: selectorAPI.getSelectedIds(),
                        HideActionRuleRecordFilter: $scope.scopeModel.hideActionRuleRecordFilter
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticDaprofcalcSettings', DAProfCalcSettingsSelector);

})(app);