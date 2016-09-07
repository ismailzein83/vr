(function (appControllers) {

    'use strict';

    concatenatedPartEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function concatenatedPartEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var concatenatedPartEntity;

        var isEditMode;
        var concatenatedPartSettingsAPI;
        var concatenatedPartSettingsReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                concatenatedPartEntity = parameters.concatenatedPartEntity;
            }
            isEditMode = (concatenatedPartEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onPartSettingsReady = function (api) {
                concatenatedPartSettingsAPI = api;
                concatenatedPartSettingsReadyDeferred.resolve();
            }
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateConcatenatedPart() : addeConcatenatedPart();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadConcatenatedPartSettingsDirective]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
        }
        function loadConcatenatedPartSettingsDirective() {
            var concatenatedPartSettingsLoadDeferred = UtilsService.createPromiseDeferred();
            concatenatedPartSettingsReadyDeferred.promise.then(function () {
                var concatenatedPartSettingsPayload = {context:getContext()};
                if (concatenatedPartEntity != undefined)
                    concatenatedPartSettingsPayload.concatenatedPartSettings = concatenatedPartEntity.Settings;
                VRUIUtilsService.callDirectiveLoad(concatenatedPartSettingsAPI, concatenatedPartSettingsPayload, concatenatedPartSettingsLoadDeferred);
            });
            return concatenatedPartSettingsLoadDeferred.promise;
        }
        function setTitle() {
            if (isEditMode && concatenatedPartEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(concatenatedPartEntity.PartTitle, 'Concatenated Part');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Concatenated Part');
        }
        function loadStaticData() {
            if (concatenatedPartEntity != undefined) {
                $scope.scopeModel.partTitle = concatenatedPartEntity.PartTitle;
            }
        }
        function getContext()
        {
            return context;
        }
        function builConcatenatedPartObjFromScope() {
            return {
                PartTitle: $scope.scopeModel.partTitle,
                Settings: concatenatedPartSettingsAPI.getData()
            };
        }
        function addeConcatenatedPart() {
            var concatenatedPartObj = builConcatenatedPartObjFromScope();
            if ($scope.onConcatenatedPartAdded != undefined) {
                $scope.onConcatenatedPartAdded(concatenatedPartObj);
            }
            $scope.modalContext.closeModal();
        }
        function updateConcatenatedPart() {
            var concatenatedPartObj = builConcatenatedPartObjFromScope();
            if ($scope.onConcatenatedPartUpdated != undefined) {
                $scope.onConcatenatedPartUpdated(concatenatedPartObj);
            }
            $scope.modalContext.closeModal();
        }

    }
    appControllers.controller('VR_Invoice_ConcatenatedPartEditorController', concatenatedPartEditorController);

})(appControllers);