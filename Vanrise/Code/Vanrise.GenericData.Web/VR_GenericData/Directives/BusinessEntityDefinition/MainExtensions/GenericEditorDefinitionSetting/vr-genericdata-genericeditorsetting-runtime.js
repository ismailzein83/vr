"use strict";

app.directive("vrGenericdataGenericeditorsettingRuntime", ["UtilsService", "VRNotificationService", "VRUIUtilsService","VR_GenericData_DataRecordFieldAPIService","VR_GenericData_GenericUIRuntimeAPIService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_GenericUIRuntimeAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericEditorDefinitionSetting($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/GenericEditorRuntimeSettingTemplate.html"
        };
        function GenericEditorDefinitionSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var sectionDirectiveApi;
            var sectionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
            var selectedValues;
            var runtimeRows;
            var definitionSettings;
            var dataRecordTypeId;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSectionDirectiveReady = function (api) {
                    sectionDirectiveApi = api;
                    sectionDirectivePromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined)
                    {
                        selectedValues = payload.selectedValues;
                        definitionSettings = payload.definitionSettings;
                        dataRecordTypeId = payload.dataRecordTypeId;
                    }
                    var editorpromise = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultipleAsyncOperations([getGenericEditorRuntimeRows, loadRunTimeFieldTypeTemplates]).then(function () {
                        loadBusinessEntityDefinitionSelector().then(function () {
                            editorpromise.resolve();
                        }).catch(function (error) {
                            editorpromise.reject(error);
                        });
                    }).catch(function (error) {
                        editorpromise.reject(error);
                    });
                    promises.push(editorpromise.promise);

                    function loadBusinessEntityDefinitionSelector() {
                        var sectionDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                        sectionDirectivePromiseDeferred.promise.then(function () {
                            var payloadSelector = {
                                rows: runtimeRows,
                                context: getContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(sectionDirectiveApi, payloadSelector, sectionDirectiveLoadDeferred);
                        });
                        return sectionDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                function getContext() {
                    var context = {
                        getRuntimeEditor: getRuntimeEditor,
                        getFieldPathValue: getFieldPathValue
                    };
                    return context;
                }

                function getRuntimeEditor(configId) {
                    if ($scope.fieldTypeConfigs != undefined) {
                        var dataRecordFieldTypeConfig = UtilsService.getItemByVal($scope.fieldTypeConfigs, configId, 'ExtensionConfigurationId');
                        if (dataRecordFieldTypeConfig != undefined)
                            return dataRecordFieldTypeConfig.RuntimeEditor;
                    }
                }
                function getFieldPathValue(fieldPath) {
                    if (selectedValues != undefined && fieldPath != undefined)
                        return selectedValues[fieldPath];
                }
                function loadRunTimeFieldTypeTemplates() {
                    return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                        if (response) {
                            $scope.fieldTypeConfigs = response;
                        }
                    });
                }
                function getGenericEditorRuntimeRows()
                {
                    var input = {
                        Rows: definitionSettings != undefined? definitionSettings.Rows:definitionSettings,
                        DataRecordTypeId: dataRecordTypeId
                    };
                    return VR_GenericData_GenericUIRuntimeAPIService.GetGenericEditorRuntimeRows(input).then(function (response) {
                        runtimeRows = response;
                    });
                }

                api.setData = function (dicData) {
                    var sectionData = sectionDirectiveApi.getData();
                    if (sectionData != undefined)
                    {
                        for(var prop in sectionData)
                        {
                            dicData[prop] = sectionData[prop];
                        }
                    }
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


        }

        return directiveDefinitionObject;
    }
]);