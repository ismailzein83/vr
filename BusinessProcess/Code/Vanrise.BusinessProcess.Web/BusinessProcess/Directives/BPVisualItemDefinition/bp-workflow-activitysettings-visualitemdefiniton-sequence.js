//(function (app) {

//    'use strict';

//    VisualItemDefinitonSequence.$inject = ['UtilsService', 'VRUIUtilsService'];

//    function VisualItemDefinitonSequence(UtilsService, VRUIUtilsService) {
//        return {
//            restrict: "E",
//            scope: {
//                onReady: "=",
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var visualItemDefinitonSequence = new VisualItemDefinitonSequenceController($scope, ctrl, $attrs);
//                visualItemDefinitonSequence.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/Templates/VisualItemDefintionSequenceWorkflowTemplate.html"
//        };

//        function VisualItemDefinitonSequenceController($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var visualItemDefinition;

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.childVisualItems = [];
//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};
//                var childVisualItems = [];

//                api.load = function (payload) {
//                    var initialPromises = [];

//                    if (payload != undefined) {
//                        visualItemDefinition = payload.visualItemDefinition;
//                        childVisualItems = visualItemDefinition.Settings.ChildVisualItems;

//                    }
                  
//                    if (childVisualItems != undefined) {
//                        var childItemsLoadPromises = [];
//                        for (var i = 0; i < childVisualItems.length; i++) {
//                            var childVisualItem = childVisualItems[i];

//                            childVisualItem.readyPromiseDeferred = UtilsService.createPromiseDeferred();
//                            childVisualItem.loadPromiseDeferred = UtilsService.createPromiseDeferred();

//                            childItemsLoadPromises.push(childVisualItem.loadPromiseDeferred.promise);
//                            loadChildDirective(childVisualItem);
//                        }
//                        console.log($scope.scopeModel.childVisualItems);
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

//            function loadChildDirective(childVisualItem) {
//                childVisualItem.onDirectiveReady = function (api) {
//                    childVisualItem.directiveAPI = api;
//                    childVisualItem.readyPromiseDeferred.resolve();
//                };

//                childVisualItem.readyPromiseDeferred.promise.then(function () {
//                    var setLoader = function (value) {
//                        $scope.scopeModel.isLoadingDirective = value;
//                    };
//                    var payload = {
//                        visualItemDefinition: childVisualItem.ChildItemDefinition
//                    };
//                    VRUIUtilsService.callDirectiveLoad(childVisualItem.directiveAPI, payload, childVisualItem.loadPromiseDeferred);
//                });
//                $scope.scopeModel.childVisualItems.push(childVisualItem);
//            }
//        }
//    }

//    app.directive('bpWorkflowActivitysettingsVisualitemdefinitonSequence', VisualItemDefinitonSequence);

//})(app);