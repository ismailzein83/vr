(function (appControllers) {

    'use strict';

    invoiceItemActionEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService','VRUIUtilsService'];

    function invoiceItemActionEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var actionEntity;

        var isEditMode;

        var gridActionSettingsAPI;
        var gridActionSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                actionEntity = parameters.actionEntity;
            }
            isEditMode = (actionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onGridActionSettingsReady = function (api)
            {
                gridActionSettingsAPI =api;
                gridActionSettingsReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateGridAction() : addGridAction();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData,loadGridActionSettingsDirective]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
        }
        function setTitle() {
            if (isEditMode && actionEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(actionEntity.ActionTypeName, 'Grid Action');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Grid Action');
        }
        function loadStaticData() {
            if (actionEntity != undefined) {
                $scope.scopeModel.actionTypeName = actionEntity.ActionTypeName;
            }
        }
        function loadGridActionSettingsDirective() {
            var gridActionSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            gridActionSettingsReadyPromiseDeferred.promise.then(function () {
                var invoiceGridActionPayload = { context: getContext() }
                if(actionEntity != undefined)
                {
                    invoiceGridActionPayload.invoiceGridActionEntity = actionEntity.Settings;
                }
                VRUIUtilsService.callDirectiveLoad(gridActionSettingsAPI, invoiceGridActionPayload, gridActionSettingsLoadPromiseDeferred);
            });
            return gridActionSettingsLoadPromiseDeferred.promise;
        }

        function builGridActionObjFromScope() {
            return {
                ActionTypeName: $scope.scopeModel.actionTypeName,
                Settings: gridActionSettingsAPI.getData()
            };
        }

        function getContext()
        {
           
            return context;
        }

        function addGridAction() {
            var gridActionObj = builGridActionObjFromScope();
            if ($scope.onGridActionAdded != undefined) {
                $scope.onGridActionAdded(gridActionObj);
            }
            $scope.modalContext.closeModal();
        }

        function updateGridAction() {
            var gridActionObj = builGridActionObjFromScope();
            if ($scope.onGridActionUpdated != undefined) {
                $scope.onGridActionUpdated(gridActionObj);
            }
            $scope.modalContext.closeModal();
        }

    }
    appControllers.controller('VR_Invoice_InvoiceItemActionEditorController', invoiceItemActionEditorController);

})(appControllers);