(function (appControllers) {

    "use strict";

    VRAlertRuleTypeEditorController.$inject = ['$scope', 'VR_Notification_VRAlertRuleTypeAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function VRAlertRuleTypeEditorController($scope, VR_Notification_VRAlertRuleTypeAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;

        var directiveAPI;
        var directiveReadyDeferred;

        var selectorAPI;

        var vrAlertRuleTypeId;
        var vrAlertRuleTypeEntity;

        var vrAlertRuleTypeSettings;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                vrAlertRuleTypeId = parameters.vrAlertRuleTypeId;
            }

            isEditMode = (vrAlertRuleTypeId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.templateConfigs = [];
            $scope.scopeModel.selectedTemplateConfig;
            $scope.scopeModel.onSettingsSelectorReady = function (api) {
                selectorAPI = api;
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getVRAlertRuleType().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getVRAlertRuleType() {
            return VR_Notification_VRAlertRuleTypeAPIService.GetVRAlertRuleType(vrAlertRuleTypeId).then(function (response) {
                vrAlertRuleTypeEntity = response;
                vrAlertRuleTypeSettings = response.Settings;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadRuleTypeSection]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var vrAlertRuleTypeName = (vrAlertRuleTypeEntity != undefined) ? vrAlertRuleTypeEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrAlertRuleTypeName, 'Action Rule Type');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Action Rule Type');
                }
            }
            function loadStaticData() {
                if (vrAlertRuleTypeEntity == undefined)
                    return;
      
                $scope.scopeModel.name = vrAlertRuleTypeEntity.Name;
            }
        }

        function getVRAlertRuleTypeSettingsTemplateConfigs() {
            return VR_Notification_VRAlertRuleTypeAPIService.GetVRAlertRuleTypeSettingsExtensionConfigs().then(function (response) {

                if (response != null) {
                    for (var i = 0; i < response.length; i++) {
                        $scope.scopeModel.templateConfigs.push(response[i]);
                    }
                    if (vrAlertRuleTypeSettings != undefined) {
                        $scope.scopeModel.selectedTemplateConfig =
                            UtilsService.getItemByVal($scope.scopeModel.templateConfigs, vrAlertRuleTypeSettings.ConfigId, 'ExtensionConfigurationId');
                    }
                }
            });
        }

        function loadRuleTypeSection() {
            var promises = [];

            var ruleTypeSelectorPromise = getVRAlertRuleTypeSettingsTemplateConfigs();
            promises.push(ruleTypeSelectorPromise);

            if (isEditMode) {
                directiveReadyDeferred = UtilsService.createPromiseDeferred();

                var directivePromise = loadDirective();
                promises.push(directivePromise);
            }

            UtilsService.waitMultiplePromises(promises);
        }

        function loadDirective() {

            var directiveLoadDeferred = UtilsService.createPromiseDeferred();

            directiveReadyDeferred.promise.then(function () {
                directiveReadyDeferred = undefined;
                var directivePayload = { settings: vrAlertRuleTypeSettings };

                VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
            });

            return directiveLoadDeferred.promise;
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return VR_Notification_VRAlertRuleTypeAPIService.AddVRAlertRuleType(buildVRAlertRuleTypeObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('VRAlertRuleType', response, 'Name')) {
                    if ($scope.onVRAlertRuleTypeAdded != undefined)
                        $scope.onVRAlertRuleTypeAdded(response.InsertedObject);
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
            return VR_Notification_VRAlertRuleTypeAPIService.UpdateVRAlertRuleType(buildVRAlertRuleTypeObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('VRAlertRuleType', response, 'Name')) {
                    if ($scope.onVRAlertRuleTypeUpdated != undefined) {
                        $scope.onVRAlertRuleTypeUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildVRAlertRuleTypeObjFromScope() {

            return {
                VRAlertRuleTypeId: vrAlertRuleTypeEntity != undefined ? vrAlertRuleTypeEntity.VRAlertRuleTypeId : undefined,
                Name: $scope.scopeModel.name,
                Settings: buildAlertTypeSettings()
            };
        }

        function buildAlertTypeSettings() {
            var vrAlertRuleTypeSettings = directiveAPI.getData();
            vrAlertRuleTypeSettings.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
            return vrAlertRuleTypeSettings;
        }
    }

    appControllers.controller('VR_Notification_VRAlertRuleTypeEditorController', VRAlertRuleTypeEditorController);

})(appControllers);