(function (appControllers) {

    "use strict";

    VRAlertRuleTypeEditorController.$inject = ['$scope', 'VR_Notification_VRAlertRuleTypeAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function VRAlertRuleTypeEditorController($scope, VR_Notification_VRAlertRuleTypeAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;

        var vrAlertRuleTypeId;
        var vrAlertRuleTypeEntity;

        var settingsDirectiveAPI;
        var settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onSettingsDirectiveReady = function (api) {
                settingsDirectiveAPI = api;
                settingsDirectiveReadyDeferred.resolve();
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
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var vrAlertRuleTypeName = (vrAlertRuleTypeEntity != undefined) ? vrAlertRuleTypeEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrAlertRuleTypeName, 'Alert Rule Type');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Alert Rule Type');
                }
            }
            function loadStaticData() {
                if (vrAlertRuleTypeEntity == undefined)
                    return;
                $scope.scopeModel.name = vrAlertRuleTypeEntity.Name;
            }
            function loadSettingsDirective() {
                var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                settingsDirectiveReadyDeferred.promise.then(function () {
                    var settingsDirectivePayload;
                    if (vrAlertRuleTypeEntity != undefined) {
                        settingsDirectivePayload = { vrAlertRuleTypeSettings: vrAlertRuleTypeEntity.Settings };
                    }
                    VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, settingsDirectivePayload, settingsDirectiveLoadDeferred);
                });

                return settingsDirectiveLoadDeferred.promise;
            }
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
            var vrAlertRuleTypeSettings = settingsDirectiveAPI.getData();

            return {
                VRAlertRuleTypeId: vrAlertRuleTypeEntity != undefined ? vrAlertRuleTypeEntity.VRAlertRuleTypeId : undefined,
                Name: $scope.scopeModel.name,
                Settings: vrAlertRuleTypeSettings
            };
        }
    }

    appControllers.controller('VR_Notification_VRAlertRuleTypeEditorController', VRAlertRuleTypeEditorController);

})(appControllers);