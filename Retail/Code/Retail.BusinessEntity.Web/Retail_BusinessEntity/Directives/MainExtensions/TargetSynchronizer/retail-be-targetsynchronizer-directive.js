'use strict';

app.directive('retailBeTargetsynchronizerDirective', ['VRNotificationService',
    function (vrNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var targetAnalysis = new targetAnalysisDirective($scope, ctrl, $attrs);
                targetAnalysis.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/TargetSynchronizer/Templates/TargetSynchronizerTemplate.html'
        };

        function targetAnalysisDirective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {

                };
                api.getData = function () {
                    return {
                        $type: "Vanrise.BEBridge.Entities.TargetBESynchronizer, Vanrise.BEBridge.Entities"

                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
