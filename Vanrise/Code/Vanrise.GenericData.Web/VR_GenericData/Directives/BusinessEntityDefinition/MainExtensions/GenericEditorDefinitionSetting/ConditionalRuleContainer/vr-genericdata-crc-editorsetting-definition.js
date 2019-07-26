(function (app) {

    'use strict';

    ConditionalRuleContainerDefinition.$inject = ['VRUIUtilsService', 'UtilsService', 'VR_GenericData_GenericBEDefinitionService'];

    function ConditionalRuleContainerDefinition(VRUIUtilsService, UtilsService, VR_GenericData_GenericBEDefinitionService) {
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

            var context;
            var genericEditorConditionalRule;
            var containerType;
            var editorDefinitionSetting;

            var editorDefinitionSettingDirectiveAPI;
            var editorDefinitionSettingDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGenericBEFieldEditorDefinitionDirectiveReady = function (api) {
                    editorDefinitionSettingDirectiveAPI = api;
                    editorDefinitionSettingDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.openCRCSettings = function () {
                    var crcEntityObject = UtilsService.cloneObject({ genericEditorConditionalRule: genericEditorConditionalRule });

                    var onCRCSettingsChanged = function (crcSettingsItem) {
                        genericEditorConditionalRule = crcSettingsItem.genericEditorConditionalRule;
                    };
                    VR_GenericData_GenericBEDefinitionService.openGenericBEConditionalRuleContainerSettings(onCRCSettingsChanged, crcEntityObject, getContext());
                };
             
                $scope.scopeModel.validate = function () {
                    if (genericEditorConditionalRule == undefined)
                        return "You should add at least one condition";
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        context = payload.context;
                        containerType = payload.containerType;

                        var settings = payload.settings;
                        if (settings != undefined) {
                            genericEditorConditionalRule = settings.GenericEditorConditionalRule;
                            editorDefinitionSetting = settings.EditorDefinitionSetting;
                        }
                    }

                    promises.push(loadFieldEditorDefinition());
                    var rootNodePromises = {
                        promises: promises
                    };

                    return UtilsService.waitPromiseNode(rootNodePromises);
                };


                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.ConditionalRuleContainerEditorDefinitionSetting, Vanrise.GenericData.MainExtensions",
                        EditorDefinitionSetting: editorDefinitionSettingDirectiveAPI.getData(),
                        GenericEditorConditionalRule: genericEditorConditionalRule
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

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

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }
    }

    app.directive('vrGenericdataConditionalrulecontainereditorsettingDefinition', ConditionalRuleContainerDefinition);

})(app);