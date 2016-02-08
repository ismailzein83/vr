(function (appControllers) {

    "use strict";

    ExpressionBuilderEditorController.$inject = ['$scope', 'VR_GenericData_ExpressionBuilderConfigAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function ExpressionBuilderEditorController($scope, VR_GenericData_ExpressionBuilderConfigAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var expressionBuilderEntity;

        var expressionBuilderDirectiveAPI;
        var expressionBuilderDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                expressionBuilderEntity = parameters.ExpressionBuilderEntity;
            }
            isEditMode = (expressionBuilderEntity != undefined);
        }

        function defineScope() {

            $scope.expressionBuilderTemplates = [];

            $scope.onExpressionBuilderTemplateDirectiveReady = function (api) {
                expressionBuilderDirectiveAPI = api;
                expressionBuilderDirectiveReadyPromiseDeferred.resolve();
            }

         
            $scope.SaveExpressionBuilder = function () {
                if (isEditMode) {
                    return updateExpressionBuilder();
                }
                else {
                    return insertExpressionBuilder();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                    loadAllControls().finally(function () {
                            dataTransformationDefinitionEntity = undefined;
                        });
            }
            else {
                loadAllControls();
            }

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, setTitle, loadAllStepts]).then(function () {
                })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    })
                   .finally(function () {
                       $scope.scopeModal.isLoading = false;
                   });


                function setTitle() {
                    if (isEditMode && dataTransformationDefinitionEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(dataTransformationDefinitionEntity.Name, 'Data Transformation');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Data Transformation');
                }
                function loadAllStepts() {
                    return VR_GenericData_DataTransformationStepConfigAPIService.GetDataTransformationSteps().then(function (response) {
                        for (var i = 0; i < response.length; i++) {
                            $scope.scopeModal.steps.push(response[i]);
                        }
                    });
                }
                function loadFilterBySection() {
                    if (dataTransformationDefinitionEntity != undefined) {
                        $scope.scopeModal.name = dataTransformationDefinitionEntity.Name;
                        $scope.scopeModal.title = dataTransformationDefinitionEntity.Title;
                    }
                }

                    // return UtilsService.waitMultiplePromises(promises);
                }
        }

        function buildDataTransformationDefinitionObjFromScope() {
            var obj = dataRecordTypeAPI.getData();
            var dataTransformationDefinition = {
                Name: $scope.scopeModal.name,
                Title: $scope.scopeModal.title,
                DataTransformationDefinitionId: dataTransformationDefinitionId,
                RecordTypes: obj != undefined ? obj.RecordTypes : undefined,
            }
            return dataTransformationDefinition;
        }

        function insertDataTransformationDefinition() {

            var dataTransformationDefinitionObject = buildDataTransformationDefinitionObjFromScope();
                    if ($scope.onDataTransformationDefinitionAdded != undefined)
                        $scope.onDataTransformationDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();

        }

        function updateDataTransformationDefinition() {
            var dataTransformationDefinitionObject = buildDataTransformationDefinitionObjFromScope();
                    if ($scope.onDataTransformationDefinitionUpdated != undefined)
                        $scope.onDataTransformationDefinitionUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
        }

    }

    appControllers.controller('VR_GenericData_ExpressionBuilderEditorController', ExpressionBuilderEditorController);
})(appControllers);
