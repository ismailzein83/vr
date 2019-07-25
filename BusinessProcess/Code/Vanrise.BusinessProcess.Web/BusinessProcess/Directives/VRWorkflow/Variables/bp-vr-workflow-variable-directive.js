//'use strict';

//app.directive('bpVrWorkflowVariableDirective', ['UtilsService', 'VRUIUtilsService',
//    function (UtilsService, VRUIUtilsService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '='
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new variableDirectiveCtor($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Variables/Templates/VRWorkflowVariableDirectiveTemplate.html'
//        };

//        function variableDirectiveCtor($scope, ctrl, attrs) {
//            this.initializeController = initializeController;

//            var vrWorkflowVariableEntity;
//            var name;
//            var variableTypeSelectorAPI;
//            var variableTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

//            var context;

//            function initializeController() {
//                $scope.scopeModel = {};

//                $scope.scopeModel.onVariableTypeSelectorReady = function (api) {
//                    variableTypeSelectorAPI = api;
//                    variableTypeSelectorReadyDeferred.resolve();
//                };

//                $scope.scopeModel.isVariableNameValid = function () {
//                    var variableName = ($scope.scopeModel.name != undefined) ? $scope.scopeModel.name.toLowerCase() : null;

//                    if (name != undefined && variableName == name.toLowerCase())
//                        return null;

//                    if (context != undefined && context.isVariableNameReserved != undefined && context.isVariableNameReserved(variableName))
//                        return 'Same variable name already exists';

//                    return null;
//                };

//                $scope.scopeModel.onNameFocus = function () {
//                    if ($scope.scopeModel.name != undefined && $scope.scopeModel.name.length > 0 && context.isVariableNameReserved($scope.scopeModel.name) && name == $scope.scopeModel.name) {
//                        context.eraseVariableName($scope.scopeModel.name);
//                    }
//                };

//                $scope.scopeModel.onNameBlur = function () {
//                    if ($scope.scopeModel.name != undefined && $scope.scopeModel.name.length > 0 && !context.isVariableNameReserved($scope.scopeModel.name)) {
//                        context.reserveVariableName($scope.scopeModel.name);
//                        name = $scope.scopeModel.name;
//                    }
//                };

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {

//                    var promises = [];

//                    if (payload != undefined) {
//                        context = payload.context;
//                        //context.isVariableNameReserved = isVariableNameReserved;
//                        //context.reserveVariableName = reserveVariableName;
//                        //context.eraseVariableName = eraseVariableName;

//                        vrWorkflowVariableEntity = payload.Entity;
//                        if (vrWorkflowVariableEntity != undefined) {
//                            name = vrWorkflowVariableEntity.Name;
//                            $scope.scopeModel.name = vrWorkflowVariableEntity.Name;
//                        }
//                    }

//                    promises.push(loadVariableTypeDirective());

//                    var rootPromiseNode = {
//                        promises: promises
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };

//                api.getData = function () {
//                    return buildVRWorkflowVariableObjFromScope();
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }

//            function loadVariableTypeDirective() {
//                var variableTypeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

//                variableTypeSelectorReadyDeferred.promise.then(function () {

//                    var variableTypeDirectivePayload = { selectIfSingleItem: true };
//                    if (vrWorkflowVariableEntity != undefined && vrWorkflowVariableEntity.Type != undefined) {
//                        variableTypeDirectivePayload.variableType = vrWorkflowVariableEntity.Type;
//                    }
//                    VRUIUtilsService.callDirectiveLoad(variableTypeSelectorAPI, variableTypeDirectivePayload, variableTypeDirectiveLoadDeferred);
//                });

//                return variableTypeDirectiveLoadDeferred.promise;
//            }

//            function buildVRWorkflowVariableObjFromScope() {
//                return {
//                    VRWorkflowVariableId: vrWorkflowVariableEntity != undefined ? vrWorkflowVariableEntity.VRWorkflowVariableId : UtilsService.guid(),
//                    Name: $scope.scopeModel.name,
//                    Type: variableTypeSelectorAPI.getData()
//                };
//            }

//        }
//    }]);