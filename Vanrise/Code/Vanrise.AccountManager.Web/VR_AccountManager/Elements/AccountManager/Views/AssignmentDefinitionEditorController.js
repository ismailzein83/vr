(function (appControllers) {

    "use strict";

    assignmentDefinitionController.$inject = ["$scope", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService"];

    function assignmentDefinitionController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {
        var isEditMode;
        var AssignementDefinitionEntity;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                AssignementDefinitionEntity = parameters;
            };
            isEditMode = (AssignementDefinitionEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.assignmentName = accountManagerAssignementDefinitionName;
            $scope.scopeModel.save = function () {
                if (!isEditMode) {
                    addAssignmentDefinition();
                }
                else {
                    updateAssignmnetDefinition();
                }
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
                      $scope.title = UtilsService.buildTitleForUpdateEditor('Assignment Definition');

                }
                return UtilsService.waitMultipleAsyncOperations([setTitle]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }
        function buildObjectFromScope() {
            if (!isEditMode) {
                var assignmentDefiniton = {
                    AccountManagerAssignementDefinitionId: UtilsService.guid(),
                    Name: $scope.scopeModel.assignmentName
                };
            }
            else {
                var assignmentDefiniton = {
                    AccountManagerAssignementDefinitionId: AssignementDefinitionEntity.AccountManagerAssignementDefinitionId,
                    Name: $scope.scopeModel.assignmentName
                };
            }
            return assignmentDefiniton;
        }
        function addAssignmentDefinition() {
            var assignmentDefinition = buildObjectFromScope();
            $scope.onAssignmentDefinitionAdded(assignmentDefinition);
            $scope.modalContext.closeModal();
        }
        function updateAssignmnetDefinition() {
            var assignmentDefinition = buildObjectFromScope();
            $scope.onAssignmentDefinitionUpdated(assignmentDefinition);
            $scope.modalContext.closeModal();
        }
    }
    appControllers.controller("VR_AccountManager_AssignmentDefinitionEditorController", assignmentDefinitionController);
})(appControllers);
