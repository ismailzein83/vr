//(function (app) {

//    'use strict';

//    VisualItemDefinitonCallHttpService.$inject = ['UtilsService', 'VRUIUtilsService'];

//    function VisualItemDefinitonCallHttpService(UtilsService, VRUIUtilsService) {
//        return {
//            restrict: "E",
//            scope: {
//                onReady: "=",
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var visualItemDefinitonCallHttpService = new VisualItemDefinitonCallHttpServiceController($scope, ctrl, $attrs);
//                visualItemDefinitonCallHttpService.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/Templates/VisualItemDefintionCallHttpServiceWorkflowTemplate.html"
//        };

//        function VisualItemDefinitonCallHttpServiceController($scope, ctrl, $attrs) {
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
//                        $scope.scopeModel.displayName = visualItemDefinition.Settings.DisplayName;
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

//    app.directive('bpWorkflowActivitysettingsVisualitemdefinitonCallhttpservice', VisualItemDefinitonCallHttpService);

//})(app);