﻿(function (appControllers) {

    "use strict";

    VRAlertRuleEditorController.$inject = ['$scope', 'VR_Notification_VRAlertRuleAPIService', 'VR_Notification_VRAlertRuleTypeAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function VRAlertRuleEditorController($scope, VR_Notification_VRAlertRuleAPIService, VR_Notification_VRAlertRuleTypeAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;

        var directiveAPI;
        var directiveReadyDeferred;

        var vrAlertRuleId;
        var vrAlertRuleEntity;
        var vrAlertRuleTypeId;
        var vrAlertRuleTypeEntity;

        var vrAlertRuleTypeSelectorAPI;
        var vrAlertRuleTypeSelectoReadyDeferred = UtilsService.createPromiseDeferred();
        var vrAlertRuleTypeSelectionChangedDeferred;

        var context;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                vrAlertRuleId = parameters.vrAlertRuleId;
                context = parameters.context;
            }
            isEditMode = (vrAlertRuleId != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.isAlertRuleTypeSelected = false;
            $scope.scopeModel.selectedRuleType;
            $scope.scopeModel.SettingEditor;


            $scope.scopeModel.onAlertRuleTypeSelectorReady = function (api) {
                vrAlertRuleTypeSelectorAPI = api;
                vrAlertRuleTypeSelectoReadyDeferred.resolve();
            };

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, getDirectiveWrapperPayload(), setLoader, directiveReadyDeferred);
            };

            $scope.scopeModel.onAlertRuleTypeSelectionChanged = function (api) {
                vrAlertRuleTypeId = vrAlertRuleTypeSelectorAPI.getSelectedIds();
                if (vrAlertRuleTypeId != undefined) {
                    getVRAlertRuleType().then(function () {
                        $scope.scopeModel.isAlertRuleTypeSelected = true;

                        if (vrAlertRuleTypeSelectionChangedDeferred != undefined) {
                            vrAlertRuleTypeSelectionChangedDeferred.resolve();
                        }
                    });
                }
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

        function loadRuleTypeSection() {
            var promises = [];

            var ruleTypeSelectorPromise = loadVRAlertRuleTypeSelector();
            promises.push(ruleTypeSelectorPromise);

            if (isEditMode) {
                directiveReadyDeferred = UtilsService.createPromiseDeferred();

                var directivePromise = loadDirective();
                promises.push(directivePromise);
            }

            UtilsService.waitMultiplePromises(promises);
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

        function getVRAlertRuleType() {
            $scope.scopeModel.SettingEditor = undefined;
            return VR_Notification_VRAlertRuleTypeAPIService.GetVRAlertRuleType(vrAlertRuleTypeId).then(function (response) {
                vrAlertRuleTypeEntity = response;
                $scope.scopeModel.SettingEditor = response.Settings.SettingEditor;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadRuleTypeSection]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        }

        function setTitle() {
            if (isEditMode) {
                var vrAlertRuleName = (vrAlertRuleEntity != undefined) ? vrAlertRuleEntity.Name : null;
                $scope.title = UtilsService.buildTitleForUpdateEditor(vrAlertRuleName, 'Action Rule');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Action Rule');
            }
        }

        function loadStaticData() {
            if (vrAlertRuleEntity == undefined)
                return;
            $scope.scopeModel.name = vrAlertRuleEntity.Name;
        }

        function loadVRAlertRuleTypeSelector() {

            var vrAlertRuleTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            vrAlertRuleTypeSelectoReadyDeferred.promise.then(function () {
                var vrAlertRuleTypeSelectorPayload;

                if (vrAlertRuleEntity != undefined && vrAlertRuleEntity.RuleTypeId != undefined) {
                    vrAlertRuleTypeSelectorPayload = {
                        selectedIds: vrAlertRuleEntity.RuleTypeId
                    };
                }
                VRUIUtilsService.callDirectiveLoad(vrAlertRuleTypeSelectorAPI, vrAlertRuleTypeSelectorPayload, vrAlertRuleTypeSelectorLoadDeferred);
            });

            return vrAlertRuleTypeSelectorLoadDeferred.promise;
        }

        function loadDirective() {

            var directiveLoadDeferred = UtilsService.createPromiseDeferred();

            directiveReadyDeferred.promise.then(function () {

                directiveReadyDeferred = undefined;
                VRUIUtilsService.callDirectiveLoad(directiveAPI, getDirectiveWrapperPayload(), directiveLoadDeferred);
                vrAlertRuleEntity = undefined;
            });

            return directiveLoadDeferred.promise;
        }

        function getDirectiveWrapperPayload() {
            return {
                alertTypeSettings: vrAlertRuleTypeEntity.Settings,
                alertExtendedSettings: vrAlertRuleEntity != undefined ? vrAlertRuleEntity.Settings.ExtendedSettings : undefined,
                context:getContext()
            };
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

            return {
                VRAlertRuleId: vrAlertRuleId,
                Name: $scope.scopeModel.name,
                RuleTypeId: vrAlertRuleTypeSelectorAPI.getSelectedIds(),
                Settings: {
                    RuleTypeId: null,
                    ExtendedSettings: directiveAPI.getData()
                }
            };
        }
        function getContext()
        {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }

    }

    appControllers.controller('VR_Notification_VRAlertRuleEditorController', VRAlertRuleEditorController);

})(appControllers);