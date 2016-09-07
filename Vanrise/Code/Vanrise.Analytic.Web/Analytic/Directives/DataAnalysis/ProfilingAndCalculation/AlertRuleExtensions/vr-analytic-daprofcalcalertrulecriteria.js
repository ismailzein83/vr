(function (app) {

    'use strict';

    DAProfCalcAlertRuleCriteria.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function DAProfCalcAlertRuleCriteria(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrDAProfCalcAlertRuleCriteria = new VRDAProfCalcAlertRuleCriteria($scope, ctrl, $attrs);
                vrDAProfCalcAlertRuleCriteria.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/AlertRuleExtensions/Templates/DAProfCalcAlertRuleCriteriaTemplate.html"

        };
        function VRDAProfCalcAlertRuleCriteria($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataAnalysisDefinitionSelectorAPI;
            var dataAnalysisDefinitionSelectoReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataAnalysisDefinitionSelectorReady = function (api) {
                    dataAnalysisDefinitionSelectorAPI = api;
                    dataAnalysisDefinitionSelectoReadyDeferred.resolve();
                }

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var vrAlertRuleTypeSettings;

                    if (payload != undefined)
                        vrAlertRuleTypeSettings = payload.vrAlertRuleTypeSettings;


                    var dataAnalysisDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    dataAnalysisDefinitionSelectoReadyDeferred.promise.then(function () {
                        var dataAnalysisDefinitionSelectorPayload;

                        if (vrAlertRuleTypeSettings != undefined) {
                            dataAnalysisDefinitionSelectorPayload = {
                                selectedIds: vrAlertRuleTypeSettings.DataAnalysisDefinitionId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(dataAnalysisDefinitionSelectorAPI, dataAnalysisDefinitionSelectorPayload, dataAnalysisDefinitionSelectorLoadDeferred);
                    });

                    return dataAnalysisDefinitionSelectorLoadDeferred.promise;
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Analytic.Entities.DAProfCalcAlertRuleCriteria, Vanrise.Analytic.Entities",
                        DataAnalysisDefinitionId: dataAnalysisDefinitionSelectorAPI.getSelectedIds()
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticDaprofcalcalertruletypesettings', DAProfCalcAlertRuleCriteria);

})(app);