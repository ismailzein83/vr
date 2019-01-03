(function (appControllers) {
    
    'use strict';

    GenericBusinessEntityManagementController.$inject = ['$scope', 'VR_GenericData_GenericBusinessEntityService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_GenericData_GenericBEDefinitionAPIService', 'VR_GenericData_RecordQueryLogicalOperatorEnum', 'VR_GenericData_GenericBusinessEntityAPIService'];

    function GenericBusinessEntityManagementController($scope, VR_GenericData_GenericBusinessEntityService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_RecordQueryLogicalOperatorEnum, VR_GenericData_GenericBusinessEntityAPIService) {

        var viewId;

        var genericBERuntimeManagementDirectiveAPI;
        var genericBERuntimeManagementDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                viewId = parameters.viewId; 
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onGenericBERuntimeManagementDirectiveReady = function (api) {
                genericBERuntimeManagementDirectiveAPI = api;
                genericBERuntimeManagementDirectiveReadyDeferred.resolve();
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
            function loadAllControls() {

                function loadGenericBERuntimeManagementDirective() {
                    var genericBERuntimeManagementDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    genericBERuntimeManagementDirectiveReadyDeferred.promise.then(function () {
                        var payload = {
                            viewId: viewId
                        };

                        VRUIUtilsService.callDirectiveLoad(genericBERuntimeManagementDirectiveAPI, payload, genericBERuntimeManagementDirectiveLoadDeferred);
                    });
                    return genericBERuntimeManagementDirectiveLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises([loadGenericBERuntimeManagementDirective()]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }
        }

    }

    appControllers.controller('VR_GenericData_GenericBusinessEntityManagementController', GenericBusinessEntityManagementController);

})(appControllers);