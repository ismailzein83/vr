(function (app) {

    'use strict';

    ConditionalRuleContainerDefinition.$inject = ['VRUIUtilsService', 'UtilsService'];

    function ConditionalRuleContainerDefinition(VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ConditionalRuleContainerDefinitionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/ConditionalRuleContainer/Templates/CRC_DefinitionSettingTemplate.html'
        };

        function ConditionalRuleContainerDefinitionCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var editorDefinitionSettingDirectiveAPI;
            var editorDefinitionSettingDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var genericEditorConditionalRuleAPI;
            var genericEditorConditionalRuleReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGenericBEFieldEditorDefinitionDirectiveReady = function (api) {
                    editorDefinitionSettingDirectiveAPI = api;
                    editorDefinitionSettingDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGenericEditorConditionalRulesDirectiveReady = function (api) {
                    genericEditorConditionalRuleAPI = api;
                    genericEditorConditionalRuleReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var context;
                    var containerType;
                    var genericEditorConditionalRule;
                    var editorDefinitionSetting;

                    if (payload != undefined) {
                        context = payload.context;
                        containerType = payload.containerType;

                        var settings = payload.settings;
                        if (settings != undefined) {
                            genericEditorConditionalRule = settings.GenericEditorConditionalRule;
                            editorDefinitionSetting = settings.EditorDefinitionSetting;
                        }
                    }

                    var promises = [];

                    var rootNodePromises = {
                        promises: promises
                    };

                    var editorDefinitionSettingDirectiveLoadedPromise = loadFieldEditorDefinition();
                    promises.push(editorDefinitionSettingDirectiveLoadedPromise);

                    var genericEditorConditionalRuleLoadedPromise = loadGenericEditorConditionalRule();
                    promises.push(genericEditorConditionalRuleLoadedPromise);

                    function loadFieldEditorDefinition() {
                        var editorDefinitionSettingDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        editorDefinitionSettingDirectiveReadyPromiseDeferred.promise.then(function () {
                           
                            var payload = {
                                settings: editorDefinitionSetting,
                                context: context,
                                containerType: containerType
                            };
                            VRUIUtilsService.callDirectiveLoad(editorDefinitionSettingDirectiveAPI, payload, editorDefinitionSettingDirectiveLoadPromiseDeferred);
                        });

                        return editorDefinitionSettingDirectiveLoadPromiseDeferred.promise;
                    }

                    function loadGenericEditorConditionalRule() {
                        var genericEditorConditionalRuleLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        genericEditorConditionalRuleReadyPromiseDeferred.promise.then(function () {
                            var payload = {
                                context: context
                            };

                            if (genericEditorConditionalRule != undefined) {
                                payload.genericEditorConditionalRule = genericEditorConditionalRule;
                            }

                            VRUIUtilsService.callDirectiveLoad(genericEditorConditionalRuleAPI, payload, genericEditorConditionalRuleLoadPromiseDeferred);
                        });

                        return genericEditorConditionalRuleLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode(rootNodePromises);
                };


                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.ConditionalRuleContainerEditorDefinitionSetting, Vanrise.GenericData.MainExtensions",
                        EditorDefinitionSetting: editorDefinitionSettingDirectiveAPI.getData(),
                        GenericEditorConditionalRule: genericEditorConditionalRuleAPI.getData()
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataConditionalrulecontainereditorsettingDefinition', ConditionalRuleContainerDefinition);

})(app);