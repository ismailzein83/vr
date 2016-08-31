(function (appControllers) {

    'use strict';

    RDLCParameterEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function RDLCParameterEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var parameters = [];
        var parameterEntity;

        var isEditMode;

        var parameterSettingsAPI;
        var parameterSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parametersObj = VRNavigationService.getParameters($scope);
            if (parametersObj != undefined) {
                parameters = parametersObj.parameters;
                parameterEntity = parametersObj.parameterEntity;
            }
            isEditMode = (parameterEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onParameterSettingsReady = function (api) {
                parameterSettingsAPI = api;
                parameterSettingsReadyPromiseDeferred.resolve();
            }

            $scope.scopeModel.isVisible = true;

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateParameter() : addParameter();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadParameterSettingsDirective]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
        }

        function setTitle() {
            if (isEditMode && parameterEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(parameterEntity.ParameterName, 'Parameter');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Parameter');
        }

        function loadStaticData() {
            if (parameterEntity != undefined) {
                $scope.scopeModel.parameterName = parameterEntity.ParameterName;
                $scope.scopeModel.isVisible = parameterEntity.IsVisible;
            }
        }

        function loadParameterSettingsDirective() {
            var parameterSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            parameterSettingsReadyPromiseDeferred.promise.then(function () {
                var parameterPayload = parameterEntity != undefined ? { parameterEntity: parameterEntity.Value } : undefined;
                VRUIUtilsService.callDirectiveLoad(parameterSettingsAPI, parameterPayload, parameterSettingsLoadPromiseDeferred);
            });
            return parameterSettingsLoadPromiseDeferred.promise;
        }

        function builParameterObjFromScope() {
            return {
                ParameterName: $scope.scopeModel.parameterName,
                IsVisible: $scope.scopeModel.isVisible,
                Value: parameterSettingsAPI.getData()
            };
        }

        function addParameter() {
            var parameterObj = builParameterObjFromScope();
            if ($scope.onParameterAdded != undefined) {
                $scope.onParameterAdded(parameterObj);
            }
            $scope.modalContext.closeModal();
        }

        function updateParameter() {
            var parameterObj = builParameterObjFromScope();
            if ($scope.onParameterUpdated != undefined) {
                $scope.onParameterUpdated(parameterObj);
            }
            $scope.modalContext.closeModal();
        }

    }
    appControllers.controller('VR_Invoice_RDLCParameterEditorController', RDLCParameterEditorController);

})(appControllers);