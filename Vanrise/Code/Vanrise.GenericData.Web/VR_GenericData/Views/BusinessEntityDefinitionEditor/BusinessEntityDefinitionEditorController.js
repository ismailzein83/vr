(function (appControllers) {

    "use strict";

    GenericBEEditorDefintionController.$inject = ['$scope', 'VR_GenericData_BusinessEntityDefinitionAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VR_Sec_ViewAPIService', 'VR_Sec_MenuAPIService', 'InsertOperationResultEnum'];

    function GenericBEEditorDefintionController($scope, VR_GenericData_BusinessEntityDefinitionAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService, VR_Sec_ViewAPIService, VR_Sec_MenuAPIService, InsertOperationResultEnum) {

        var isEditMode;
        var businessEntityDefinitionEntity;
        var businessEntityDefinitionId;

        var settingDirectiveAPI;
        var settingReadyPromiseDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                businessEntityDefinitionId = parameters.businessEntityDefinitionId;
            }
            isEditMode = (businessEntityDefinitionId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onSettingDirectiveReady = function (api) {
                settingDirectiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, settingDirectiveAPI, undefined, setLoader, settingReadyPromiseDeferred);
            };

           
            $scope.scopeModel.bEDefinitionSettingConfigs = [];

            $scope.scopeModel.SaveGenericBEEditor = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.hasSaveGenericBEEditor = function () {
                if (isEditMode) {
                    return VR_GenericData_BusinessEntityDefinitionAPIService.HasUpdateBusinessEntityDefinition();
                }
                else {
                    return VR_GenericData_BusinessEntityDefinitionAPIService.HasAddBusinessEntityDefinition();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }
        function load() {
            $scope.scopeModel.isLoading = true;
            getBEDefinitionSettingConfigs().then(function () {
                if (isEditMode) {
                    getBusinessEntityDefinition().then(function () {
                        loadAllControls();
                    }).catch(function () {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModel.isLoading = false;
                    });
                }
                else {
                    loadAllControls();
                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });


            function loadAllControls() {

                function setTitle() {
                    if (isEditMode && businessEntityDefinitionEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(businessEntityDefinitionEntity.Name, 'Generic BE Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Generic BE Editor');
                }
                function loadStaticData() {
                    if (businessEntityDefinitionEntity != undefined) {
                        $scope.scopeModel.businessEntityName = businessEntityDefinitionEntity.Name;
                        $scope.scopeModel.businessEntityTitle = businessEntityDefinitionEntity.Title;
                    }
                }
                function loadSettingDirectiveSection() {
                    if (businessEntityDefinitionEntity != undefined && businessEntityDefinitionEntity.Settings != undefined) {
                        settingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                        $scope.scopeModel.selectedSetingsTypeConfig = UtilsService.getItemByVal($scope.scopeModel.bEDefinitionSettingConfigs, businessEntityDefinitionEntity.Settings.ConfigId, "ExtensionConfigurationId");

                        var loadSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        settingReadyPromiseDeferred.promise
                            .then(function () {
                                var directivePayload = {
                                    businessEntityDefinitionId: businessEntityDefinitionId,
                                    businessEntityDefinitionSettings: businessEntityDefinitionEntity.Settings,
                                };
                                VRUIUtilsService.callDirectiveLoad(settingDirectiveAPI, directivePayload, loadSettingDirectivePromiseDeferred);
                            });

                        return loadSettingDirectivePromiseDeferred.promise;
                    }
                }


                return UtilsService.waitMultipleAsyncOperations([loadStaticData, loadSettingDirectiveSection, setTitle]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }

            function getBusinessEntityDefinition() {
                return VR_GenericData_BusinessEntityDefinitionAPIService.GetBusinessEntityDefinition(businessEntityDefinitionId).then(function (businessEntityDefinition) {
                    businessEntityDefinitionEntity = businessEntityDefinition;
                });
            }
        }

        function getBEDefinitionSettingConfigs() {
            return VR_GenericData_BusinessEntityDefinitionAPIService.GetBEDefinitionSettingConfigs().then(function (response) {
                if (response) {
                    $scope.scopeModel.bEDefinitionSettingConfigs = response;
                }
            });
        }

        function buildGenericBEDefinitionFromScope() {
            var settings = settingDirectiveAPI.getData();
            if (settings != undefined) {
                settings.ConfigId = $scope.scopeModel.selectedSetingsTypeConfig.ExtensionConfigurationId;
            }
            var bEdefinition = {
                BusinessEntityDefinitionId: businessEntityDefinitionId,
                Name: $scope.scopeModel.businessEntityName,
                Title: $scope.scopeModel.businessEntityTitle,
                Settings: settings
            };
            return bEdefinition;
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            var genericBEDefinition = buildGenericBEDefinitionFromScope();
            return VR_GenericData_BusinessEntityDefinitionAPIService.AddBusinessEntityDefinition(genericBEDefinition).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Business Entity Definition', response, 'Name')) {
                    if ($scope.onBusinessEntityDefinitionAdded != undefined) {
                        $scope.onBusinessEntityDefinitionAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            var genericBEDefinition = buildGenericBEDefinitionFromScope();

            return VR_GenericData_BusinessEntityDefinitionAPIService.UpdateBusinessEntityDefinition(genericBEDefinition).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Business Entity Definition', response, 'Name')) {
                    if ($scope.onBusinessEntityDefinitionUpdated != undefined) {
                        $scope.onBusinessEntityDefinitionUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('VR_GenericData_GenericBEEditorDefintionController', GenericBEEditorDefintionController);
})(appControllers);
