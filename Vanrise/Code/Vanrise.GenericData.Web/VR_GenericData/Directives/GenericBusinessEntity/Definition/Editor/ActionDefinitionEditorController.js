(function (appControllers) {

    "use strict";

    GenericBEActionDefintionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function GenericBEActionDefintionController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var actionDefinition;
        var context;


        var actionSettingsDirectiveAPI;
        var actionSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                actionDefinition = parameters.actionDefinition;
            }
            isEditMode = (actionDefinition != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onActionDefinitionSettingDirectiveReady = function (api) {
                actionSettingsDirectiveAPI = api;
                actionSettingsReadyPromiseDeferred.resolve();
            };



            $scope.scopeModel.saveActionDefinition = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }
        function load() {

            loadAllControls();

            function loadAllControls() {
                $scope.scopeModel.isLoading = true;
                function setTitle() {
                    if (isEditMode && actionDefinition != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(actionDefinition.Name, 'Action Definition Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Action Definition Editor');
                }

                function loadStaticData() {
                    if (!isEditMode)
                        return;

                    $scope.scopeModel.name = actionDefinition.Name;
                }

                function loadSettingDirectiveSection() {
                    var loadActionSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
                    actionSettingsReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            settings: actionDefinition != undefined && actionDefinition.Settings != undefined ? actionDefinition.Settings : undefined
                        };

                        VRUIUtilsService.callDirectiveLoad(actionSettingsDirectiveAPI, payload, loadActionSettingsPromiseDeferred);
                    });
                    return loadActionSettingsPromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadSettingDirectiveSection]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }

        }

        function buildActionDefinitionFromScope() {
            return {
                GenericBEActionId: actionDefinition != undefined ? actionDefinition.GenericBEActionId : UtilsService.guid(),
                Name: $scope.scopeModel.name,
                Settings: actionSettingsDirectiveAPI.getData()
            };
        }

        function insert() {
            var actionDefinition = buildActionDefinitionFromScope();
            if ($scope.onGenericBEActionDefinitionAdded != undefined) {
                $scope.onGenericBEActionDefinitionAdded(actionDefinition);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            var actionDefinition = buildActionDefinitionFromScope();
            if ($scope.onGenericBEActionDefinitionUpdated != undefined) {
                $scope.onGenericBEActionDefinitionUpdated(actionDefinition);
            }
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('VR_GenericData_GenericBEActionDefintionController', GenericBEActionDefintionController);
})(appControllers);
