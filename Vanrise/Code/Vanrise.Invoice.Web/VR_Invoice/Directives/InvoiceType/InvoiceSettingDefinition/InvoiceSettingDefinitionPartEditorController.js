(function (appControllers) {

    "use strict";

    InvoiceSettingDefinitionPartEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VR_Invoice_InvoiceTypeConfigsAPIService'];

    function InvoiceSettingDefinitionPartEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VR_Invoice_InvoiceTypeConfigsAPIService) {

        var isEditMode;
        var rowEntity;
        $scope.scopeModel = {};

        var directiveAPI;
        var directiveReadyDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                rowEntity = parameters.rowEntity;
            }

            isEditMode = (rowEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel.SaveRow = function () {
                if (isEditMode) {
                    return updateRow();
                }
                else {
                    return insertRow();
                }
            };

            $scope.scopeModel.templateConfigs = [];

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                var directivePayload;
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
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

            function setTitle() {
                if (isEditMode && rowEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor('Part');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Part');
            }

            function loadDirective() {
                if(rowEntity != undefined && rowEntity.PartDefinitionSetting != undefined)
                {
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();
                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                    directiveReadyDeferred.promise.then(function () {
                        directiveReadyDeferred = rowEntity.PartDefinitionSetting;
                        var directivePayload;
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                    });
                    return directiveLoadDeferred.promise;
                }
            }

            function getInvoiceSettingDefinitionTemplateConfigs() {
                return VR_Invoice_InvoiceTypeConfigsAPIService.GetInvoiceSettingPartsConfigs().then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            $scope.scopeModel.templateConfigs.push(response[i]);
                        }
                        if (rowEntity != undefined) {
                            $scope.scopeModel.selectedTemplateConfig =
                                UtilsService.getItemByVal($scope.scopeModel.templateConfigs, rowEntity.PartConfigId, 'ExtensionConfigurationId');
                        }
                    }
                });
            }

            return UtilsService.waitMultipleAsyncOperations([loadDirective, setTitle, getInvoiceSettingDefinitionTemplateConfigs])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function buildRowObjectFromScope() {
            var rowObject = {
                PartConfigId: $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId,
                PartDefinitionSetting: directiveAPI != undefined? directiveAPI.getData():undefined
            };
            return rowObject;
        }

        function insertRow() {

            var rowObject = buildRowObjectFromScope();
            if ($scope.onRowAdded != undefined)
                $scope.onRowAdded(rowObject);
            $scope.modalContext.closeModal();
        }

        function updateRow() {
            var rowObject = buildRowObjectFromScope();
            if ($scope.onRowUpdated != undefined)
                $scope.onRowUpdated(rowObject);
            $scope.modalContext.closeModal();
        }

    }

    appControllers.controller('VR_Invoice_InvoiceSettingDefinitionPartEditorController', InvoiceSettingDefinitionPartEditorController);
})(appControllers);