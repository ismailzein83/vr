'use strict';
app.directive('vrGenericdataGenericrulesettingsRuntimeeditor', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldTypeConfigAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldTypeConfigAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectionmode: '@'
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
                }
            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/GenericRule/MappingRule/Templates/MappingRuleSettingsRuntimeEditor.html"
        };

        function mappingRuleSettingRuntimeEditor(ctrl, $scope, $attrs) {

            var fieldTypeOnReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var fieldTypeDirectiveAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRuntimeEditorDirectiveReady = function (api)
                {
                    fieldTypeDirectiveAPI = api;
                    fieldTypeOnReadyPromiseDeferred.resolve();
                }
                
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var fieldType;
                    var fieldTitle;

                    if (payload != undefined) {
                        fieldType = payload.fieldType;
                        fieldTitle = payload.fieldTitle;
                    }

                    if (fieldType != undefined) {

                        var promises = [];

                        var loadWholeSectionPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadWholeSectionPromiseDeferred.promise);

                        var loadFieldTypeConfigPromise = VR_GenericData_DataRecordFieldTypeConfigAPIService.GetDataRecordFieldTypeConfig(fieldType.ConfigId).then(function (fieldTypeConfig) {
                            $scope.scopeModel.runtimeEditorDirective = fieldTypeConfig.DynamicGroupUIControl;

                            var loadFieldTypeConfigDirective = UtilsService.createPromiseDeferred();

                            fieldTypeOnReadyPromiseDeferred.promise.then(function () {
                                var payload = {
                                    fieldTitle: fieldTitle,
                                    fieldType: fieldType
                                }

                                VRUIUtilsService.callDirectiveLoad(fieldTypeDirectiveAPI, payload, loadFieldTypeConfigDirective);
                                loadWholeSectionPromiseDeferred.resolve();
                            }).catch(function (error) {
                                loadWholeSectionPromiseDeferred.reject();
                            });
                        });

                        promises.push(loadFieldTypeConfigPromise);
                        return UtilsService.waitMultiplePromises(promises);
                    }
                }

                api.getData = function()
                {
                    return {
                        $type: "Vanrise.GenericData.Transformation.Entities.MappingRuleSettings, Vanrise.GenericData.Transformation.Entities",
                        Value: fieldTypeDirectiveAPI.getData()
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);