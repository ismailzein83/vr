//(function (app) {

//    'use strict';

//    ConditionalRuleContainerDefinition.$inject = ['VRNotificationService', 'UtilsService'];

//    function ConditionalRuleContainerDefinition(VRNotificationService, UtilsService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new ConditionalRuleContainerDefinitionCtor($scope, ctrl);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/ConditionalRuleContainerDefinitionSettingTemplate.html'
//        };

//        function ConditionalRuleContainerDefinitionCtor($scope, ctrl) {
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

//                    var settings;
//                    var containerType;
//                    var context;

//                    var editorType;
//                    var genericEditorConditionalRule;

//                    if (payload != undefined) {
//                        settings = payload.settings;
//                        context = payload.context;
//                        containerType = payload.containerType;

//                        if (settings != undefined) {
//                            genericEditorConditionalRule = settings.GenericEditorConditionalRule;
//                            editorType = settings.EditorType;
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

//                            if (genericEditorConditionalRule != undefined) {
//                                payload = {
//                                    genericEditorConditionalRule: genericEditorConditionalRule
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
//                        $type: "Vanrise.GenericData.MainExtensions.ConditionalRuleContainerSetting, Vanrise.GenericData.MainExtensions",
//                        EditorType: fieldEditorDefinitionApi.getData(),
//                        GenericEditorConditionalRule: genericEditorConditionalRuleApi.getData()
//                    };
//                };

//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }
//        }
//    }

//    app.directive('vrGenericdataConditionalrulecontainersettingDefinition', ConditionalRuleContainerDefinition);

//})(app);