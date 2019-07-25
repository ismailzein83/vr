//'use strict';

//app.directive('businessprocessVrWorkflowVariablesExpandablegrid', ['BusinessProcess_VRWorkflowService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService',
//    function (BusinessProcess_VRWorkflowService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService) {

//        var directiveDefinitionObject = {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//                readOnly: '='
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new VrWorkflowVariablesGridDirectiveCtor(ctrl, $scope, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            compile: function (element, attrs) {
//                return {
//                    pre: function ($scope, iElem, iAttrs, ctrl) {

//                    }
//                };
//            },
//            templateUrl: "/Client/Modules/BusinessProcess/Directives/VRWorkflow/Variables/Templates/VRWorkflowVariablesExpandableGridTemplate.html"
//        };

//        function VrWorkflowVariablesGridDirectiveCtor(ctrl, $scope, attrs) {
//            this.initializeController = initializeController;

//            var gridAPI;
//            var context;
//            var variableTypeDirectiveAPI;
//            var variableTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
//            var reserveVariableName;
//            var eraseVariableName;
//            var isVariableNameReserved;
//            var gridDrillDownTabsObj;
//            var vrWorkflowVariables;

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.datasource = [];

//                $scope.scopeModel.onGridReady = function (api) {
//                    gridAPI = api;
//                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(defineWorkFlowVariableDrillDownTabs(), gridAPI, $scope.scopeModel.gridMenuActions);
//                    defineAPI();
//                };

//                $scope.scopeModel.addVRWorkflowVariable = function () {
//                    //var onVRWorkflowVariableAdded = function (addedVariable) {
//                    //    getVRWorkflowVariableTypeDescription(addedVariable).then(function () {
//                    //        $scope.scopeModel.datasource.push({ Entity: addedVariable });
//                    //        reserveVariableName(addedVariable.Name);
//                    //    });
//                    //};
//                    //BusinessProcess_VRWorkflowService.addVRWorkflowVariable(onVRWorkflowVariableAdded, isVariableNameReserved);
//                    var dataItem = {
//                        Entity: { VRWorkflowVariableId: UtilsService.guid() }
//                    };
//                    gridDrillDownTabsObj.setDrillDownExtensionObject(dataItem);
//                    $scope.scopeModel.datasource.push(dataItem);
//                    gridAPI.expandRow(dataItem);
//                };

//                $scope.scopeModel.removeVRWorkflowVariable = function (dataItem) {
//                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.Entity.VRWorkflowVariableId, 'Entity.VRWorkflowVariableId');
//                    $scope.scopeModel.datasource.splice(index, 1);
//                    eraseVariableName(dataItem.Entity.Name);
//                };
//                $scope.scopeModel.onCollapseRow = function (dataItem) {
//                    setVRWorkflowVariableNameAndTypeDescription(dataItem.variableDirectiveAPI.getData());
//                };

//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];

//                    var vRWorkflowVariablesTypeDescription;

//                    if (payload != undefined) {
//                        isVariableNameReserved = payload.isVariableNameReserved;
//                        reserveVariableName = payload.reserveVariableName;
//                        eraseVariableName = payload.eraseVariableName;
//                        vrWorkflowVariables = payload.vrWorkflowVariables;
//                        vRWorkflowVariablesTypeDescription = payload.vRWorkflowVariablesTypeDescription;
//                    }

//                    if (vrWorkflowVariables != undefined) {
//                        for (var i = 0; i < vrWorkflowVariables.length; i++) {
//                            var gridVariableItem = vrWorkflowVariables[i];

//                            var vRWorkflowVariableTypeDescription = vRWorkflowVariablesTypeDescription[gridVariableItem.VRWorkflowVariableId];
//                            gridVariableItem.TypeDescription = vRWorkflowVariableTypeDescription;

//                            reserveVariableName(gridVariableItem.Name);
//                            var dataItem = { Entity: gridVariableItem };
//                            gridDrillDownTabsObj.setDrillDownExtensionObject(dataItem);
//                            $scope.scopeModel.datasource.push(dataItem);
//                        }
//                    }

//                    var rootPromiseNode = {
//                        promises: promises
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };

//                api.getData = function () {
//                    if ($scope.scopeModel.datasource != undefined) {
//                        vrWorkflowVariables = [];
//                        for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
//                            vrWorkflowVariables.push($scope.scopeModel.datasource[i].variableDirectiveAPI != undefined ? $scope.scopeModel.datasource[i].variableDirectiveAPI.getData() : $scope.scopeModel.datasource[i].Entity);
//                        }
//                    }
//                    return vrWorkflowVariables;
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }

//            //function editVRWorkflowVariable(variableObj) {
//            //    var onVRWorkflowVariableUpdated = function (updatedVariable) {
//            //        getVRWorkflowVariableTypeDescription(updatedVariable).then(function () {
//            //            var index = $scope.scopeModel.datasource.indexOf(variableObj);
//            //            $scope.scopeModel.datasource[index] = { Entity: updatedVariable };
//            //            eraseVariableName(variableObj.Entity.Name);
//            //            reserveVariableName(updatedVariable.Name);
//            //        });
//            //    };
//            //    BusinessProcess_VRWorkflowService.editVRWorkflowVariable(variableObj.Entity, onVRWorkflowVariableUpdated);
//            //}

//            function setVRWorkflowVariableNameAndTypeDescription(vrWorkflowVariable) {
//                return BusinessProcess_VRWorkflowAPIService.GetVRWorkflowVariableTypeDescription(vrWorkflowVariable.Type).then(function (response) {

//                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, vrWorkflowVariable.VRWorkflowVariableId, 'Entity.VRWorkflowVariableId');
//                    $scope.scopeModel.datasource[index].Entity.TypeDescription = response;
//                    $scope.scopeModel.datasource[index].Entity.Name = vrWorkflowVariable.Name;
//                });
//            }

//            function defineWorkFlowVariableDrillDownTabs(workFlowVariable, gridAPI) {
//                var drillDownTabs = [];
//                drillDownTabs.push(buildVariableDefinitionDrillDownTab());
//                //  setDrillDownTabs();

//                function buildVariableDefinitionDrillDownTab() {
//                    var drillDownTab = {};
//                    drillDownTab.title = "Definition";
//                    drillDownTab.directive = "bp-vr-workflow-variable-directive";

//                    drillDownTab.loadDirective = function (variableDirectiveAPI, variableObj) {
//                        variableObj.variableDirectiveAPI = variableDirectiveAPI;
//                        return variableObj.variableDirectiveAPI.load(buildVariablePayload(variableObj));
//                    };

//                    function buildVariablePayload(variableObj) {
//                        var variablePayload = {};
//                        variablePayload.Entity = variableObj.Entity;
//                        variablePayload.context = buildContext();
//                        return variablePayload;
//                    }

//                    return drillDownTab;
//                }

//                //function setDrillDownTabs() {
//                //    var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI);
//                //    drillDownManager.setDrillDownExtensionObject(workFlowVariable);
//                //}

//                return drillDownTabs;
//            }

//            function buildContext() {
//                var currentContext = context;
//                if (currentContext == undefined) {
//                    currentContext = {};
//                    currentContext.isVariableNameReserved = isVariableNameReserved;
//                    currentContext.reserveVariableName = reserveVariableName;
//                    currentContext.eraseVariableName = eraseVariableName;
//                }
//                return currentContext;
//            }
//        }

//        return directiveDefinitionObject;
//    }]);