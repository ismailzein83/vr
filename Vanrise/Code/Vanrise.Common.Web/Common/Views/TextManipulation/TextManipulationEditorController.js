(function (appControllers) {

    "use strict";

    textManipulationEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function textManipulationEditorController($scope,  VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
            }
        }
        function defineScope() {

            $scope.saveTextManipulation = function () {
                return saveTextManipulation();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = "Text Manipulation";
        }

        function loadStaticData() {
        }

        function buildTextManipulationObjFromScope() {
            var obj = {
            };
            return obj;
        }

        function saveTextManipulation() {
            var textManipulation = buildTextManipulationObjFromScope();
            if ($scope.onTextManipulationSave != undefined)
                $scope.onTextManipulationSave(textManipulation);
            $scope.modalContext.closeModal();
        }

    }

    appControllers.controller('VRCommon_TextManipulationEditorController', textManipulationEditorController);
})(appControllers);
