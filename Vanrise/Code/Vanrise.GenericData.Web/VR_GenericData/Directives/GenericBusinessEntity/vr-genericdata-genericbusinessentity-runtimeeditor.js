"use strict";

app.directive("vrGenericdataGenericbusinessentityRuntimeeditor", ["UtilsService", "VRNotificationService", "VRUIUtilsService", 'VR_GenericData_GenericBEDefinitionAPIService', 'VR_GenericData_GenericBusinessEntityAPIService',
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_GenericBusinessEntityAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBusinessEntityEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
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

            var titleFieldName;

            var runtimeEditorAPI;
            var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();
            var context;
            var historyId;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                    runtimeEditorAPI = api;
                    runtimeEditorReadyDeferred.resolve();
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
                    if (fieldValues != undefined) {
                        for (var prop in fieldValues) {
                            var fieldValue = fieldValues[prop];
                            if (!fieldValue.visibility)
                                fieldValuesObj[prop] = fieldValue.value;
                        }
                    }

                    runtimeEditorAPI.setData(fieldValuesObj);

                    genericBusinessEntity.FieldValues = fieldValuesObj;

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
                            titleFieldName = response.TitleFieldName;
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
                                context.setTitle(UtilsService.buildTitleForUpdateEditor(response, definitionTitle));
                            });
                        } else {
                            context.setTitle(UtilsService.buildTitleForAddEditor(definitionTitle));
                        }
                    }
                }

                function loadEditorRuntimeDirective() {
                    var runtimeEditorLoadDeferred = UtilsService.createPromiseDeferred();
                    runtimeEditorReadyDeferred.promise.then(function () {
                        var defaultValues = {};
                        for (var prop in fieldValues) {
                            var propValue = fieldValues[prop];
                            if (propValue.default)
                                defaultValues[prop] = propValue.value;
                        }

                        var runtimeEditorPayload = {
                            selectedValues: (isEditMode) ? genericBusinessEntity.FieldValues : defaultValues,
                            dataRecordTypeId: businessEntityDefinitionSettings.DataRecordTypeId,
                            definitionSettings: businessEntityDefinitionSettings.EditorDefinition.Settings,
                            historyId: historyId,
                            parentFieldValues: fieldValues
                        };
                        VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, runtimeEditorPayload, runtimeEditorLoadDeferred);
                    });

                    return runtimeEditorLoadDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadEditorRuntimeDirective, setTitle]);
            }
        }

        return directiveDefinitionObject;
    }
]);