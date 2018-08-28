(function (appControllers) {

    "use strict";

    GenericBEViewDefintionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function GenericBEViewDefintionController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var viewDefinition;
        var context;

        var dataRecordTypeFieldsSelectorAPI;
        var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var viewSettingsDirectiveAPI;
        var viewSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                viewDefinition = parameters.viewDefinition;
                context = parameters.context;
            }
            isEditMode = (viewDefinition != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};



            $scope.scopeModel.onViewDefinitionSettingDirectiveReady = function (api) {
                viewSettingsDirectiveAPI = api;
                viewSettingsReadyPromiseDeferred.resolve();
            };



            $scope.scopeModel.saveViewDefinition = function () {
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
                    if (isEditMode && viewDefinition != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(viewDefinition.Name, 'View Definition Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('View Definition Editor');
                }

                function loadStaticData() {
                    if (!isEditMode)
                        return;

                    $scope.scopeModel.name = viewDefinition.Name;
                }

                function loadSettingDirectiveSection() {
                    var loadViewSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
                    viewSettingsReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            parameterEntity: viewDefinition != undefined && viewDefinition.Settings != undefined ? viewDefinition.Settings : undefined,
                            context:context
                        };

                        VRUIUtilsService.callDirectiveLoad(viewSettingsDirectiveAPI, payload, loadViewSettingsPromiseDeferred);
                    });
                    return loadViewSettingsPromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadSettingDirectiveSection]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }

        }

        function buildViewDefinitionFromScope() {
            return {
                GenericBEViewDefinitionId: viewDefinition != undefined ? viewDefinition.GenericBEViewDefinitionId : UtilsService.guid(),
                Name: $scope.scopeModel.name,
                Settings: viewSettingsDirectiveAPI.getData()
            };
        }

        function insert() {
            var viewDefinition = buildViewDefinitionFromScope();
            if ($scope.onGenericBEViewDefinitionAdded != undefined) {
                $scope.onGenericBEViewDefinitionAdded(viewDefinition);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            var viewDefinition = buildViewDefinitionFromScope();
            if ($scope.onGenericBEViewDefinitionUpdated != undefined) {
                $scope.onGenericBEViewDefinitionUpdated(viewDefinition);
            }
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('VR_GenericData_GenericBEViewDefintionController', GenericBEViewDefintionController);
})(appControllers);
