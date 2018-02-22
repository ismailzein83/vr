"use strict";

app.directive("vrGenericdataGenericbusinessentityRuntimeeditor", ["UtilsService", "VRNotificationService", "VRUIUtilsService",'VR_GenericData_GenericBEDefinitionAPIService', 'VR_GenericData_GenericBusinessEntityAPIService',
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

            var titleFieldName;

            var runtimeEditorAPI;
            var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();
            var context;
            var historyId;
            var isEditMode;
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

                    var fieldValues = {};
                    runtimeEditorAPI.setData(fieldValues);

                    genericBusinessEntity.FieldValues = fieldValues;

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
                        var title = genericBusinessEntity != undefined && genericBusinessEntity.FieldValues != undefined ? UtilsService.buildTitleForUpdateEditor(genericBusinessEntity.FieldValues[titleFieldName], definitionTitle) : UtilsService.buildTitleForAddEditor(definitionTitle);
                        context.setTitle(title);
                    }
                }

                function loadEditorRuntimeDirective() {
                    var runtimeEditorLoadDeferred = UtilsService.createPromiseDeferred();

                    runtimeEditorReadyDeferred.promise.then(function () {
                        var runtimeEditorPayload = {
                            selectedValues: (isEditMode) ? genericBusinessEntity.FieldValues : undefined,
                            dataRecordTypeId: businessEntityDefinitionSettings.DataRecordTypeId,
                            definitionSettings: businessEntityDefinitionSettings.EditorDefinition.Settings,
                            historyId: historyId
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