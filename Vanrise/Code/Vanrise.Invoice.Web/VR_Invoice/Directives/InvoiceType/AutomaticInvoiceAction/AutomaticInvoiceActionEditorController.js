(function (appControllers) {

    'use strict';

    AutomaticInvoiceActionEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function AutomaticInvoiceActionEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var automaticInvoiceActionEntity;

        var isEditMode;

        var automaticInvoiceActionSettingsAPI;
        var automaticInvoiceActionSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                automaticInvoiceActionEntity = parameters.automaticInvoiceActionEntity;
            }
            isEditMode = (automaticInvoiceActionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onAutomaticInvoiceActionSettingsReady = function (api) {
                automaticInvoiceActionSettingsAPI = api;
                automaticInvoiceActionSettingsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateAutomaticInvoiceAction() : addAutomaticInvoiceAction();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function builAutomaticInvoiceActionObjFromScope() {
                return {
                    Title: $scope.scopeModel.actionTitle,
                    AutomaticInvoiceActionId: automaticInvoiceActionEntity != undefined ? automaticInvoiceActionEntity.AutomaticInvoiceActionId : UtilsService.guid(),
                    Settings: automaticInvoiceActionSettingsAPI.getData(),
                };
            }
            function addAutomaticInvoiceAction() {
                var automaticInvoiceActionObj = builAutomaticInvoiceActionObjFromScope();
                if ($scope.onAutomaticInvoiceActionAdded != undefined) {
                    $scope.onAutomaticInvoiceActionAdded(automaticInvoiceActionObj);
                }
                $scope.modalContext.closeModal();
            }
            function updateAutomaticInvoiceAction() {
                var automaticInvoiceActionObj = builAutomaticInvoiceActionObjFromScope();
                if ($scope.onAutomaticInvoiceActionUpdated != undefined) {
                    $scope.onAutomaticInvoiceActionUpdated(automaticInvoiceActionObj);
                }
                $scope.modalContext.closeModal();
            }

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            function setTitle() {
                if (isEditMode && automaticInvoiceActionEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(automaticInvoiceActionEntity.Title, 'Automatic Invoice Action');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Automatic Invoice Action');
            }
            function loadStaticData() {
                if (automaticInvoiceActionEntity != undefined) {
                    $scope.scopeModel.actionTitle = automaticInvoiceActionEntity.Title;
                }
            }
            function loadAutomaticInvoiceActionSettingsDirective() {
                var automaticInvoiceActionSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                automaticInvoiceActionSettingsReadyPromiseDeferred.promise.then(function () {
                    var automaticInvoiceActionPayload = { context: getContext() };
                    if (automaticInvoiceActionEntity != undefined) {
                        automaticInvoiceActionPayload.automaticInvoiceActionEntity = automaticInvoiceActionEntity.Settings;
                    }
                    VRUIUtilsService.callDirectiveLoad(automaticInvoiceActionSettingsAPI, automaticInvoiceActionPayload, automaticInvoiceActionSettingsLoadPromiseDeferred);
                });
                return automaticInvoiceActionSettingsLoadPromiseDeferred.promise;
            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAutomaticInvoiceActionSettingsDirective]).then(function () {

            }).finally(function () {
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

    }
    appControllers.controller('VR_Invoice_AutomaticInvoiceActionEditorController', AutomaticInvoiceActionEditorController);

})(appControllers);