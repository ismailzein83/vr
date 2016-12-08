﻿(function (appControllers) {

    "use strict";

    switchEditorController.$inject = ["$scope", "WhS_BE_SwitchAPIService", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService"];

    function switchEditorController($scope, WhS_BE_SwitchAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var switchId;
        var switchEditorDirectiveAPI;
        var switchEditorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                switchId = parameters.switchId;
            }

            isEditMode = (switchId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.hasSaveSwitchPermission = function () {
                if (isEditMode)
                    return WhS_BE_SwitchAPIService.HasUpdateSwitchPermission();
                else
                    return WhS_BE_SwitchAPIService.HasAddSwitchPermission();
            };

            $scope.scopeModel.saveSwitch = function () {
                $scope.isLoading = true;
                switchEditorDirectiveAPI.save().then(function () {
                    $scope.modalContext.closeModal();
                }).finally(function () {
                    $scope.isLoading = false;
                })
            };

            $scope.onSwitchEditorReady = function (api) {
                switchEditorDirectiveAPI = api;
                switchEditorReadyDeferred.resolve();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.isLoading = true;
           loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSwitchEditorDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
        function loadSwitchEditorDirective() {
            var switchEditorLoadDeferred = UtilsService.createPromiseDeferred();
            switchEditorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(switchEditorDirectiveAPI, { switchId: switchId }, switchEditorLoadDeferred);
            });
            return switchEditorLoadDeferred.promise.then(function () {
                setTitle();
            });
        }
        function setTitle() {
            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor(switchEditorDirectiveAPI.getTitle(), 'SwitchName');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('SwitchName');
            }
        }
        
    }

    appControllers.controller("WhS_BE_SwitchEditorController", switchEditorController);
})(appControllers);
