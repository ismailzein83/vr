(function (appControllers) {

    "use strict";

    ChangeUserRoutingGroupEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'Retail_Teles_UserAPIService', 'Retail_BE_AccountBEDefinitionAPIService', 'Retail_Teles_EnterpriseAPIService', 'InsertOperationResultEnum'];

    function ChangeUserRoutingGroupEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_Teles_UserAPIService, Retail_BE_AccountBEDefinitionAPIService, Retail_Teles_EnterpriseAPIService, InsertOperationResultEnum) {
        var isEditMode;

        var accountId;
        var telesInfoEntity;
        var accountActionDefinitionEntity;
        var accountBEDefinitionId;
        var actionDefinitionId;
        var siteId;
        var currentUserRoutingGroup;
        var siteRoutingGroupDirectiveAPI;
        var siteRoutingGroupDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                telesInfoEntity = parameters.telesInfoEntity;
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                actionDefinitionId = parameters.actionDefinitionId;
                accountId = parameters.accountId;
            }
            isEditMode = (telesInfoEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
       
            $scope.scopeModel.onSiteRoutingGroupSelectorReady = function (api) {
                siteRoutingGroupDirectiveAPI = api;
                siteRoutingGroupDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return changeUserRoutingGroup();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            UtilsService.waitMultipleAsyncOperations([getAccountActionDefinition]).then(function () {
                UtilsService.waitMultipleAsyncOperations([getUserTelesSiteId, getCurrentUserRoutingGroup]).then(function () {
                    loadAllControls();
                })
            });
        }
        function getAccountActionDefinition()
        {
            return Retail_BE_AccountBEDefinitionAPIService.GetAccountActionDefinition(accountBEDefinitionId, actionDefinitionId).then(function (reponse) {
                accountActionDefinitionEntity = reponse;
            });
        }
        function getUserTelesSiteId()
        {
            return Retail_Teles_UserAPIService.GetUserTelesSiteId(accountBEDefinitionId, accountId, accountActionDefinitionEntity.ActionDefinitionSettings.VRConnectionId).then(function (response) {
                siteId = response;
            });
        }
        function getCurrentUserRoutingGroup() {
            if (telesInfoEntity != undefined) {
                return Retail_Teles_UserAPIService.GetCurrentUserRoutingGroupId(accountBEDefinitionId, accountId, accountActionDefinitionEntity.ActionDefinitionSettings.VRConnectionId).then(function (response) {
                    currentUserRoutingGroup = response;
                });
            }
        }
        function loadAllControls() {

            function setTitle() {
                $scope.title = 'Change User Routing Group';
            }
            function loadStaticData() {
               
            }
            function loadSiteRoutingGroupSelector() {
                if (telesInfoEntity != undefined) {
                    var siteRoutingGroupSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    siteRoutingGroupDirectiveReadyDeferred.promise.then(function () {
                         var  sitesDirectivePayload = {
                                vrConnectionId: accountActionDefinitionEntity.ActionDefinitionSettings.VRConnectionId,
                                selectedIds: currentUserRoutingGroup,
                                siteId: siteId ,
                            };
                         VRUIUtilsService.callDirectiveLoad(siteRoutingGroupDirectiveAPI, sitesDirectivePayload, siteRoutingGroupSelectorLoadDeferred);
                    });

                    return siteRoutingGroupSelectorLoadDeferred.promise;
                }

            }
          
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSiteRoutingGroupSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

           
        }

        function changeUserRoutingGroup() {
            $scope.scopeModel.isLoading = true;
            return Retail_Teles_UserAPIService.ChangeUserRoutingGroup(accountBEDefinitionId, accountId, accountActionDefinitionEntity.ActionDefinitionSettings.VRConnectionId, siteRoutingGroupDirectiveAPI.getSelectedIds()).then(function (response) {
                if (response) {
                    switch (response.Result) {
                        case InsertOperationResultEnum.Succeeded.value:
                            VRNotificationService.showSuccess("User routing group changed successfully");
                            if ($scope.onChangeUserRoutingGroup != undefined) {
                                $scope.onChangeUserRoutingGroup(response.UpdatedObject);
                            }
                            $scope.modalContext.closeModal();
                            break;
                        case InsertOperationResultEnum.Failed.value:
                            VRNotificationService.showError("Failed to change user routing group.");
                            break;
                        case InsertOperationResultEnum.SameExists.value:
                            VRNotificationService.showWarning("User already assigned to the same routing group.");
                    }

                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }

    appControllers.controller('Retail_Teles_ChangeUserRoutingGroupEditorController', ChangeUserRoutingGroupEditorController);

})(appControllers);