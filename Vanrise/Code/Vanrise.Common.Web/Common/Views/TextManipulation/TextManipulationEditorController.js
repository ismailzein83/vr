(function (appControllers) {

    "use strict";

    textManipulationEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function textManipulationEditorController($scope,  VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var textManipulationSettingsAPI;
        var textManipulationSettingsReadyDeferred = UtilsService.createPromiseDeferred();
        var textManipulationSettings;

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                textManipulationSettings = parameters.textManipulationSettings;
            }
        }
        function defineScope() {

            $scope.onTextManipulationSettingsDirectiveReady = function (api) {
                textManipulationSettingsAPI = api;
                textManipulationSettingsReadyDeferred.resolve();
            };

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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadTextManipulationSettings])
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

        function loadTextManipulationSettings() {
            var loadTextManipulationSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
            textManipulationSettingsReadyDeferred.promise.then(function () {
                var payLoad = { isNotRequired: true };
                if (textManipulationSettings != undefined)
                {
                    payLoad.textManipulationSettings = textManipulationSettings;
                }
                VRUIUtilsService.callDirectiveLoad(textManipulationSettingsAPI, payLoad, loadTextManipulationSettingsPromiseDeferred);
            });
            return loadTextManipulationSettingsPromiseDeferred.promise;
        }

        function buildTextManipulationObjFromScope() {
            var obj = {
                textManipulationSettings: textManipulationSettingsAPI.getData()
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
