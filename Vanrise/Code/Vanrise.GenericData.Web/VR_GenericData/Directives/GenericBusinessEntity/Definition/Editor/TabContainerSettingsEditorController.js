(function (appControllers) {

    "use strict";

    TabContainerSettingsEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function TabContainerSettingsEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {
        var tabEntitySettings;
        var context;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                tabEntitySettings = parameters.tabEntitySettings;
                context = parameters.context;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.saveTabContainerSettings = function () {
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
                $scope.title = UtilsService.buildTitleForUpdateEditor(tabEntitySettings.TabTitle, 'Tab Settings');
            }

            function loadStaticData() {
                if (tabEntitySettings != undefined) {
                    $scope.scopeModel.showTab = tabEntitySettings.ShowTab;
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

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};

            return currentContext;
        }

        function saveSettings() {
            var tabSettingsDefinition = buildObjectFromScope();
            if ($scope.onTabSettingsChanged != undefined) {
                $scope.onTabSettingsChanged(tabSettingsDefinition);
            }
            $scope.modalContext.closeModal();
        }

        function buildObjectFromScope() {
            return {
                ShowTab: $scope.scopeModel.showTab,
            };
        }
    }

    appControllers.controller('VR_GenericData_TabContainerSettingsEditorController', TabContainerSettingsEditorController);
})(appControllers);
