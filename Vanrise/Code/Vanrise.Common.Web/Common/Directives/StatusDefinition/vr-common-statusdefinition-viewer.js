(function (app) {

    'use strict';

    StatusDefinitionViewer.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Common_StatusDefinitionAPIService','VR_Analytic_AnalyticTableService'];

    function StatusDefinitionViewer(UtilsService, VRUIUtilsService, VR_Common_StatusDefinitionAPIService, VR_Analytic_AnalyticTableService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var statusDefinitionViewer = new StatusDefinitionViewer(ctrl, $scope, $attrs);
                statusDefinitionViewer.initializeController();
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
                return '/Client/Modules/Common/Directives/StatusDefinition/Templates/StatusDefinitionViewerTemplate.html';
            }
        };

        function StatusDefinitionViewer(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var highlightedId;
            var statusBeDefinitionId;
            var analyticTableId;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.statusDefinitionStyles = [];
                $scope.onHelpBoxClicked = function () {
                    VR_Analytic_AnalyticTableService.viewMeasureStyles(analyticTableId,statusBeDefinitionId,highlightedId);
                };
                $scope.showHelpBox = false;
                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        statusBeDefinitionId = payload.statusBeDefinitionId;
                        $scope.showHelpBox = statusBeDefinitionId != undefined;
                        highlightedId = payload.highlightedId;
                        analyticTableId = payload.analyticTableId;


                        if (statusBeDefinitionId != undefined) {
                            var statusDefinitionStylesPromise = VR_Common_StatusDefinitionAPIService.GetStatusDefinitionStylesByBusinessEntityId(statusBeDefinitionId).then(function (response) {
                                if (response != undefined) {
                                    for (var i = 0; i < response.length; i++) {
                                        var statusDefinitionStyle = response[i];
                                        if (statusDefinitionStyle.StatusDefinitionId == highlightedId)
                                            statusDefinitionStyle.LabelClass = "bold-label";
                                        $scope.statusDefinitionStyles.push(statusDefinitionStyle);
                                    }
                                }

                            });
                            promises.push(statusDefinitionStylesPromise);
                        }
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

    app.directive('vrCommonStatusdefinitionViewer', StatusDefinitionViewer);

})(app);
