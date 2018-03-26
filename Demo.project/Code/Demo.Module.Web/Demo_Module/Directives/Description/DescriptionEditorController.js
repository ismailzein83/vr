(function (appControllers) {

    "use strict";

    descriptionEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function descriptionEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var descriptionEntity;
        var isEditMode;
        var context;

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                descriptionEntity = parameters.descriptionEntity;
                context = parameters.context;
            }
            isEditMode = (descriptionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
           
            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return save();
                }
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

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && descriptionEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(descriptionEntity.Title, 'Description');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Description');
        }

        function loadStaticData() {
            if (descriptionEntity != undefined) {
                $scope.scopeModel.descriptionTitle = descriptionEntity.Title;
            }
        }

        function buildDescriptionObjFromScope() {
            return {
                DescriptionId: descriptionEntity != undefined ? descriptionEntity.DescriptionId : UtilsService.guid(),
                Title: $scope.scopeModel.descriptionTitle
            };
        }

        function save() {
            var descriptionObject = buildDescriptionObjFromScope();
            if ($scope.onDescriptionAdded != undefined)
                $scope.onDescriptionAdded(descriptionObject);
            $scope.modalContext.closeModal();
        }

        function update() {
            var descriptionObject = buildDescriptionObjFromScope();
            if ($scope.onDescriptionUpdated != undefined)
                $scope.onDescriptionUpdated(descriptionObject);
            $scope.modalContext.closeModal();
        }

    }

    appControllers.controller('Demo_Module_DescriptionEditorController', descriptionEditorController);
})(appControllers);
