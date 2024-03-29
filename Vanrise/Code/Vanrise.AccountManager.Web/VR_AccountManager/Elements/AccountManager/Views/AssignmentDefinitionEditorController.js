﻿(function (appControllers) {

    "use strict";

    assignmentDefinitionController.$inject = ["$scope", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService"];

    function assignmentDefinitionController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {
        var isEditMode;
        var assignmentDefinitionEntity;
        var assignmentDefinitionAPI;
        var assignmentDefinitionReadyDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                assignmentDefinitionEntity = parameters.assignmentDefinitionEntity;
            };
            isEditMode = (assignmentDefinitionEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.save = function () {
                if (!isEditMode) {
                    addAssignmentDefinition();
                }
                else {
                    updateAssignmnetDefinition();
                }
            };
            $scope.scopeModel.onAssignmentDefinitionSelectorReady = function (api) {
                assignmentDefinitionAPI = api;
                assignmentDefinitionReadyDeferred.resolve();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
                function setTitle() {
                  if(!isEditMode)
                      $scope.title = UtilsService.buildTitleForAddEditor('Assignment Definition');
                  else
                      $scope.title = UtilsService.buildTitleForUpdateEditor(assignmentDefinitionEntity.Name, 'Assignment Definition');
                }
                function loadStaticData() {
                    if (assignmentDefinitionEntity != undefined) {
                        $scope.scopeModel.assignmentName = assignmentDefinitionEntity.Name;
                    }
                }
                function loadDirective() {
                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                    assignmentDefinitionReadyDeferred.promise.then(function () {
                        var payload;
                        if (assignmentDefinitionEntity != undefined) {
                            payload = {
                                assignmentDefinitionEntity: assignmentDefinitionEntity.Settings
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(assignmentDefinitionAPI, payload, directiveLoadDeferred);
                    });
                    return directiveLoadDeferred.promise;
                }
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData,loadDirective]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }
        function buildObjectFromScope() {
            var assignmentDefiniton = {
                Name: $scope.scopeModel.assignmentName,
                Settings: assignmentDefinitionAPI.getData()
            };
            if (isEditMode) {
                assignmentDefiniton.AccountManagerAssignementDefinitionId = assignmentDefinitionEntity.AccountManagerAssignementDefinitionId;
            }
            else {
                assignmentDefiniton.AccountManagerAssignementDefinitionId = UtilsService.guid();
            }
            return assignmentDefiniton;
        }
        function addAssignmentDefinition() {
            var assignmentDefinition = buildObjectFromScope();
            if ($scope.onAssignmentDefinitionAdded != undefined)
            {
                $scope.onAssignmentDefinitionAdded(assignmentDefinition);
                $scope.modalContext.closeModal();
            }
        }
        function updateAssignmnetDefinition() {
            var assignmentDefinition = buildObjectFromScope();
            if ($scope.onAssignmentDefinitionUpdated != undefined) {
                $scope.onAssignmentDefinitionUpdated(assignmentDefinition);
                $scope.modalContext.closeModal();
            }
        }
    }
    appControllers.controller("VR_AccountManager_AssignmentDefinitionEditorController", assignmentDefinitionController);
})(appControllers);
