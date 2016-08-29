(function (appControllers) {
    "use strict";

    function beReceiveDefinitionEditorController($scope, utilsService, vrNotificationService, vrNavigationService, beRecieveDefinitionApiService) {

        var isEditMode;
        var recevieDefinitionId;
        var receveiveDEfinitionEntity;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = vrNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                recevieDefinitionId = parameters.BEReceiveDefinitionId;
            }
            isEditMode = (recevieDefinitionId != undefined);
        }
        function setTitle() {
            if (isEditMode) {
                var receiveDefinitionName = (receveiveDEfinitionEntity != undefined) ? receveiveDEfinitionEntity.Name : null;
                $scope.title = utilsService.buildTitleForUpdateEditor(receiveDefinitionName, 'ReceiveDefinition');

            }
            else {
                $scope.title = utilsService.buildTitleForAddEditor('ReceiveDefinition');
            }
        }
        function loadAllControls() {
            return utilsService.waitMultipleAsyncOperations([setTitle]).catch(function (error) {
                vrNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function getReceiveDefinition() {
            return beRecieveDefinitionApiService.GetReceiveDefinition(recevieDefinitionId).then(function (response) {
                receveiveDEfinitionEntity = response;
            });
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getReceiveDefinition().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    vrNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.save = function () {

            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
    }

    beReceiveDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_BEBridge_BERecieveDefinitionAPIService'];
    appControllers.controller('VR_BEBridge_BEReceiveDefinitionEditorController', beReceiveDefinitionEditorController);

})(appControllers);