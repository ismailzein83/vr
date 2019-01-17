(function (app) {

    'use strict';

    AnalyticLegend.$inject = ['UtilsService', 'VRUIUtilsService'];

    function AnalyticLegend(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var itemconfigJoinEditor = new ItemconfigJoinEditor(ctrl, $scope, $attrs);
                itemconfigJoinEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },

            templateUrl: function (element, attrs) {
                return '/Client/Modules/Analytic/Directives/Definition/AnalyticReport/Templates/AnalyticLegendTemplate.html';
            }
        };

        function ItemconfigJoinEditor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var statusDefinitionViewerAPI;
            var statusDefinitionViewerReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var statusBeDefinitionId;
            var recommendedId;
            var analyticTableId;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onStatusDefinitionViewerReady = function (api) {
                    statusDefinitionViewerAPI = api;
                    statusDefinitionViewerReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.showLegend = false;

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        statusBeDefinitionId = payload.statusBeDefinitionId;
                        recommendedId = payload.recommendedId;
                        $scope.scopeModel.showLegend = payload.showLegend;
                        analyticTableId = payload.analyticTableId;
                        var loadStatusDefinitionViewerPromiseDeferred = UtilsService.createPromiseDeferred();
                       

                        statusDefinitionViewerReadyPromiseDeferred.promise.then(function () {

                            var payload =
                            {
                                statusBeDefinitionId: statusBeDefinitionId,
                                highlightedId: recommendedId,
                                analyticTableId: analyticTableId
                            };
                            VRUIUtilsService.callDirectiveLoad(statusDefinitionViewerAPI, payload, loadStatusDefinitionViewerPromiseDeferred);
                            promises.push(loadStatusDefinitionViewerPromiseDeferred.promise);
                        });
                        
                    }
                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {

            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                ctrl.onReady(api);
            }
        }
    }
}

    app.directive('vrAnalyticLegend', AnalyticLegend);

}) (app);
