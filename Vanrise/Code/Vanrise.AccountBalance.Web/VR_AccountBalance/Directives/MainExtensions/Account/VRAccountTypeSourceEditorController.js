(function (appControllers) {

    'use strict';

    AccountTypeSourceEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function AccountTypeSourceEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var sourceEntity;

        var isEditMode;

        var sourceSettingsAPI;
        var sourceSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                sourceEntity = parameters.sourceEntity;
            }
            isEditMode = (sourceEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onSourceSettingsReady = function (api) {
                sourceSettingsAPI = api;
                sourceSettingsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSource() : addSource();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function builSourceObjFromScope() {
                return {
                    AccountBalanceFieldSourceId:sourceEntity != undefined?sourceEntity.AccountBalanceFieldSourceId: UtilsService.guid(),
                    Name: $scope.scopeModel.name,
                    Settings: sourceSettingsAPI.getData(),
                };
            }

            function addSource() {
                var sourceObj = builSourceObjFromScope();
                if ($scope.onSourceAdded != undefined) {
                    $scope.onSourceAdded(sourceObj);
                }
                $scope.modalContext.closeModal();
            }

            function updateSource() {
                var sourceObj = builSourceObjFromScope();
                if ($scope.onSourceUpdated != undefined) {
                    $scope.onSourceUpdated(sourceObj);
                }
                $scope.modalContext.closeModal();
            }
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
            function loadAllControls() {
                function setTitle() {
                    if (isEditMode && sourceEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(sourceEntity.Name, 'Source');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Source');
                }
                function loadStaticData() {
                    if (sourceEntity != undefined) {
                        $scope.scopeModel.name = sourceEntity.Name;
                    }
                }
                function loadSourceSettingsDirective() {
                    var sourceSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    sourceSettingsReadyPromiseDeferred.promise.then(function () {
                        var sourcePayload = { context: getContext() };
                        if (sourceEntity != undefined)
                            sourcePayload.sourceEntity = sourceEntity.Settings;
                        VRUIUtilsService.callDirectiveLoad(sourceSettingsAPI, sourcePayload, sourceSettingsLoadPromiseDeferred);
                    });
                    return sourceSettingsLoadPromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSourceSettingsDirective]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
            }
        }
        function getContext() {
            var currentContext = context;
            if (currentContext == undefined) {
                currentContext = {};
            }
            return currentContext;
        }

    }
    appControllers.controller('VR_AccountBalance_AccountTypeSourceEditorController', AccountTypeSourceEditorController);

})(appControllers);