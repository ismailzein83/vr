(function (app) {

    'use strict';

    DAProfCalcAlertRuleCriteriaDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function DAProfCalcAlertRuleCriteriaDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var daProfCalcAlertRuleCriteria = new DAProfCalcAlertRuleCriteria($scope, ctrl, $attrs);
                daProfCalcAlertRuleCriteria.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/AlertRuleExtensions/Templates/DAProfCalcAlertRuleCriteriaTemplate.html"

        };
        function DAProfCalcAlertRuleCriteria($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataAnalysisItemDefinitionSelectorAPI;
            var dataAnalysisItemDefinitionSelectoReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                var promises = [dataAnalysisItemDefinitionSelectoReadyDeferred.promise];

                $scope.scopeModel = {};

                $scope.scopeModel.onDataAnalysisItemDefinitionSelectorReady = function (api) {
                    dataAnalysisItemDefinitionSelectorAPI = api;
                    dataAnalysisItemDefinitionSelectoReadyDeferred.resolve();
                }


                UtilsService.waitMultiplePromises(promises).then(function () {
                    defineAPI();
                })
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var context;
                    var criteria;

                    if (payload != undefined) {
                        context = payload.context;
                        criteria = payload.criteria;
                    }
                        

                    //Loading Data Analysis Item Definition Selector
                    var dataAnalysisItemDefinitionSelectorLoadPromise = getDataAnalysisItemDefinitionSelectorLoadPromise();
                    promises.push(dataAnalysisItemDefinitionSelectorLoadPromise);


                    function getDataAnalysisItemDefinitionSelectorLoadPromise() {
                        var dataAnalysisItemDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        var dataAnalysisItemDefinitionSelectorPayload = {};
                        if(context != undefined){
                            dataAnalysisItemDefinitionSelectorPayload.dataAnalysisDefinitionId = context.getDataAnalysisDefinitionId();
                        }
                        if (criteria != undefined && criteria.DAProfCalcOutputItemDefinitionId != undefined) {
                            dataAnalysisItemDefinitionSelectorPayload.selectedIds = criteria.DAProfCalcOutputItemDefinitionId
                        }
                        VRUIUtilsService.callDirectiveLoad(dataAnalysisItemDefinitionSelectorAPI, dataAnalysisItemDefinitionSelectorPayload, dataAnalysisItemDefinitionSelectorLoadDeferred);

                        return dataAnalysisItemDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Analytic.Entities.DAProfCalcAlertRuleCriteria, Vanrise.Analytic.Entities",
                        DAProfCalcOutputItemDefinitionId: dataAnalysisItemDefinitionSelectorAPI.getSelectedIds()
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticDaprofcalcAlertrulecriteria', DAProfCalcAlertRuleCriteriaDirective);

})(app);