(function (appControllers) {

    'use strict';

    AutomaticInvoiceActionEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function AutomaticInvoiceActionEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var automaticInvoiceActionEntity;

        var isEditMode;

        var automaticInvoiceActionSettingsAPI;
        var automaticInvoiceActionSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var localizationTextResourceSelectorAPI;
        var localizationTextResourceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
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

            $scope.scopeModel.onLocalizationTextResourceSelectorReady = function (api) {
                localizationTextResourceSelectorAPI = api;
                localizationTextResourceSelectorReadyPromiseDeferred.resolve();
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
                    TitleResourceKey: localizationTextResourceSelectorAPI != undefined ? localizationTextResourceSelectorAPI.getSelectedValues() : undefined
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
            function loadLocalizationTextResourceSelector() {
                var localizationTextResourceSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {
                    var localizationTextResourcePayload = automaticInvoiceActionEntity != undefined ? { selectedValue: automaticInvoiceActionEntity.TitleResourceKey } : undefined;

                    VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, localizationTextResourcePayload, localizationTextResourceSelectorLoadPromiseDeferred);
                });
                return localizationTextResourceSelectorLoadPromiseDeferred.promise;
            }
            var promises = [setTitle, loadStaticData, loadAutomaticInvoiceActionSettingsDirective, loadLocalizationTextResourceSelector];
            return UtilsService.waitMultipleAsyncOperations(promises).then(function () {

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