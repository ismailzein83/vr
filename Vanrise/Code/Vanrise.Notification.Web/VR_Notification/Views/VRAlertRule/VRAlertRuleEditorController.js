(function (appControllers) {

    "use strict";

    VRAlertRuleEditorController.$inject = ['$scope', 'VR_Notification_VRAlertRuleAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function VRAlertRuleEditorController($scope, VR_Notification_VRAlertRuleAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;

        var vrAlertRuleId;
        var vrAlertRuleEntity;

        var settingsDirectiveAPI;
        var settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                vrAlertRuleId = parameters.vrAlertRuleId;
            }

            isEditMode = (vrAlertRuleId != undefined);
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
                getVRAlertRule().then(function () {
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

        function getVRAlertRule() {
            return VR_Notification_VRAlertRuleAPIService.GetVRAlertRule(vrAlertRuleId).then(function (response) {
                vrAlertRuleEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var vrAlertRuleName = (vrAlertRuleEntity != undefined) ? vrAlertRuleEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrAlertRuleName, 'VRAlertRule');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('VRAlertRule');
                }
            }
            function loadStaticData() {
                if (vrAlertRuleEntity == undefined)
                    return;
                $scope.scopeModel.name = vrAlertRuleEntity.Name;
            }
            function loadSettingsDirective() {
                var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                settingsDirectiveReadyDeferred.promise.then(function () {
                    var settingsDirectivePayload;
                    if (vrAlertRuleEntity != undefined) {
                        settingsDirectivePayload = { vrAlertRuleSettings: vrAlertRuleEntity.VRAlertRuleSettings };
                    }
                    VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, settingsDirectivePayload, settingsDirectiveLoadDeferred);
                });

                return settingsDirectiveLoadDeferred.promise;
            }
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return VR_Notification_VRAlertRuleAPIService.AddVRAlertRule(buildVRAlertRuleObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('VRAlertRule', response, 'Name')) {
                    if ($scope.onVRAlertRuleAdded != undefined)
                        $scope.onVRAlertRuleAdded(response.InsertedObject);
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
            return VR_Notification_VRAlertRuleAPIService.UpdateVRAlertRule(buildVRAlertRuleObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('VRAlertRule', response, 'Name')) {
                    if ($scope.onVRAlertRuleUpdated != undefined) {
                        $scope.onVRAlertRuleUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildVRAlertRuleObjFromScope() {
            var vrAlertRuleSettings = settingsDirectiveAPI.getData();

            return {
                VRAlertRuleId: vrAlertRuleEntity != undefined ? vrAlertRuleEntity.VRAlertRuleId : undefined,
                Name: $scope.scopeModel.name,
                VRAlertRuleSettings: vrAlertRuleSettings
            };
        }
    }

    appControllers.controller('VR_Notification_VRAlertRuleEditorController', VRAlertRuleEditorController);

})(appControllers);