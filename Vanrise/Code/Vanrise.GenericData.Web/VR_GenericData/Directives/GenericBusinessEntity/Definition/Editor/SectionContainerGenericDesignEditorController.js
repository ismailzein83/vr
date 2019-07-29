(function (appControllers) {

    "use strict";

    SectionContainerGenericDesignEditorController.$inject = ['$scope', 'UtilsService','VRNavigationService', 'VRUIUtilsService'];

    function SectionContainerGenericDesignEditorController($scope, UtilsService, VRNavigationService, VRUIUtilsService) {

        var sectionEntity;
        var context;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                sectionEntity = parameters.sectionEntityObject;
                context = parameters.context;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.saveSectionSettings = function () {
                return saveSettings();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            var initialPromises = [];

            $scope.scopeModel.isLoading = true;
            function setTitle() {
                $scope.title = UtilsService.buildTitleForUpdateEditor(sectionEntity.entity.SectionTitle, 'Section Settings');
            }

            function loadStaticData() {
                if (sectionEntity != undefined) {
                    $scope.scopeModel.sectionWidth = sectionEntity.entity.ColNum;
                }
            }

            initialPromises.push(UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle]));

            var rootPromiseNode = {
                promises: initialPromises
            };

            return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function saveSettings() {
            var sectionSettingsDefinition = buildObjectFromScope();
            if ($scope.onSectionSettingsChanged != undefined) {
                $scope.onSectionSettingsChanged(sectionSettingsDefinition);
            }
            $scope.modalContext.closeModal();
        }

        function buildObjectFromScope() {
            return {
                ColNum: $scope.scopeModel.sectionWidth,
            };
        }
    }

    appControllers.controller('VR_GenericData_SectionContainerGenericDesignEditorController', SectionContainerGenericDesignEditorController);
})(appControllers);
