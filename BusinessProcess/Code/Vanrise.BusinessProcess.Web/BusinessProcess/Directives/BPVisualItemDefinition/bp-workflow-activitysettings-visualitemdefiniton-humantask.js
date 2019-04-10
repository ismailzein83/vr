//(function (app) {

//    'use strict';

//    VisualItemDefinitonHumanTask.$inject = ['UtilsService', 'VRUIUtilsService'];

//    function VisualItemDefinitonHumanTask(UtilsService, VRUIUtilsService) {
//        return {
//            restrict: "E",
//            scope: {
//                onReady: "=",
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var visualItemDefinitonHumanTask = new VisualItemDefinitonHumanTaskController($scope, ctrl, $attrs);
//                visualItemDefinitonHumanTask.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/Templates/VisualItemDefintionHumanTaskWorkflowTemplate.html"
//        };

//        function VisualItemDefinitonHumanTaskController($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var visualItemDefinition;

//            function initializeController() {
//                $scope.scopeModel = {};
//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var initialPromises = [];

//                    if (payload != undefined) {
//                        visualItemDefinition = payload.visualItemDefinition;
//                        $scope.scopeModel.displayName = visualItemDefinition.Settings.TaskName;
//                    }

//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            var directivePromises = [];

//                            return {
//                                promises: directivePromises
//                            };
//                        }
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };

//                api.getData = function () {

//                };

//                if (ctrl.onReady != null) {
//                    ctrl.onReady(api);
//                }
//            }
//        }
//    }

//    app.directive('bpWorkflowActivitysettingsVisualitemdefinitonHumantask', VisualItemDefinitonHumanTask);

//})(app);