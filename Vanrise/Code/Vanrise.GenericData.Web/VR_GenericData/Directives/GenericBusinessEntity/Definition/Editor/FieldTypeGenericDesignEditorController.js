(function (appControllers) {

    "use strict";

    FieldTypeGenericDesignEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService'];

    function FieldTypeGenericDesignEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {

        var fieldTypeEntitySettings;
        var context;
        var dataRecordFieldTypes = [];

        var fieldTypeRuntimeDirectiveAPI;
        var fieldTypeRuntimeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var localizationTextResourceSelectorAPI;
        var localizationResourceSelectorDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var runtimeViewSettingsEditorDirectiveAPI;
        var runtimeViewSettingsEditorDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                fieldTypeEntitySettings = parameters.fieldTypeEntity;
                context = parameters.context;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onFieldTypeRumtimeDirectiveReady = function (api) {
                fieldTypeRuntimeDirectiveAPI = api;
                fieldTypeRuntimeReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onLocalizationTextResourceDirectiveReady = function (api) {
                localizationTextResourceSelectorAPI = api;
                localizationResourceSelectorDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onRuntimeViewSettingsEditorDirectiveReady = function (api) {
                runtimeViewSettingsEditorDirectiveAPI = api;
                runtimeViewSettingsEditorDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.saveFieldTypeSettings = function () {
                return saveSettings();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function load() {
            var initialPromises = [];
            dataRecordFieldTypes.length = 0;

            $scope.scopeModel.isLoading = true;
            function setTitle() {
                $scope.title = UtilsService.buildTitleForUpdateEditor(fieldTypeEntitySettings.FieldPath, 'Field Settings');
            }

            function loadStaticData() {
                if (fieldTypeEntitySettings != undefined) {
                    $scope.scopeModel.fieldWidth = fieldTypeEntitySettings.FieldWidth;
                    $scope.scopeModel.isRequired = fieldTypeEntitySettings.IsRequired;
                    $scope.scopeModel.isDisabled = fieldTypeEntitySettings.IsDisabled;
                    $scope.scopeModel.showAsLabel = fieldTypeEntitySettings.ShowAsLabel;
                    $scope.scopeModel.hideLabel = fieldTypeEntitySettings.HideLabel;
                    $scope.scopeModel.readOnly = fieldTypeEntitySettings.ReadOnly;
                }
            }

            initialPromises.push(UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle]));
            initialPromises.push(getDataRecordFieldTypeConfigs());
            var rootPromiseNode = {
                promises: initialPromises,
                getChildNode: function () {
                    var directivePromises = [];

                    var fieldType = context.getFieldType(fieldTypeEntitySettings.FieldPath);
                    $scope.scopeModel.runtimeViewSettingEditor = fieldType.RuntimeViewSettingEditor;

                    var dataRecordFieldType = UtilsService.getItemByVal(dataRecordFieldTypes, fieldType.ConfigId, "ExtensionConfigurationId");
                    if (dataRecordFieldType != undefined) {
                        $scope.scopeModel.fieldTypeRuntimeDirective = dataRecordFieldType.RuntimeEditor;
                        if ($scope.scopeModel.fieldTypeRuntimeDirective != undefined)
                            directivePromises.push(loadFieldTypeRuntimeDirective());
                    }

                    if ($scope.scopeModel.runtimeViewSettingEditor != undefined)
                        directivePromises.push(loadRuntimeViewSettingsEditorDirective());

                    directivePromises.push(loadLocalizationResourceSelector());

                    return {
                        promises: directivePromises,
                    }
                }
            };

            return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function getDataRecordFieldTypeConfigs() {
            return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                for (var i = 0; i < response.length; i++) {
                    var dataRecordFieldType = response[i];
                    dataRecordFieldTypes.push(dataRecordFieldType);
                }
            });
        }

        function loadFieldTypeRuntimeDirective() {
            var fieldTypeRuntimeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            fieldTypeRuntimeReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {
                    fieldTitle: "Default Value",
                    fieldType: context.getFieldType(fieldTypeEntitySettings.FieldPath),
                    fieldName: fieldTypeEntitySettings.FieldPath,
                    fieldValue: fieldTypeEntitySettings.DefaultFieldValue
                };
                VRUIUtilsService.callDirectiveLoad(fieldTypeRuntimeDirectiveAPI, directivePayload, fieldTypeRuntimeLoadPromiseDeferred);
            });
            return fieldTypeRuntimeLoadPromiseDeferred.promise;
        }

        function loadLocalizationResourceSelector() {
            var localizationResourceSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            localizationResourceSelectorDirectiveReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    selectedValue: fieldTypeEntitySettings != undefined ? fieldTypeEntitySettings.TextResourceKey : undefined
                };
                VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, payload, localizationResourceSelectorLoadPromiseDeferred);
            });
            return localizationResourceSelectorLoadPromiseDeferred.promise;
        }

        function loadRuntimeViewSettingsEditorDirective() {
            var runtimeViewSettingsEditorDirectiveloadPromiseDeferred = UtilsService.createPromiseDeferred();
            runtimeViewSettingsEditorDirectiveReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    configId: fieldTypeEntitySettings.FieldViewSettings != undefined ? fieldTypeEntitySettings.FieldViewSettings.ConfigId : undefined,
                    context: context,
                    settings: fieldTypeEntitySettings.FieldViewSettings,
                    dataRecordTypeId: context.getDataRecordTypeId()
                };
                VRUIUtilsService.callDirectiveLoad(runtimeViewSettingsEditorDirectiveAPI, payload, runtimeViewSettingsEditorDirectiveloadPromiseDeferred);
            });
            return runtimeViewSettingsEditorDirectiveloadPromiseDeferred.promise;
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};

            currentContext.getFieldTypeWith = function () {
                return $scope.scopeModel.fieldWidth;
            }
            return currentContext;
        }

        function saveSettings() {
            var fieldTypeSettingsDefinition = buildObjectFromScope();
            if ($scope.onFiledTypeSettingsChanged != undefined) {
                $scope.onFiledTypeSettingsChanged(fieldTypeSettingsDefinition);
            }
            $scope.modalContext.closeModal();
        }

        function buildObjectFromScope() {
            return {
                FieldPath: fieldTypeEntitySettings.FieldPath,
                IsRequired: $scope.scopeModel.isRequired,
                IsDisabled: $scope.scopeModel.isDisabled,
                ShowAsLabel: $scope.scopeModel.showAsLabel,
                HideLabel: $scope.scopeModel.hideLabel,
                ReadOnly: $scope.scopeModel.readOnly,
                FieldWidth: $scope.scopeModel.fieldWidth,
                FieldViewSettings: runtimeViewSettingsEditorDirectiveAPI != undefined ? runtimeViewSettingsEditorDirectiveAPI.getData() : fieldTypeEntitySettings.oldRuntimeViewSettings,
                TextResourceKey: localizationTextResourceSelectorAPI != undefined ? localizationTextResourceSelectorAPI.getSelectedValues() : fieldTypeEntitySettings.oldTextResourceKey,
                DefaultFieldValue: fieldTypeRuntimeDirectiveAPI != undefined ? fieldTypeRuntimeDirectiveAPI.getData() : fieldTypeEntitySettings.oldFieldTypeRumtimeSettings,
            };
        }
    }

    appControllers.controller('VR_GenericData_FieldTypeGenericDesignEditorController', FieldTypeGenericDesignEditorController);
})(appControllers);
