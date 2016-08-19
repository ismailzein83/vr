(function (appControllers) {

    'use strict';

    subSectionEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService'];

    function subSectionEditorController($scope, VRNavigationService, UtilsService, VRNotificationService) {

        var subSections = [];
        var subSectionEntity;

        var isEditMode;
        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                subSections = parameters.subSections;
                subSectionEntity = parameters.subSectionEntity;
            }
            isEditMode = (subSectionEntity != undefined);
        }

        function defineScope() {
           
            $scope.save = function () {
                return (isEditMode) ? updateSubSection() : addeSubSection();
            };
            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {

            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).then(function () {

            }).finally(function () {
                $scope.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
        }
        function setTitle() {
            if (isEditMode && subSectionEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(subSectionEntity.SectionTitle, 'Sub Section');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Sub Section');
        }

        function loadStaticData() {
            if (subSectionEntity != undefined) {
                $scope.sectionTitle = subSectionEntity.SectionTitle;
                $scope.directive = subSectionEntity.Directive;
            }
        }

        function builSubSectionObjFromScope() {
            return {
                SectionTitle: $scope.sectionTitle,
                Directive: $scope.directive,
            };
        }

        function addeSubSection() {
            var subSectionObj = builSubSectionObjFromScope();
            if ($scope.onSubSectionAdded != undefined) {
                $scope.onSubSectionAdded(subSectionObj);
            }
            $scope.modalContext.closeModal();
        }

        function updateSubSection() {
            var subSectionObj = builSubSectionObjFromScope();
            if ($scope.onSubSectionUpdated != undefined) {
                $scope.onSubSectionUpdated(subSectionObj);
            }
            $scope.modalContext.closeModal();
        }

    }
    appControllers.controller('VR_Invoice_SubSectionEditorController', subSectionEditorController);

})(appControllers);