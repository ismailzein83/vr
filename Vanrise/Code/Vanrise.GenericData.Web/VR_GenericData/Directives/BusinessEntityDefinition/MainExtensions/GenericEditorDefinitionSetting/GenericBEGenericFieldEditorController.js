(function (appControllers) {

    "use strict";

    GenericFieldEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function GenericFieldEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var recordTypeFields;
        var fieldEntity;
        var context;

        var genericFieldDefinitionDirectiveAPI;
        var genericFieldDefinitionDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                recordTypeFields = parameters.recordTypeFields;
                context = parameters.context;
                fieldEntity = parameters.fieldEntity;
            }
            isEditMode = (fieldEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGenericFieldDefinitionDirectiveReady = function (api) {
                genericFieldDefinitionDirectiveAPI = api;
                genericFieldDefinitionDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.saveGenericField = function () {
                if (isEditMode) {
                    return updateField();
                }
                else {
                    return insertField();
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
            return UtilsService.waitMultipleAsyncOperations([loadGenericFieldDirective, setTitle]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode)
                    $scope.title = UtilsService.buildTitleForUpdateEditor('Field');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Field');
            }

            function loadGenericFieldDirective() {
                var loadGenericFieldDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                genericFieldDefinitionDirectiveReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        recordTypeFields: recordTypeFields,
                        context: context,
                        entity: fieldEntity
                    };

                    VRUIUtilsService.callDirectiveLoad(genericFieldDefinitionDirectiveAPI, payload, loadGenericFieldDirectivePromiseDeferred);
                });

                return loadGenericFieldDirectivePromiseDeferred.promise;
            }
        }
        function buildFieldObjectFromScope() {
            return genericFieldDefinitionDirectiveAPI.getData();
        }

        function insertField() {
            var fieldObject = buildFieldObjectFromScope();
            if ($scope.onBEFieldAdded != undefined)
                $scope.onBEFieldAdded(fieldObject);
            $scope.modalContext.closeModal();
        }

        function updateField() {
            var fieldObject = buildFieldObjectFromScope();
            if ($scope.onBEFieldUpdated != undefined)
                $scope.onBEFieldUpdated(fieldObject);
            $scope.modalContext.closeModal();
        }

    }

    appControllers.controller('VR_GenericData_GenericFieldEditorController', GenericFieldEditorController);
})(appControllers);