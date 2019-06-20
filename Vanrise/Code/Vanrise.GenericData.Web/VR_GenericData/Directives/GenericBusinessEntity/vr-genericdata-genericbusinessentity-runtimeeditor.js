"use strict";

app.directive("vrGenericdataGenericbusinessentityRuntimeeditor", ["UtilsService", "VRUIUtilsService", 'VR_GenericData_GenericBusinessEntityAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBusinessEntityEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Templates/GenericBusinessEntityEditor.html"
        };

        function GenericBusinessEntityEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var isEditMode;
            var businessEntityDefinitionId;
            var businessEntityDefinitionSettings;

            var genericBusinessEntityEditorRuntime;
            var definitionTitle;

            var genericBusinessEntityId;
            var genericBusinessEntity;

            var fieldValues;
            var context;
            var historyId;

            var rootRuntimeEditorAPI;
            var rootRuntimeEditorReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRootEditorRuntimeDirectiveReady = function (api) {
                    rootRuntimeEditorAPI = api;
                    rootRuntimeEditorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        genericBusinessEntityId = payload.genericBusinessEntityId;
                        fieldValues = payload.fieldValues;
                        historyId = payload.historyId;
                        context = payload.context;

                        if (payload.isReadOnly)
                            UtilsService.setContextReadOnly($scope);
                    }

                    isEditMode = (genericBusinessEntityId != undefined || historyId != undefined);

                    var loadPromise = UtilsService.createPromiseDeferred();
                    promises.push(loadPromise.promise);

                    getGenericBusinessEntityEditorRuntime().then(function () {
                        loadAllControls().then(function () {
                            genericBusinessEntity = undefined;
                            loadPromise.resolve();
                        }).catch(function (error) {
                            loadPromise.reject(error);
                        });
                    }).catch(function (error) {
                        loadPromise.reject(error);
                    });

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var genericBusinessEntity = {};
                    genericBusinessEntity.GenericBusinessEntityId = genericBusinessEntityId;
                    genericBusinessEntity.BusinessEntityDefinitionId = businessEntityDefinitionId;

                    var fieldValuesObj = {};
                    var nullFields = [];
                    if (fieldValues != undefined) {
                        for (var prop in fieldValues) {
                            var fieldValue = fieldValues[prop];
                            if (fieldValue != undefined)
                                fieldValuesObj[prop] = fieldValue.value;
                        }
                    }

                    rootRuntimeEditorAPI.setData(fieldValuesObj);
                    if (fieldValuesObj != undefined) {
                        for (var field in fieldValuesObj) {
                            if (fieldValuesObj[field] == undefined) {
                                nullFields.push(field);
                            }
                        }
                    }

                    genericBusinessEntity.FieldValues = fieldValuesObj;
                    genericBusinessEntity.NullFields = nullFields;
                    return genericBusinessEntity;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getGenericBusinessEntityEditorRuntime() {
                return VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityEditorRuntime(businessEntityDefinitionId, genericBusinessEntityId, historyId).then(function (response) {
                    genericBusinessEntityEditorRuntime = response;
                    if (genericBusinessEntityEditorRuntime != undefined) {
                        definitionTitle = response.DefinitionTitle;
                        if (context != undefined) {
                            context.setTitleFieldName(response.TitleFieldName);
                            context.setTitleOperation(definitionTitle);
                        }
                        genericBusinessEntity = response.GenericBusinessEntity;
                        businessEntityDefinitionSettings = response.GenericBEDefinitionSettings;
                        if (businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.EditorDefinition != undefined && businessEntityDefinitionSettings.EditorDefinition.Settings != undefined)
                            $scope.scopeModel.runtimeEditor = businessEntityDefinitionSettings.EditorDefinition.Settings.RuntimeEditor;
                    }
                });
            }

            function loadAllControls() {

                function setTitle() {
                    if (context != undefined) {
                        if (isEditMode) {
                            return VR_GenericData_GenericBusinessEntityAPIService.GetGenericBETitleFieldValue(businessEntityDefinitionId, genericBusinessEntityId).then(function (response) {
                                context.setTitle(UtilsService.buildTitleForUpdateEditor(response, definitionTitle, $scope));
                            });
                        } else {
                            context.setTitle(UtilsService.buildTitleForAddEditor(definitionTitle));
                        }
                    }
                }

                function loadRootEditorRuntimeDirective() {
                    var runtimeEditorLoadDeferred = UtilsService.createPromiseDeferred();
                    rootRuntimeEditorReadyDeferred.promise.then(function () {
                        var defaultValues;
                        if (fieldValues != undefined) {
                            defaultValues = {};
                            for (var prop in fieldValues) {
                                var propValue = fieldValues[prop];
                                if (!propValue.isHidden)
                                    defaultValues[prop] = propValue.value;
                            }
                        }

                        var runtimeEditorPayload = {
                            selectedValues: (isEditMode) ? genericBusinessEntity.FieldValues : defaultValues,
                            dataRecordTypeId: businessEntityDefinitionSettings.DataRecordTypeId,
                            definitionSettings: businessEntityDefinitionSettings.EditorDefinition.Settings,
                            historyId: historyId,
                            parentFieldValues: fieldValues,
                            runtimeEditor: $scope.scopeModel.runtimeEditor
                        };
                        VRUIUtilsService.callDirectiveLoad(rootRuntimeEditorAPI, runtimeEditorPayload, runtimeEditorLoadDeferred);
                    });

                    return runtimeEditorLoadDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadRootEditorRuntimeDirective, setTitle]);
            }
        }

        return directiveDefinitionObject;
    }
]);