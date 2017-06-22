(function (appControllers) {

    "use strict";

    MappingTelesSiteEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'Retail_Teles_SiteAPIService', 'Retail_BE_AccountBEDefinitionAPIService','Retail_Teles_EnterpriseAPIService'];

    function MappingTelesSiteEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_Teles_SiteAPIService, Retail_BE_AccountBEDefinitionAPIService, Retail_Teles_EnterpriseAPIService) {
        var isEditMode;

        var accountId;
        var telesInfoEntity;
        var accountActionDefinitionEntity;
        var enterpriseId;
        var accountBEDefinitionId;
        var actionDefinitionId;

        var sitesDirectiveAPI;
        var sitesDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.onSitesDirectiveReady = function (api) {
                sitesDirectiveAPI = api;
                sitesDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
                return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            UtilsService.waitMultipleAsyncOperations([getAccountActionDefinition, getAccountEnterpriseId]).then(function () {
                loadAllControls();
            });
        }
        function getAccountActionDefinition()
        {
            return Retail_BE_AccountBEDefinitionAPIService.GetAccountActionDefinition(accountBEDefinitionId, actionDefinitionId).then(function (reponse) {
                accountActionDefinitionEntity = reponse;
            });
        }
        function getAccountEnterpriseId() {
            return Retail_Teles_EnterpriseAPIService.GetParentAccountEnterpriseId(accountBEDefinitionId, accountId).then(function (response) {
                enterpriseId = response
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                $scope.title = 'Mapping to teles account';
            }
            function loadStaticData() {
            }
            function loadSettingsDirective() {
                var sitesDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                sitesDirectiveReadyDeferred.promise.then(function () {
                    var enterprisesDirectivePayload;
                    if (accountActionDefinitionEntity != undefined && accountActionDefinitionEntity.ActionDefinitionSettings != undefined) {
                        enterprisesDirectivePayload = {
                            vrConnectionId: accountActionDefinitionEntity.ActionDefinitionSettings.VRConnectionId,
                            enterpriseId:enterpriseId,
                            selectedIds: telesInfoEntity != undefined ? telesInfoEntity.TelesSiteId : undefined,
                            filter: {
                                Filters: [{
                                    $type: "Retail.Teles.Business.SitesNotMappedToAccountFilter,Retail.Teles.Business",
                                    EditedSiteId: telesInfoEntity != undefined ? telesInfoEntity.TelesSiteId : undefined,
                                    EnterpriseId: enterpriseId
                                }],
                                AccountBEDefinitionId: accountBEDefinitionId
                            }
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(sitesDirectiveAPI, enterprisesDirectivePayload, sitesDirectiveLoadDeferred);
                });

                return sitesDirectiveLoadDeferred.promise;
            }
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return Retail_Teles_SiteAPIService.MapSiteToAccount(buildMapSiteToAccountObjFromScope()).then(function (response) {
                if (response)
                {
                    VRNotificationService.showSuccess("Site mapped successfully");
                    if ($scope.onMappingAccount != undefined) {
                        $scope.onMappingAccount(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                } else
                {
                    VRNotificationService.showSuccess("Failed to map account");

                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildMapSiteToAccountObjFromScope() {
            return {
                TelesSiteId: sitesDirectiveAPI.getSelectedIds(),
                AccountBEDefinitionId: accountBEDefinitionId,
                AccountId: accountId
            };
        }
    }

    appControllers.controller('Retail_Teles_MappingTelesSiteEditorController', MappingTelesSiteEditorController);

})(appControllers);