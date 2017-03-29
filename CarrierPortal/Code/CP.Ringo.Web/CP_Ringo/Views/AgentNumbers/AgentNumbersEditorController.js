(function (appControllers) {

    "use strict";

    AgentNumbersEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'CP_Ringo_AgentNumbersAPIService'];

    function AgentNumbersEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, CP_Ringo_AgentNumbersAPIService) {
        var isEditMode;

        var agentNumbersId;
        var agentNumbersEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {

            }
        }
        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.numbers = [];

            $scope.scopeModel.save = function () {
                return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();

        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                $scope.title = UtilsService.buildTitleForAddEditor('Agent Numbers');
            }
            function loadStaticData() {
            }
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return CP_Ringo_AgentNumbersAPIService.AddAgentNumbersRequest(buildAgentNumbersObjFromScope()).then(function (response) {
                if ($scope.onAgentNumbersAdded != undefined)
                    $scope.onAgentNumbersAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildAgentNumbersObjFromScope() {
            var agentNumbersSettings = {};
            agentNumbersSettings.AgentNumbers = [];
            for (var i = 0; i < $scope.scopeModel.numbers.length; i++) {
                var number = $scope.scopeModel.numbers[i];
                agentNumbersSettings.AgentNumbers.push({
                    Number: number
                });
            }

            return {
                Settings: agentNumbersSettings
            };
        }
    }

    appControllers.controller('CP_Ringo_AgentNumbersEditorController', AgentNumbersEditorController);

})(appControllers);