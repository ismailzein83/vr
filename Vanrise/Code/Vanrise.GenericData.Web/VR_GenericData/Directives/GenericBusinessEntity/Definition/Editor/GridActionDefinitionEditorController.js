(function (appControllers) {

    "use strict";

    GenericBEGridActionDefintionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function GenericBEGridActionDefintionController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var gridActionDefinition;
        var context;

        var actionGroupSelectorAPI;
        var actionGroupSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var filterConditionAPI;
        var filterConditionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                gridActionDefinition = parameters.gridActionDefinition;
                context = parameters.context;
            }
            isEditMode = (gridActionDefinition != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.bEActionsDefinitionInfos = context.getActionInfos();

            $scope.scopeModel.onGenericBEGridActionGroupSelectorReady = function (api) {
                actionGroupSelectorAPI = api;
                actionGroupSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onGenericBEGridactiondefinitionFilterReady = function (api) {
                filterConditionAPI = api;
                filterConditionReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.saveGridActionDefinition = function () {
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
                    if (isEditMode && gridActionDefinition != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(gridActionDefinition.Title, 'Grid Action Definition Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Grid Action Definition Editor');
                }

                function loadStaticData() {
                    if (!isEditMode)
                        return;

                    $scope.scopeModel.title = gridActionDefinition.Title;
                    $scope.scopeModel.reloadGridItem = gridActionDefinition.ReloadGridItem;
                    $scope.scopeModel.selectedActionDefnition = UtilsService.getItemByVal($scope.scopeModel.bEActionsDefinitionInfos, gridActionDefinition.GenericBEActionId, "GenericBEActionId");

                }

                function loadActionGroupSelector() {
                    var loadActionGroupSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                    actionGroupSelectorReadyPromiseDeferred.promise.then(function () {
                        var actionGroupPayload = {
                            context: getContext(),
                            selectedIds: gridActionDefinition != undefined && gridActionDefinition.GenericBEGridActionGroupId != undefined ? gridActionDefinition.GenericBEGridActionGroupId : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(actionGroupSelectorAPI, actionGroupPayload, loadActionGroupSelectorPromiseDeferred);
                    });
                    return loadActionGroupSelectorPromiseDeferred.promise;
                }


                function loadFilterConditionEditor() {
                    var loadFilterConditionEditorPromiseDeferred = UtilsService.createPromiseDeferred();
                    filterConditionReadyPromiseDeferred.promise.then(function () {
                        var settingPayload = {
                            context: getContext(),
                            filterCondition: gridActionDefinition != undefined ? gridActionDefinition.FilterCondition : undefined
                        };

                        VRUIUtilsService.callDirectiveLoad(filterConditionAPI, settingPayload, loadFilterConditionEditorPromiseDeferred);
                    });
                    return loadFilterConditionEditorPromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadActionGroupSelector, loadFilterConditionEditor]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }

        }

        function buildGridActionDefinitionFromScope() {
            return {
                GenericBEGridActionId: gridActionDefinition != undefined ? gridActionDefinition.GenericBEGridActionId : UtilsService.guid(),
                GenericBEActionId: $scope.scopeModel.selectedActionDefnition != undefined ? $scope.scopeModel.selectedActionDefnition.GenericBEActionId : undefined,
                Title: $scope.scopeModel.title,
                ReloadGridItem: $scope.scopeModel.reloadGridItem,
                GenericBEGridActionGroupId: actionGroupSelectorAPI.getSelectedIds(),
                FilterCondition: filterConditionAPI.getData()
            };
        }

        function insert() {
            var gridActionObj = buildGridActionDefinitionFromScope();
            if ($scope.onGenericBEGridActionDefinitionAdded != undefined) {
                $scope.onGenericBEGridActionDefinitionAdded(gridActionObj);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            var gridActionObj = buildGridActionDefinitionFromScope();
            if ($scope.onGenericBEGridActionDefinitionUpdated != undefined) {
                $scope.onGenericBEGridActionDefinitionUpdated(gridActionObj);
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

    appControllers.controller('VR_GenericData_GenericBEGridActionDefintionController', GenericBEGridActionDefintionController);
})(appControllers);
