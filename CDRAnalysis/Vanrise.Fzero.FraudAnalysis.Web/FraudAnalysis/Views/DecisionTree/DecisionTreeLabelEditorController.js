(function (appControllers) {

    'use strict';

    LabelEditorController.$inject = ['$scope', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'CDRAnalysis_FA_SuspicionLevelEnum', 'VRUIUtilsService'];

    function LabelEditorController($scope, VRModalService, VRNotificationService, VRNavigationService, UtilsService, CDRAnalysis_FA_SuspicionLevelEnum, VRUIUtilsService) {
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
            $scope.scopeModel.levels = [];

            $scope.scopeModel.saveLabel = function () {
                if (isEditMode) {
                    return updateLabel();
                }
                else {
                    return addLabel();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticControls, loadLevelTypes]).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                var suspicionLevel = labelEntity != undefined?UtilsService.getItemByVal($scope.scopeModel.suspicionLevels, labelEntity.SuspicionLevel, 'value'):undefined;
                $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(suspicionLevel ? suspicionLevel.SuspicionLevel : null, 'Label') : UtilsService.buildTitleForAddEditor('Label');
            }
            function loadStaticControls() {
                if (labelEntity) {
                    $scope.scopeModel.suspicionLevel = UtilsService.getItemByVal($scope.scopeModel.suspicionLevels,labelEntity.SuspicionLevel, 'value');
                }
            }

        }

        function addLabel() {
            var labelObject = buildLabelObjFromScope();
            if ($scope.onLabelAdded != undefined && typeof $scope.onLabelAdded == 'function')
                $scope.onLabelAdded(labelObject);
            $scope.modalContext.closeModal();
                
        }

        function updateLabel() {
            var labelObject = buildLabelObjFromScope();
            if ($scope.onLabelUpdated != undefined && typeof $scope.onLabelUpdated == 'function')
                $scope.onLabelUpdated(labelObject);
            $scope.modalContext.closeModal();
             
        }

        function buildLabelObjFromScope() {
            var labelObject = {
                SuspicionLevel: $scope.scopeModel.level.value == -1 ? undefined : $scope.scopeModel.level.value,
            };
            return labelObject;
        }
        function loadLevelTypes()
        {
            var cleanLevel = {
                value : -1,
                description : "Clean"
            }
            $scope.scopeModel.levels.push(cleanLevel);
            var levels = UtilsService.getArrayEnum(CDRAnalysis_FA_SuspicionLevelEnum);
            for(var i=0;i<levels.length;i++)
            {
                $scope.scopeModel.levels.push(levels[i]);
            }
        }
    }
    appControllers.controller('CDRAnalysis_FA_LabelEditorController', LabelEditorController);

})(appControllers);
