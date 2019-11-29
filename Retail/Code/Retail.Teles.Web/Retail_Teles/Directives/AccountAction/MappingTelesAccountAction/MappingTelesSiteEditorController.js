(function (appControllers) {

    "use strict";

    MappingTelesSiteEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'Retail_Teles_SiteAPIService', 'Retail_BE_AccountBEDefinitionAPIService', 'Retail_Teles_EnterpriseAPIService', 'InsertOperationResultEnum'];

    function MappingTelesSiteEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_Teles_SiteAPIService, Retail_BE_AccountBEDefinitionAPIService, Retail_Teles_EnterpriseAPIService, InsertOperationResultEnum) {
        var isEditMode;

        var accountId;
        var telesInfoEntity;
        var accountActionDefinitionEntity;
        var enterpriseId;
        var accountBEDefinitionId;
        var actionDefinitionId;


        var enterprisesDirectiveAPI;
        var enterprisesDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        var enterprisesSelectedDeferred;

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

            $scope.scopeModel.onEnterprisesDirectiveReady = function (api) {
                enterprisesDirectiveAPI = api;
                enterprisesDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onSitesDirectiveReady = function (api) {
                sitesDirectiveAPI = api;
                sitesDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onEnterprisesSelectionChanged = function (selectedItem) {
                if (selectedItem != undefined) {
                    if (enterprisesSelectedDeferred != undefined) {
                        enterprisesSelectedDeferred.resolve();
                    } else {
                        var sitesDirectivePayload = {
                            vrConnectionId: accountActionDefinitionEntity.ActionDefinitionSettings.VRConnectionId,
                            enterpriseId: selectedItem.TelesEnterpriseId,
                            filter: {
                                Filters: [{
                                    $type: "Retail.Teles.Business.SitesNotMappedToAccountFilter,Retail.Teles.Business",
                                    EditedSiteId: telesInfoEntity != undefined ? telesInfoEntity.TelesSiteId : undefined,
                                    AccountId: accountId
                                }],
                                AccountBEDefinitionId: accountBEDefinitionId
                            }
                        };
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingSelector = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sitesDirectiveAPI, sitesDirectivePayload, setLoader);

                    }
                } else {
                    sitesDirectiveAPI.clearDataSource();
                }
            };

            $scope.scopeModel.save = function () {
                return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            UtilsService.waitMultipleAsyncOperations([getAccountActionDefinition, getAccountEnterpriseId]).then(function () {
                if (isEditMode) {
                    enterpriseId = telesInfoEntity.TelesEnterpriseId != undefined ? telesInfoEntity.TelesEnterpriseId : enterpriseId;
                }

                if (accountActionDefinitionEntity != undefined && accountActionDefinitionEntity.ActionDefinitionSettings != undefined)
                    $scope.scopeModel.showEnterprise = accountActionDefinitionEntity.ActionDefinitionSettings.ShowEnterprise;
                if ($scope.scopeModel.showEnterprise) {
                    if (enterpriseId != undefined) {
                        enterprisesSelectedDeferred = UtilsService.createPromiseDeferred();
                    }
                }

                loadAllControls();
            });
        }
        function getAccountActionDefinition() {
            return Retail_BE_AccountBEDefinitionAPIService.GetAccountActionDefinition(accountBEDefinitionId, actionDefinitionId).then(function (reponse) {
                accountActionDefinitionEntity = reponse;
            });
        }
        function getAccountEnterpriseId() {
            return Retail_Teles_EnterpriseAPIService.GetParentAccountEnterpriseId(accountBEDefinitionId, accountId).then(function (response) {
                enterpriseId = response;
            });
        }
        function loadAllControls() {
            var promises = [setTitle, loadStaticData];
            if ($scope.scopeModel.showEnterprise) {
                promises.push(loadEnterprisesSelector);
            }
            if (enterpriseId != undefined) {
                promises.push(loadSitesSelector);
            }

            return UtilsService.waitMultipleAsyncOperations(promises).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
                enterprisesSelectedDeferred = undefined;
            });

            function setTitle() {
                $scope.title = 'Mapping to teles account';
            }
            function loadStaticData() {
            }


            function loadEnterprisesSelector() {
                var enterprisesDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                enterprisesDirectiveReadyDeferred.promise.then(function () {
                    var enterprisesDirectivePayload;
                    if (accountActionDefinitionEntity != undefined && accountActionDefinitionEntity.ActionDefinitionSettings != undefined) {
                        enterprisesDirectivePayload = {
                            vrConnectionId: accountActionDefinitionEntity.ActionDefinitionSettings.VRConnectionId,
                            selectedIds: enterpriseId,
                            filter: {
                                Filters: [{
                                    $type: "Retail.Teles.Business.EnterpriseNotMappedToAccountFilter,Retail.Teles.Business",
                                    EditedEnterpriseId: enterpriseId,
                                }],
                                AccountBEDefinitionId: accountBEDefinitionId
                            }
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(enterprisesDirectiveAPI, enterprisesDirectivePayload, enterprisesDirectiveLoadDeferred);
                });

                return enterprisesDirectiveLoadDeferred.promise;
            }


            function loadSitesSelector() {
                var sitesDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                var promises = [sitesDirectiveReadyDeferred.promise];

                if (enterprisesSelectedDeferred != undefined)
                    promises.push(enterprisesSelectedDeferred.promise);

                UtilsService.waitMultiplePromises(promises).then(function () {
                    var enterprisesDirectivePayload;
                    if (accountActionDefinitionEntity != undefined && accountActionDefinitionEntity.ActionDefinitionSettings != undefined) {
                        enterprisesDirectivePayload = {
                            vrConnectionId: accountActionDefinitionEntity.ActionDefinitionSettings.VRConnectionId,
                            enterpriseId: enterpriseId,
                            selectedIds: telesInfoEntity != undefined ? telesInfoEntity.TelesSiteId : undefined,
                            filter: {
                                Filters: [{
                                    $type: "Retail.Teles.Business.SitesNotMappedToAccountFilter,Retail.Teles.Business",
                                    EditedSiteId: telesInfoEntity != undefined ? telesInfoEntity.TelesSiteId : undefined,
                                    AccountId: accountId
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
                if (response) {
                    switch (response.Result) {
                        case InsertOperationResultEnum.Succeeded.value:
                            VRNotificationService.showSuccess("Site mapped successfully");
                            if ($scope.onMappingAccount != undefined) {
                                $scope.onMappingAccount(response.UpdatedObject);
                            }
                            $scope.modalContext.closeModal();
                            break;
                        case InsertOperationResultEnum.Failed.value:
                            VRNotificationService.showError("Failed to map site.");
                            break;
                        case InsertOperationResultEnum.SameExists.value:
                            VRNotificationService.showWarning("Same site mapped.");
                    }

                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildMapSiteToAccountObjFromScope() {
            return {
                TelesEnterpriseId: $scope.scopeModel.showEnterprise ? enterprisesDirectiveAPI.getSelectedIds() : enterpriseId,
                TelesSiteId: sitesDirectiveAPI.getSelectedIds(),
                AccountBEDefinitionId: accountBEDefinitionId,
                AccountId: accountId,
                ActionDefinitionId: actionDefinitionId
            };
        }
    }

    appControllers.controller('Retail_Teles_MappingTelesSiteEditorController', MappingTelesSiteEditorController);

})(appControllers);