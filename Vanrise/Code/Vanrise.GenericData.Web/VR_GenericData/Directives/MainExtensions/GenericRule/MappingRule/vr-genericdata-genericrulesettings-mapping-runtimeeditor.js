'use strict';
app.directive('vrGenericdataGenericrulesettingsMappingRuntimeeditor', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectionmode: '@',
                isrequired: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var obj = new mappingRuleSettingRuntimeEditor(ctrl, $scope, $attrs);
                obj.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {
                    }
                };
            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/GenericRule/MappingRule/Templates/MappingRuleSettingsRuntimeEditor.html"
        };

        function mappingRuleSettingRuntimeEditor(ctrl, $scope, $attrs) {

            var fieldTypeOnReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var fieldTypeDirectiveAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isrequired = ctrl.isrequired;
                $scope.scopeModel.onRuntimeEditorDirectiveReady = function (api) {
                    fieldTypeDirectiveAPI = api;
                    fieldTypeOnReadyPromiseDeferred.resolve();
                };
                
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var genericRuleDefinition;
                    var settings;

                    if (payload != undefined) {
                        genericRuleDefinition = payload.genericRuleDefinition,
                        settings = payload.settings;
                    }

                    if (genericRuleDefinition.SettingsDefinition != null) {
                        var fieldType = genericRuleDefinition.SettingsDefinition.FieldType;
                        var fieldTitle = genericRuleDefinition.SettingsDefinition.FieldTitle;

                        var promises = [];

                        var getFieldTypeConfigPromise = getFieldTypeConfig();
                        promises.push(getFieldTypeConfigPromise);

                        var loadFieldTypeDirectiveDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadFieldTypeDirectiveDeferred.promise);

                        getFieldTypeConfigPromise.then(function () {
                            fieldTypeOnReadyPromiseDeferred.promise.then(function () {
                                var payload = {
                                    fieldTitle: fieldTitle,
                                    fieldType: fieldType,
                                    fieldValue: (settings != undefined) ? settings.Value : undefined
                                };
                                VRUIUtilsService.callDirectiveLoad(fieldTypeDirectiveAPI, payload, loadFieldTypeDirectiveDeferred);
                            });
                        });

                        return UtilsService.waitMultiplePromises(promises);

                        function getFieldTypeConfig() {
                            return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                                if (response) {
                                    var item = UtilsService.getItemByVal(response, fieldType.ConfigId, "ExtensionConfigurationId");
                                    if (item != undefined) {
                                        $scope.scopeModel.runtimeEditorDirective = item.RuntimeEditor;
                                    }
                                }
                            });
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Transformation.Entities.MappingRuleSettings, Vanrise.GenericData.Transformation.Entities",
                        Value: fieldTypeDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);