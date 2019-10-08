(function (appControllers) {

    'use strict';

    actionTypeEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'VRUIUtilsService', 'UtilsService'];

    function actionTypeEditorController($scope, VRNavigationService, VRNotificationService, VRUIUtilsService, UtilsService) {

        var isEditMode;
        var context;
        var actionTypeEntity;
        var actionTypeTemplateConfig;

        var directiveAPI;
        var directiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                context = parameters.context;
                actionTypeEntity = parameters.actionTypeEntity;
                actionTypeTemplateConfig = parameters.actionTypeTemplateConfig;
            }

            isEditMode = (actionTypeEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                directiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return updateAction();
                else
                    return insertAction();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            loadAllControls().finally(function () {
                actionTypeEntity = undefined;
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && actionTypeEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(actionTypeEntity.ActionType, 'Action');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Action');
            }

            function loadStaticData() {
                $scope.scopeModel.actionTypeTemplateConfig = actionTypeTemplateConfig;
            }

            function loadDirective() {
                var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                directiveReadyDeferred.promise.then(function () {
                    var payload = {
                        context: context
                    };
                    if (actionTypeEntity != undefined) {
                        payload.actionType = actionTypeEntity;
                    }

                    VRUIUtilsService.callDirectiveLoad(directiveAPI, payload, directiveLoadPromiseDeferred);
                });

                return directiveLoadPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDirective]);
        }

        function insertAction() {
            var actionObject = buildActionObjectFromScope();

            if ($scope.onActionAdded != undefined) {
                $scope.onActionAdded(actionObject);

                $scope.modalContext.closeModal();
            }
        }

        function updateAction() {
            var actionObject = buildActionObjectFromScope();

            if ($scope.onActionUpdated != undefined) {
                $scope.onActionUpdated(actionObject);

                $scope.modalContext.closeModal();
            }
        }

        function buildActionObjectFromScope() {
            var actionObject;

            if (directiveAPI != undefined)
                actionObject = directiveAPI.getData();

            if (actionObject != undefined) {
                if (actionTypeTemplateConfig != undefined) {
                    actionObject.ConfigId = actionTypeTemplateConfig.ExtensionConfigurationId;
                    actionObject.ActionType = actionTypeTemplateConfig.Title;
                }

                actionObject.reload = false;

                if (directiveAPI != undefined && directiveAPI.reload != undefined && typeof (directiveAPI.reload) == 'function')
                    actionObject.isReloadImplemented = true;
                else
                    actionObject.isReloadImplemented = false;
            }
            return actionObject;
        }
    }

    appControllers.controller('VR_Analytic_ActionTypeEditorController', actionTypeEditorController);
})(appControllers);