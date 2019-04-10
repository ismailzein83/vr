//(function (app) {

//    'use strict';

//    WorkflowActivitySettingsVisualItemSelective.$inject = ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService', 'BusinessProcess_BPDefinitionAPIService'];

//    function WorkflowActivitySettingsVisualItemSelective(UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService, BusinessProcess_BPDefinitionAPIService) {
//        return {
//            restrict: "E",
//            scope: {
//                onReady: "=",
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var workflowActivitySettingsVisualItemSelective = new VisualItemSelectiveController($scope, ctrl, $attrs);
//                workflowActivitySettingsVisualItemSelective.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/Templates/VisualItemDefintionDirectiveTemplate.html"
//        };

//        function VisualItemSelectiveController($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var bpDefinitionEntity; 

//            var directiveAPI;
//            var directiveReadyDeferred;
//            var directivePayload;

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.visualItemDefinitions = [];
//                $scope.scopeModel.selectedTemplateConfig;

//                $scope.scopeModel.onDirectiveReady = function (api) {
//                    directiveAPI = api;
//                    var setLoader = function (value) {
//                        $scope.scopeModel.isLoadingDirective = value;
//                    };
//                    var payload = {
//                        visualItemDefinition:  $scope.scopeModel.visualItemDefinition
//                    };
//                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, payload, setLoader, directiveReadyDeferred);
//                };

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var initialPromises = [];

//                    if (payload != undefined) {
//                        initialPromises.push(getBPDefintion(payload.bpDefinitionID));
//                    }

//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            var directivePromises = [];

//                            if (bpDefinitionEntity != undefined && bpDefinitionEntity.VRWorkflowId != undefined)
//                                directivePromises.push(getVRWorkflowVisualItemDefinition(bpDefinitionEntity.VRWorkflowId));

//                            return {
//                                promises: directivePromises
//                            };
//                        }
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
//                        bpDefinitionEntity = undefined;
//                    });

//                };

//                api.getData = function () {
                    
//                };

//                if (ctrl.onReady != null) {
//                    ctrl.onReady(api);
//                }
//            }

//            function getBPDefintion(bpDefinitionId) {
//                return BusinessProcess_BPDefinitionAPIService.GetBPDefintion(bpDefinitionId).then(function (response) {
//                    bpDefinitionEntity = response;
//                });
//            }

//            function getVRWorkflowVisualItemDefinition(vrWorkflowId) {
//                return BusinessProcess_VRWorkflowAPIService.GetVisualItemDefinition(vrWorkflowId).then(function (response) {
//                    $scope.scopeModel.visualItemDefinition = response;
//                    console.log($scope.scopeModel.visualItemDefinition);
//                });
//            }
//        }
//    }

//    app.directive('bpWorkflowActivitysettingsVisualitemdefiniton', WorkflowActivitySettingsVisualItemSelective);

//})(app);