(function (appControllers) {

    'use strict';

    ConditionEditorController.$inject = ['$scope', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'CDRAnalysis_FA_DecisionTreeConditionOperatorEnum', 'VRUIUtilsService','StrategyAPIService'];

    function ConditionEditorController($scope, VRModalService, VRNotificationService, VRNavigationService, UtilsService, CDRAnalysis_FA_DecisionTreeConditionOperatorEnum, VRUIUtilsService, StrategyAPIService) {
        var isEditMode;
        var labelEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                labelEntity = parameters.labelObj;
            }

            isEditMode = labelEntity != undefined;
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.filters = [];
            $scope.scopeModel.operators = UtilsService.getArrayEnum(CDRAnalysis_FA_DecisionTreeConditionOperatorEnum);
            $scope.scopeModel.saveCondition = function () {
                if (isEditMode) {
                    return updateCondition();
                }
                else {
                    return addCondition();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticControls,loadFilters]).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                var suspicionLevel = labelEntity != undefined ? UtilsService.getItemByVal($scope.scopeModel.suspicionLevels, labelEntity.SuspicionLevel, 'value') : undefined;
                $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(suspicionLevel ? suspicionLevel.SuspicionLevel : null, 'Condition') : UtilsService.buildTitleForAddEditor('Condition');
            }
            function loadStaticControls() {
                if (labelEntity) {
                    $scope.scopeModel.suspicionLevel = UtilsService.getItemByVal($scope.scopeModel.suspicionLevels, labelEntity.SuspicionLevel, 'value');
                }
            }

        }
        function loadFilters() {
            return StrategyAPIService.GetFilters().then(function (response) {
                $scope.scopeModel.filters = response;
            });
        }
        function addCondition() {
            var labelObject = buildConditionObjFromScope();
            if ($scope.onConditionAdded != undefined && typeof $scope.onConditionAdded == 'function')
                $scope.onConditionAdded(labelObject);
            $scope.modalContext.closeModal();

        }

        function updateCondition() {
            var labelObject = buildConditionObjFromScope();
            if ($scope.onConditionUpdated != undefined && typeof $scope.onConditionUpdated == 'function')
                $scope.onConditionUpdated(labelObject);
            $scope.modalContext.closeModal();

        }

        function buildConditionObjFromScope() {
            var labelObject = {
                Value: $scope.scopeModel.conditionValue,
                Operator: $scope.scopeModel.selectedOperator.value,
                FilterId: $scope.scopeModel.selectedFilter.FilterId
            };
            return labelObject;
        }
    }
    appControllers.controller('CDRAnalysis_FA_ConditionEditorController', ConditionEditorController);

})(appControllers);
