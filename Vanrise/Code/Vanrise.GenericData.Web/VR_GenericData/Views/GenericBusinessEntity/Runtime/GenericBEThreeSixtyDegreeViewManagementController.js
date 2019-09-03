(function (appControllers) {

    'use strict';

    GenericBEThreeSixtyDegreeViewManagementController.$inject = ['$scope', 'VR_GenericData_GenericBEDefinitionAPIService', 'VR_GenericData_GenericBusinessEntityAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function GenericBEThreeSixtyDegreeViewManagementController($scope, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_GenericBusinessEntityAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var genericBusinessEntityId;
        var parentBusinessEntityDefinitionId;
        var fieldName;

        var businessEntityDefinitionId;

        var genericBE360degreeViewerDirectiveAPI;
        var genericBE360degreeViewerDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                parentBusinessEntityDefinitionId = parameters.parentBusinessEntityDefinitionId;
                genericBusinessEntityId = parameters.genericBusinessEntityId;
                fieldName = parameters.fieldName;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGenericBE360degreeViewerDirectiveReady = function (api) {
                genericBE360degreeViewerDirectiveAPI = api;
                genericBE360degreeViewerDirectiveReadyDeferred.resolve();
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

            function loadGenericBE360degreeViewerDirective() {
                var genericBE360degreeViewerDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                genericBE360degreeViewerDirectiveReadyDeferred.promise.then(function () {
                    var genericBE360degreeViewerDirectivePayload = {
                        parentBusinessEntityDefinitionId: parentBusinessEntityDefinitionId,
                        genericBusinessEntityId: genericBusinessEntityId,
                        fieldName: fieldName,
                        modalContext: getModalContext(),
                        hideCloseButton: true
                    };
                    VRUIUtilsService.callDirectiveLoad(genericBE360degreeViewerDirectiveAPI, genericBE360degreeViewerDirectivePayload, genericBE360degreeViewerDirectiveLoadDeferred);
                });

                return genericBE360degreeViewerDirectiveLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([loadGenericBE360degreeViewerDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function getModalContext() {
            return {
                closeModal: function () {
                    $scope.modalContext.closeModal();
                }
            };
        }
    }

    appControllers.controller('VR_GenericData_GenericBEThreeSixtyDegreeViewManagementController', GenericBEThreeSixtyDegreeViewManagementController);
})(appControllers);