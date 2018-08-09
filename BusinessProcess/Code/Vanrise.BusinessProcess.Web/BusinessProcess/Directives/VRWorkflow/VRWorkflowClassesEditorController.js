(function (appControllers) {

    'use strict';

    VRWorkflowClassEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'BusinessProcess_VRWorkflowService'];

    function VRWorkflowClassEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, BusinessProcess_VRWorkflowService) {

        var vrWorkflowClassEntity;
        var vrWorkflowClassNamespaces = []; //for validation
        var isEditMode;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                vrWorkflowClassEntity = parameters.vrWorkflowClassEntity;
                vrWorkflowClassNamespaces = parameters.vrWorkflowClassNamespaces;
            }

            isEditMode = (vrWorkflowClassEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.isClassNamespaceValid = function () {
                var classNamespace = ($scope.scopeModel.namespace != undefined) ? $scope.scopeModel.namespace.toLowerCase() : null;
                if (isEditMode && classNamespace == vrWorkflowClassEntity.Namespace.toLowerCase())
                    return null;

                if (UtilsService.contains(vrWorkflowClassNamespaces, classNamespace))
                    return 'Same namespace already exists';

                return null;
            };

            $scope.scopeModel.save = function () {
                return isEditMode ? updateVRWorkflowClass() : addVRWorkflowClass();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && vrWorkflowClassEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrWorkflowClassEntity.Namespace, 'Workflow Class');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Workflow Class');
            }

            function loadStaticData() {
                if (vrWorkflowClassEntity != undefined) {
                    $scope.scopeModel.namespace = vrWorkflowClassEntity.Namespace;
                    $scope.scopeModel.classCode = vrWorkflowClassEntity.Code;
                }
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).then(function () {

            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function addVRWorkflowClass() {
            var vrWorkflowClassObj = buildVRWorkflowClassObjFromScope();
            if ($scope.onVRWorkflowClassAdded != undefined) {
                $scope.onVRWorkflowClassAdded(vrWorkflowClassObj);
            }
            $scope.modalContext.closeModal();
        }

        function updateVRWorkflowClass() {
            var vrWorkflowClassObj = buildVRWorkflowClassObjFromScope();
            if ($scope.onVRWorkflowClassUpdated != undefined) {
                $scope.onVRWorkflowClassUpdated(vrWorkflowClassObj);
            }
            $scope.modalContext.closeModal();
        }

        function buildVRWorkflowClassObjFromScope() {
            return {
                VRWorkflowClassId: vrWorkflowClassEntity != undefined ? vrWorkflowClassEntity.VRWorkflowClassId : UtilsService.guid(),
                Namespace: $scope.scopeModel.namespace,
                Code : $scope.scopeModel.classCode
            };
        }
    }

    appControllers.controller('VR_Workflow_VRWorkflowClassEditorController', VRWorkflowClassEditorController);
})(appControllers);