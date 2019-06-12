//(function (app) {

//    'use strict';

//    ConditionalRuleFieldValueSetting.$inject = ['VRNotificationService', 'UtilsService'];

//    function ConditionalRuleFieldValueSetting(VRNotificationService, UtilsService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new ConditionalRuleFieldValueSettingCtor($scope, ctrl);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorConditionalRuleSetting/Templates/ConditionalRuleFieldValueSettingTemplate.html'
//        };

//        function ConditionalRuleFieldValueSettingCtor($scope, ctrl) {
//            this.initializeController = initializeController;

//            var fieldEditorDefinitionApi;
//            var fieldEditorDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            var genericEditorConditionalRuleApi;
//            var genericEditorConditionalRuleReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                $scope.scopeModel = {};

//                $scope.scopeModel.onGenericBEFieldEditorDefinitionDirectiveReady = function (api) {
//                    fieldEditorDefinitionApi = api;
//                    fieldEditorDefinitionReadyPromiseDeferred.resolve();
//                };

//                $scope.scopeModel.onGenericEditorConditionalRulesDirectiveReady = function (api) {
//                    genericEditorConditionalRuleApi = api;
//                    genericEditorConditionalRuleReadyPromiseDeferred.resolve();
//                };

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {

//                    var genericEditorContainerRule;

//                    var logicalOperator;
//                    var conditions;

//                    if (payload != undefined) {
//                        genericEditorContainerRule = payload.genericEditorContainerRule;

//                        if (genericEditorContainerRule != undefined) {
//                            logicalOperator = genericEditorContainerRule.LogicalOperator;
//                            conditions = genericEditorContainerRule.Conditions;
//                        }
//                    }

//                    var promises = [];

//                    var fieldEditorDefinitionLoadedPromise = loadFieldEditorDefinition();
//                    promises.push(fieldEditorDefinitionLoadedPromise);

//                    var genericEditorConditionalRuleLoadedPromise = loadGenericEditorConditionalRule();
//                    promises.push(genericEditorConditionalRuleLoadedPromise);

//                    var rootNodePromises = {
//                        promises: promises
//                    };

//                    function loadFieldEditorDefinition() {
//                        var fieldEditorDefinitionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

//                        fieldEditorDefinitionReadyPromiseDeferred.promise.then(function () {
//                            var payload = {
//                                settings: editorType,
//                                context: context,
//                                containerType: containerType
//                            };

//                            VRUIUtilsService.callDirectiveLoad(fieldEditorDefinitionApi, payload, fieldEditorDefinitionLoadPromiseDeferred);
//                        });

//                        return fieldEditorDefinitionLoadPromiseDeferred.promise;
//                    }

//                    function loadGenericEditorConditionalRule() {
//                        var genericEditorConditionalRuleLoadPromiseDeferred = UtilsService.createPromiseDeferred();

//                        genericEditorConditionalRuleReadyPromiseDeferred.promise.then(function () {
//                            var payload;

//                            if (genericEditorContainerRule != undefined) {
//                                payload = {
//                                    genericEditorContainerRule: genericEditorContainerRule
//                                };
//                            }

//                            VRUIUtilsService.callDirectiveLoad(genericEditorConditionalRuleApi, payload, genericEditorConditionalRuleLoadPromiseDeferred);
//                        });

//                        return genericEditorConditionalRuleLoadPromiseDeferred.promise;
//                    }

//                    return UtilsService.waitPromiseNode(rootNodePromises);
//                };


//                api.getData = function () {
//                    return {
//                        $type: "Vanrise.GenericData.MainExtensions.GenericEditorFieldValueContainerRule, Vanrise.GenericData.MainExtensions",
//                        LogicalOperator: undefined,
//                        Conditions: undefined
//                    };
//                };

//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }
//        }
//    }

//    app.directive('vrGenericeditorConditionalruleFieldvalue', ConditionalRuleFieldValueSetting);

//})(app);