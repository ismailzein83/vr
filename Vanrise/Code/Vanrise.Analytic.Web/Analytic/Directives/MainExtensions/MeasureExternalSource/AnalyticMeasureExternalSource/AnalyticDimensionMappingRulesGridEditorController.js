(function (appControllers) {

    "use strict";
    DimensionMappingRulesGridEditorController.$inject = ["$scope", "VRNavigationService", "UtilsService", "VRNotificationService", "VRUIUtilsService"];

    function DimensionMappingRulesGridEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var ruleEntity;      
        var isEditMode;
        var tableId;

        var dimensionSelectiveAPI;
        var dimensionSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
               
                context = parameters.context;                
                ruleEntity = parameters.ruleEntity;
                tableId = parameters.tableId;
            }
            isEditMode = (ruleEntity != undefined);

        };
        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.onDimensionSelectiveReady = function (api) {
                dimensionSelectiveAPI = api;
                dimensionSelectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateRule() : addRule();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function buildRuleObjFromScope() {
               
                return {
                    Id: UtilsService.guid(),
                    Name: $scope.scopeModel.name,
                    Settings: dimensionSelectiveAPI.getData()                        

                };
            };

            function addRule() {
                var ruleObject = buildRuleObjFromScope();
                if ($scope.onRuleAdded != undefined)
                    $scope.onRuleAdded(ruleObject);
                $scope.modalContext.closeModal();
            };
            function updateRule() {
                var ruleObject = buildRuleObjFromScope();
                if ($scope.onRuleUpdated != undefined)
                    $scope.onRuleUpdated(ruleObject);
                $scope.modalContext.closeModal();
                
            };
        };

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        };

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && ruleEntity != undefined) {
                    var entity = ruleEntity.Entity;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(entity.Name, " Dimension Mapping Rule");
                }
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Dimension Mapping Rule");
            }

            function loadStaticData() {
                if (ruleEntity != undefined)
                    $scope.scopeModel.name = ruleEntity.Entity != undefined ? ruleEntity.Entity.Name : undefined;
            }

            function loadDimensionSelective() {

                var loadDimensionSelectiveDeferred = UtilsService.createPromiseDeferred();

                dimensionSelectiveReadyDeferred.promise.then(function () {
                    
                    var payload = {
                        context: getContext(), 
                        ruleEntity: ruleEntity,
                        tableId: tableId
                    };
                    VRUIUtilsService.callDirectiveLoad(dimensionSelectiveAPI, payload, loadDimensionSelectiveDeferred);
                });
               return loadDimensionSelectiveDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDimensionSelective])
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        };

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            
            return currentContext;
        }
    }
    app.controller("VR_Analytic_DimensionMappingRulesGridEditorController", DimensionMappingRulesGridEditorController);
})(appControllers);
