(function (appControllers) {

    "use strict";

    GenericBEGridActionDefintionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function GenericBEGridActionDefintionController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var gridActionDefinition;
        var context;
        
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


                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle]).then(function () {
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
                FilterCondition: null
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
    }

    appControllers.controller('VR_GenericData_GenericBEGridActionDefintionController', GenericBEGridActionDefintionController);
})(appControllers);
