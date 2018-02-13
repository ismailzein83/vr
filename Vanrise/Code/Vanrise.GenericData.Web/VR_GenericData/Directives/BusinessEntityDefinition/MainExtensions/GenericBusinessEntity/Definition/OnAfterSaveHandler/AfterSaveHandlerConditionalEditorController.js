(function (appControllers) {

    "use strict";

    AfterSaveHandlerConditionalEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function AfterSaveHandlerConditionalEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var conditionalHandler;
        var context;



        var afterSaveHandlerEditorAPI;
        var afterSaveHandlerEditorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saveConditionEditorAPI;
        var saveConditionEditorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                conditionalHandler = parameters.conditionalHandler;
                context = parameters.context;
            }
            isEditMode = (conditionalHandler != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGenericBEAfterSaveHandlerSettingsReady = function (api) {
                afterSaveHandlerEditorAPI = api;
                afterSaveHandlerEditorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onGenericBESaveConditionSettingsReady = function (api) {
                saveConditionEditorAPI = api;
                saveConditionEditorReadyPromiseDeferred.resolve();
            };



            $scope.scopeModel.saveConditionalHandler = function () {
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
                    if (isEditMode && conditionalHandler != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(conditionalHandler.Name, 'Conditional Handler Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Conditional Handler Editor');
                }

                function loadStaticData() {
                    if (!isEditMode)
                        return;

                    $scope.scopeModel.name = conditionalHandler.Name;
                }

                function loadAfterSaveHandlerEditorSettings() {
                    var loadAfterSaveHandlerSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
                    afterSaveHandlerEditorReadyPromiseDeferred.promise.then(function () {
                        var editorHandlerPayload = {
                            settings: conditionalHandler != undefined && conditionalHandler.Handler || undefined,
                            context:getContext()
                        };

                        VRUIUtilsService.callDirectiveLoad(afterSaveHandlerEditorAPI, editorHandlerPayload, loadAfterSaveHandlerSettingsPromiseDeferred);
                    });
                    return loadAfterSaveHandlerSettingsPromiseDeferred.promise;
                }


                function loadSaveConditionHandlerEditorSettings() {
                    var loadSaveConditionSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
                    saveConditionEditorReadyPromiseDeferred.promise.then(function () {
                        var saveConditionPayload = {
                            settings: conditionalHandler != undefined && conditionalHandler.Condition || undefined,
                            context: getContext()
                        };

                        VRUIUtilsService.callDirectiveLoad(saveConditionEditorAPI, saveConditionPayload, loadSaveConditionSettingsPromiseDeferred);
                    });
                    return loadSaveConditionSettingsPromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadAfterSaveHandlerEditorSettings, loadSaveConditionHandlerEditorSettings]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }

        }

        function buildConditionalHandlerFromScope() {
            return {
                ConditionalAfterSaveHandlerItemId: conditionalHandler != undefined ? conditionalHandler.ConditionalAfterSaveHandlerItemId : UtilsService.guid(),
                Name: $scope.scopeModel.name,
                Handler: afterSaveHandlerEditorAPI.getData(),
                Condition: saveConditionEditorAPI.getData()
            };
        }

        function insert() {
            var conditionalHandler = buildConditionalHandlerFromScope();
            if ($scope.onConditionalHandlerAdded != undefined) {
                $scope.onConditionalHandlerAdded(conditionalHandler);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            var conditionalHandler = buildConditionalHandlerFromScope();
            if ($scope.onConditionalHandlerUpdated != undefined) {
                $scope.onConditionalHandlerUpdated(conditionalHandler);
            }
            $scope.modalContext.closeModal();
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
    }

    appControllers.controller('VR_GenericData_AfterSaveHandlerConditionalEditorController', AfterSaveHandlerConditionalEditorController);
})(appControllers);
