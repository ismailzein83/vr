(function (appControllers) {

    "use strict";

    MappingTelesAccountEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService','Retail_Teles_EnterpriseAPIService','Retail_BE_AccountBEDefinitionAPIService'];

    function MappingTelesAccountEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_Teles_EnterpriseAPIService, Retail_BE_AccountBEDefinitionAPIService) {
        var isEditMode;

        var enterpriseIdId;
        var accountActionDefinitionEntity;

        var accountBEDefinitionId;
        var actionDefinitionId;

        var enterprisesDirectiveAPI;
        var enterprisesDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                enterpriseIdId = parameters.enterpriseIdId;
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                actionDefinitionId = parameters.actionDefinitionId;
            }
            isEditMode = (enterpriseIdId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onEnterprisesDirectiveReady = function (api) {
                enterprisesDirectiveAPI = api;
                enterprisesDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            getAccountActionDefinition().then(function () {
                loadAllControls();
            });
        }
        function getAccountActionDefinition()
        {
            return Retail_BE_AccountBEDefinitionAPIService.GetAccountActionDefinition(accountBEDefinitionId, actionDefinitionId).then(function (reponse) {
                accountActionDefinitionEntity = reponse;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                $scope.title = 'Mapping Teles Account';
            }
            function loadStaticData() {
            }
            function loadSettingsDirective() {
                var enterprisesDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                enterprisesDirectiveReadyDeferred.promise.then(function () {
                    var enterprisesDirectivePayload;
                    if (accountActionDefinitionEntity != undefined && accountActionDefinitionEntity.ActionDefinitionSettings != undefined) {
                        enterprisesDirectivePayload = {
                            switchId: accountActionDefinitionEntity.ActionDefinitionSettings.SwitchId,
                            domainId: accountActionDefinitionEntity.ActionDefinitionSettings.DomainId,
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(enterprisesDirectiveAPI, enterprisesDirectivePayload, enterprisesDirectiveLoadDeferred);
                });

                return enterprisesDirectiveLoadDeferred.promise;
            }
        }

        function insert() {
            //$scope.scopeModel.isLoading = true;
            //return Retail_Teles_EnterpriseAPIService.AddEnterprise(buildEnterpriseObjFromScope()).then(function (response) {
            //    if (VRNotificationService.notifyOnItemAdded('Enterprise', response, 'Name')) {
            //        if ($scope.onEnterpriseAdded != undefined)
            //            $scope.onEnterpriseAdded(response.InsertedObject);
            //        $scope.modalContext.closeModal();
            //    }
            //}).catch(function (error) {
            //    VRNotificationService.notifyException(error, $scope);
            //}).finally(function () {
            //    $scope.scopeModel.isLoading = false;
            //});
        }
        function update() {
            //$scope.scopeModel.isLoading = true;
            //return Retail_Teles_EnterpriseAPIService.UpdateEnterprise(buildEnterpriseObjFromScope()).then(function (response) {
            //    if (VRNotificationService.notifyOnItemUpdated('Enterprise', response, 'Name')) {
            //        if ($scope.onEnterpriseUpdated != undefined) {
            //            $scope.onEnterpriseUpdated(response.UpdatedObject);
            //        }
            //        $scope.modalContext.closeModal();
            //    }
            //}).catch(function (error) {
            //    VRNotificationService.notifyException(error, $scope);
            //}).finally(function () {
            //    $scope.scopeModel.isLoading = false;
            //});
        }

        function buildEnterpriseObjFromScope() {
            return {
            };
        }
    }

    appControllers.controller('Retail_Teles_MappingTelesAccountEditorController', MappingTelesAccountEditorController);

})(appControllers);