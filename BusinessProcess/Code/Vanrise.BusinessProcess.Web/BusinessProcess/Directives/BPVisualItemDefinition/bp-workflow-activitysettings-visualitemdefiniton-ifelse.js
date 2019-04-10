//(function (app) {

//    'use strict';

//    VisualItemDefinitonIfElse.$inject = ['UtilsService', 'VRUIUtilsService'];

//    function VisualItemDefinitonIfElse(UtilsService, VRUIUtilsService) {
//        return {
//            restrict: "E",
//            scope: {
//                onReady: "=",
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var visualItemDefinitonIfElse = new VisualItemDefinitonIfElseController($scope, ctrl, $attrs);
//                visualItemDefinitonIfElse.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/Templates/VisualItemDefintionIfElseWorkflowTemplate.html"
//        };

//        function VisualItemDefinitonIfElseController($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var visualItemDefinition;

//            var trueBranchDirectiveAPI;
//            var trueBranchDirectivePromiseReadyDeferred = UtilsService.createPromiseDeferred();

//            var falseBranchDirectiveAPI;
//            var falseBranchDirectivePromiseReadyDeferred = UtilsService.createPromiseDeferred();
            
//            function initializeController() {
//                $scope.scopeModel = {};

//                $scope.scopeModel.onTrueBranchDirectiveReady = function (api) {
//                    trueBranchDirectiveAPI = api;
//                    trueBranchDirectivePromiseReadyDeferred.resolve();
//                };

//                $scope.scopeModel.onFalseBranchDirectiveReady = function (api) {
//                    falseBranchDirectiveAPI = api;
//                    falseBranchDirectivePromiseReadyDeferred.resolve();
//                };

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var initialPromises = [];

//                    if (payload != undefined) {
//                        visualItemDefinition = payload.visualItemDefinition;
//                        $scope.scopeModel.conditionDescription = visualItemDefinition.Settings.ConditionDescription;
//                        $scope.scopeModel.trueBranch = visualItemDefinition.Settings.TrueBranchVisualItemDefinition;
//                        $scope.scopeModel.falseBranch = visualItemDefinition.Settings.FalseBranchVisualItemDefinition;
//                    }

//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            var directivePromises = [];
//                            directivePromises.push(loadTrueBranchDirective());
//                            directivePromises.push(loadFalseBranchDirective());

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

//            function loadTrueBranchDirective(){
//                var loadTrueBranchDirectiveDeferred = UtilsService.createPromiseDeferred();

//                trueBranchDirectivePromiseReadyDeferred.promise.then(function () {
//                    var setLoader = function (value) {
//                        $scope.scopeModel.isLoadingDirective = value;
//                    };
//                    var payload = {
//                        visualItemDefinition: $scope.scopeModel.trueBranch
//                    };
//                    VRUIUtilsService.callDirectiveLoad(trueBranchDirectiveAPI, payload, loadTrueBranchDirectiveDeferred);
//                });
//                return loadTrueBranchDirectiveDeferred.promise;
//            }

//            function loadFalseBranchDirective() {
//                var loadFalseBranchDirectiveDeferred = UtilsService.createPromiseDeferred();

//                falseBranchDirectivePromiseReadyDeferred.promise.then(function () {
//                    var setLoader = function (value) {
//                        $scope.scopeModel.isLoadingDirective = value;
//                    };
//                    var payload = {
//                        visualItemDefinition: $scope.scopeModel.falseBranch
//                    };
//                    VRUIUtilsService.callDirectiveLoad(falseBranchDirectiveAPI, payload, loadFalseBranchDirectiveDeferred);
//                });
//                return loadFalseBranchDirectiveDeferred.promise;
//            }
//        }
//    }

//    app.directive('bpWorkflowActivitysettingsVisualitemdefinitonIfelse', VisualItemDefinitonIfElse);

//})(app);