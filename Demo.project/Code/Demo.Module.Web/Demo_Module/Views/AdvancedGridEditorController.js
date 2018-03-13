(function (appControllers) {
    
    "use strict";
    advancedGridEditorController.$inject = ["$scope", "VRNavigationService", "UtilsService", "VRNotificationService", "VRUIUtilsService"];

    function advancedGridEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var gridItemEntity;

        var isEditMode;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                gridItemEntity = parameters.gridItemEntity
            }
            isEditMode = (gridItemEntity != undefined);
        };
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateGridItem() : addGridItem();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function buildGridObjFromScope() {

                return {
                    Id: UtilsService.guid(),
                    Name: $scope.scopeModel.name
                };
            };

            function addGridItem(){
                var gridObject = buildGridObjFromScope();
                console.log("addGridItem");
                console.log(gridObject);
                if($scope.onGridItemAdded!=undefined)
                    $scope.onGridItemAdded(gridObject);
                $scope.modalContext.closeModal();
            };
            function updateGridItem(){
                var gridObject = buildGridObjFromScope();
                if($scope.onGridItemUpdated!=undefined)
                    $scope.onGridItemUpdated(gridObject);
                $scope.modalContext.closeModal();
            };
        };
        
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        };

        function loadAllControls() {
            function setTitle() {
                if (isEditMode && gridItemEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(gridItemEntity.Name, " Advanced Item");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Advanced Item");
            };
            function loadStaticData() {
                if (gridItemEntity != undefined)
                    $scope.scopeModel.name = gridItemEntity.Name;
            };
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        };
    }
    app.controller("Demo_Module_AdvancedGridEditorController", advancedGridEditorController);
})(appControllers);
