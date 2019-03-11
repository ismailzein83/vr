(function (appControllers) {

    "use strict";

    CaseCDRChangeStatusControllerEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_TestCallAnalysis_CaseCDRAPIService'];

    function CaseCDRChangeStatusControllerEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_TestCallAnalysis_CaseCDRAPIService) {

        var caseCDREntity;
        var genericBusinessEntityId;
        var businessEntityDefinitionId;

        var statusDefinitionAPI;
        var statusDefinitionReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                genericBusinessEntityId = parameters.genericBusinessEntityId;
                businessEntityDefinitionId = parameters.businessEntityDefinitionId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                statusDefinitionAPI = api;
                statusDefinitionReadyDeferred.resolve();
            };

            $scope.scopeModel.saveStatus = function () {
                $scope.scopeModel.isLoading = true;
              
                return VR_TestCallAnalysis_CaseCDRAPIService.UpdateCaseCDRStatus(buildCaseCDRToUpdate()).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated('Case CDR Table', response, 'Name')) {
                        if ($scope.onGenericBEUpdated != undefined)
                            $scope.onGenericBEUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            caseCDREntity = undefined;

            getCaseCDR().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });


            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadCaseCDRStatusSelector]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }

            function setTitle() {
                if (caseCDREntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(caseCDREntity.CallingNumber, 'Change Status Editor');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Change Status Editor');
            }

        }

        function getCaseCDR() {
            return VR_TestCallAnalysis_CaseCDRAPIService.GetCaseCDREntity(businessEntityDefinitionId, genericBusinessEntityId).then(function (response) {
                if (response != null) {
                    caseCDREntity = response;
                }
            });
        }

        function loadCaseCDRStatusSelector() {
            var promises = [];
            if (caseCDREntity != undefined) {
                var loadStatusDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();

                promises.push(loadStatusDefinitionPromiseDeferred.promise);
                UtilsService.waitMultiplePromises([statusDefinitionReadyDeferred.promise]).then(function () {
                    var statusDefinitionPayload = {
                        filter: {
                            BusinessEntityDefinitionId: '36D87065-A854-4A56-AD28-CCADB74B31EF',  
                            Filters: [{
                                $type: "TestCallAnalysis.Business.CaseCDRChangeStatusFilter ,TestCallAnalysis.Business",
                                StatusDefinitionId: caseCDREntity.StatusId
                            }]
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(statusDefinitionAPI, statusDefinitionPayload, loadStatusDefinitionPromiseDeferred);
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }


        function buildCaseCDRToUpdate() {
            return {
                CaseCDRId: genericBusinessEntityId,
                StatusId: statusDefinitionAPI.getSelectedIds(),
            };
        }
    }

    appControllers.controller('VR_TestCallAnalysis_CaseCDRChangeStatusController', CaseCDRChangeStatusControllerEditorController);
})(appControllers);
