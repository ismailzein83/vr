//'use strict';

//app.directive('businessprocessVrWorkflowActivityFieldtypeExpressionbuilder', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowService', 'BusinessProcess_CustomCodeTaskService',
//	function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowService, BusinessProcess_CustomCodeTaskService) {

//		var directiveDefinitionObject = {
//			restrict: 'E',
//			scope: {
//                isrequired: '=',
//                onReady: '=',
//				label: '@',
//				hidelabel: '=',
//				nbOfRows: '='
//			},
//			controller: function ($scope, $element, $attrs) {
//				var ctrl = this;
//				if (ctrl.nbOfRows != undefined)
//					$scope.nbOfRows = ctrl.nbOfRows;
//				else
//					$scope.nbOfRows = 1;
//				var ctor = new workflowAssign(ctrl, $scope, $attrs);
//				ctor.initializeController();
//			},
//			controllerAs: 'ctrl',
//			bindToController: true,
//			compile: function (element, attrs) {

//			},
//            templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowActivityFieldTypeExpressionBuilderTemplate.html'
//		};

//		function workflowAssign(ctrl, $scope, $attrs) {

//			this.initializeController = initializeController;
//            function initializeController() {
//                $scope.scopeModel = {};
//                var context;
//                var codeExpression;
//                var fieldValue;
//                var fieldType = {
//                    $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions",
//                    CanRoundValue: false,
//                    ConfigId: "3f29315e-b542-43b8-9618-7de216cd9653",
//                    ConvertFilterMethod: null,
//                    DetailViewerEditor: "vr-genericdata-datarecordfield-defaultdetailviewer",
//                    DifferenceEditor: null,
//                    GetValueMethod: null,
//                    Hint: null,
//                    OrderType: 1,
//                    RuntimeEditor: "vr-genericdata-fieldtype-text-runtimeeditor",
//                    RuntimeViewSettingEditor: null,
//                    StoreValueSerialized: false,
//                    TextType: undefined,
//                    ViewerEditor: "vr-genericdata-fieldtype-text-viewereditor"
//                };
//				if (ctrl.label == undefined)
//                    ctrl.label = "Value";
//                //setTimeout(function () {
//                //    if (ctrl.value != undefined) {
//                //        $scope.scopeModel.codeExpression = ctrl.value.CodeExpression; 
//                //        $scope.$apply();
//                //    }
//                //    $scope.$watch('ctrl.value', function (newValue) {
//                //        if (newValue) {
//                //            if (newValue.CodeExpression != undefined) {
//                //                $scope.scopeModel.codeExpression = newValue.CodeExpression;
//                //            }
//                //            else if (newValue.Value != undefined) {
//                //                $scope.scopeModel.codeExpression = newValue.Value;
//                //            }
//                //        }
//                //        else
//                //            $scope.scopeModel.codeExpression = undefined;
//                //    });
//                //    $scope.$watch('scopeModel.codeExpression', function (newCodeExpression) {
//                //        if (newCodeExpression != undefined && newCodeExpression != '') {
//                //            ctrl.value = {
//                //                "$type": "Vanrise.BusinessProcess.Entities.VRWorkflowCodeExpression, Vanrise.BusinessProcess.Entities",
//                //                "CodeExpression": newCodeExpression
//                //            };
//                //        }
//                //    });
//                //    $scope.$watch('scopeModel.fieldValue', function (newFieldValue) {
//                //        if (newFieldValue != undefined && newFieldValue != '') {
//                //            ctrl.value = {
//                //                "$type": "Vanrise.BusinessProcess.Entities.VRWorkflowFieldTypeExpression, Vanrise.BusinessProcess.Entities",
//                //                "Value": newFieldValue
//                //            };
//                //        }
//                //    });
//                //}, 500);

//                $scope.scopeModel.disableExpression = function () {
//                    return false;
//                };
//				$scope.scopeModel.openExpressionBuilder = function () {
//                    var args = context != undefined ? context.getWorkflowArguments() : undefined;
//                    var vars = context != undefined ? context.getParentVariables() : undefined;
//					var onSetExpressionBuilder = function (expressionBuilderValue) {
//                        $scope.scopeModel.expression = expressionBuilderValue;
//                    };
//                    var onSetFieldValue = function (value) {
//                        fieldValue = value;
//                    };
//                    BusinessProcess_VRWorkflowService.openFieldTypeExpressionBuilder(onSetExpressionBuilder, onSetFieldValue, args, vars, fieldType, fieldValue, $scope.scopeModel.expression );
//                };

//                defineAPI();

//                function defineAPI() {
//                    var api = {};

//                    api.load = function (payload) {

//                        if (payload != undefined) {
//                            context = payload.context;
//                            fieldValue = payload.fieldValue;
//                            $scope.scopeModel.expression  = payload.codeExpression;
//                        }

//                        var rootPromiseNode = {
//                            promises: []
//                        };
//                        return UtilsService.waitPromiseNode(rootPromiseNode);
//                    };

//                    api.getData = function () {
//                        if ($scope.scopeModel.expression  != undefined && $scope.scopeModel.expression  != '') {
//                            return {
//                                $type: "Vanrise.BusinessProcess.Entities.VRWorkflowCodeExpression, Vanrise.BusinessProcess.Entities",
//                                CodeExpression: $scope.scopeModel.expression 
//                            };
//                        }
//                        else if (fieldValue != undefined && fieldValue != '') {
//                            return {
//                                $type: "Vanrise.BusinessProcess.Entities.VRWorkflowFieldTypeExpression, Vanrise.BusinessProcess.Entities",
//                                Value: fieldValue
//                            }
//                        }
//                    };

//                    if (ctrl.onReady != null)
//                        ctrl.onReady(api);
//                }

//            }


//		}
//		return directiveDefinitionObject;
//	}]);