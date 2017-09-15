(function (appControllers) {

    "use strict";

    MappingTelesUserEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'Retail_Teles_UserAPIService', 'Retail_BE_AccountBEDefinitionAPIService','Retail_Teles_EnterpriseAPIService','InsertOperationResultEnum'];

    function MappingTelesUserEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_Teles_UserAPIService, Retail_BE_AccountBEDefinitionAPIService, Retail_Teles_EnterpriseAPIService, InsertOperationResultEnum) {
        var isEditMode;

        var accountId;
        var telesInfoEntity;
        var accountActionDefinitionEntity;
        var accountBEDefinitionId;
        var actionDefinitionId;

        var domainsDirectiveAPI;
        var domainsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedDomainDeferred;

        var enterprisesDirectiveAPI;
        var enterprisesDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedEnterpriseDeferred;

        var sitesDirectiveAPI;
        var sitesDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedSiteDeferred;

        var usersDirectiveAPI;
        var usersDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.onDomainsDirectiveReady = function (api) {
                domainsDirectiveAPI = api;
                domainsDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onDomainsSelectionChanged= function (value) {
                if(value != undefined)
                {
                    if (selectedDomainDeferred != undefined)
                        selectedDomainDeferred.resolve();
                    else
                    {
                        var payload = {
                            vrConnectionId: accountActionDefinitionEntity.ActionDefinitionSettings.VRConnectionId,
                            filter: {
                                AccountBEDefinitionId: accountBEDefinitionId,
                                TelesDomainId: domainsDirectiveAPI.getSelectedIds(),
                            }
                        };
                        var setLoader = function (value) {
                            $scope.scopeModel.isEnterprisesSelectorLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, enterprisesDirectiveAPI, payload, setLoader, undefined);
                        sitesDirectiveAPI.clearDataSource();
                        usersDirectiveAPI.clearDataSource();
                    }
                   
                }
            };
            $scope.scopeModel.onEnterprisesDirectiveReady = function (api) {
                enterprisesDirectiveAPI = api;
                enterprisesDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onEnterprisesSelectionChanged = function (value) {
                if (value != undefined) {
                    if (selectedEnterpriseDeferred != undefined)
                        selectedEnterpriseDeferred.resolve();
                    else
                    {
                        var payload = {
                            vrConnectionId: accountActionDefinitionEntity.ActionDefinitionSettings.VRConnectionId,
                            enterpriseId: enterprisesDirectiveAPI.getSelectedIds(),
                            filter: {
                                AccountBEDefinitionId: accountBEDefinitionId,
                            }
                        };
                        var setLoader = function (value) {
                            $scope.scopeModel.isSitesSelectorLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sitesDirectiveAPI, payload, setLoader, undefined);
                        usersDirectiveAPI.clearDataSource();
                    }
                }
            };

            $scope.scopeModel.onSitesDirectiveReady = function (api) {
                sitesDirectiveAPI = api;
                sitesDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onSitesSelectionChanged = function (value) {
                if (value != undefined) {
                    if (selectedSiteDeferred != undefined)
                        selectedSiteDeferred.resolve();
                    else
                    {
                        var payload = {
                            vrConnectionId: accountActionDefinitionEntity.ActionDefinitionSettings.VRConnectionId,
                            siteId: sitesDirectiveAPI.getSelectedIds(),
                            filter: {
                                AccountBEDefinitionId: accountBEDefinitionId,
                                Filters: [{
                                    $type: "Retail.Teles.Business.UsersNotMappedToAccountFilter,Retail.Teles.Business",
                                    AccountId: accountId
                                }],
                            }
                        };
                        var setLoader = function (value) {
                            $scope.scopeModel.isUsersSelectorLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, usersDirectiveAPI, payload, setLoader, undefined);
                    }
                   
                }
            };
            $scope.scopeModel.onUsersDirectiveReady = function (api) {
                usersDirectiveAPI = api;
                usersDirectiveReadyDeferred.resolve();
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
            UtilsService.waitMultipleAsyncOperations([getAccountActionDefinition]).then(function () {
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

            function setTitle() {
                $scope.title = 'Mapping to teles account';
            }
            function loadStaticData() {
                if(telesInfoEntity != undefined)
                {
                    selectedDomainDeferred = UtilsService.createPromiseDeferred();
                    selectedEnterpriseDeferred = UtilsService.createPromiseDeferred();
                    selectedSiteDeferred = UtilsService.createPromiseDeferred();
                }
            }
            function loadDomainsDirective() {
                var domainsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                domainsDirectiveReadyDeferred.promise.then(function () {
                    var domainsDirectivePayload;
                    if (accountActionDefinitionEntity != undefined && accountActionDefinitionEntity.ActionDefinitionSettings != undefined) {
                        domainsDirectivePayload = {
                            vrConnectionId: accountActionDefinitionEntity.ActionDefinitionSettings.VRConnectionId,
                            selectedIds: telesInfoEntity != undefined ? telesInfoEntity.TelesDomainId : undefined,
                            filter: {
                                AccountBEDefinitionId: accountBEDefinitionId
                            }
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(domainsDirectiveAPI, domainsDirectivePayload, domainsDirectiveLoadDeferred);
                });

                return domainsDirectiveLoadDeferred.promise;
            }
            function loadEnterprisesDirective() {
                if (telesInfoEntity != undefined) {
                    var enterprisesDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultiplePromises([enterprisesDirectiveReadyDeferred.promise, selectedDomainDeferred.promise]).then(function () {
                        selectedDomainDeferred = undefined;
                        var enterprisesDirectivePayload;
                        if (accountActionDefinitionEntity != undefined && accountActionDefinitionEntity.ActionDefinitionSettings != undefined) {
                            enterprisesDirectivePayload = {
                                vrConnectionId: accountActionDefinitionEntity.ActionDefinitionSettings.VRConnectionId,
                                selectedIds: telesInfoEntity != undefined ? telesInfoEntity.TelesEnterpriseId : undefined,
                                filter: {
                                    AccountBEDefinitionId: accountBEDefinitionId,
                                    TelesDomainId: telesInfoEntity != undefined ? telesInfoEntity.TelesDomainId : undefined,
                                }
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(enterprisesDirectiveAPI, enterprisesDirectivePayload, enterprisesDirectiveLoadDeferred);
                    });

                    return enterprisesDirectiveLoadDeferred.promise;
                }

            }
            function loadSitesDirective() {
                if (telesInfoEntity != undefined) {
                    var sitesDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultiplePromises([sitesDirectiveReadyDeferred.promise, selectedEnterpriseDeferred.promise]).then(function () {
                        selectedEnterpriseDeferred = undefined;
                        var sitesDirectivePayload;
                        if (accountActionDefinitionEntity != undefined && accountActionDefinitionEntity.ActionDefinitionSettings != undefined) {
                            sitesDirectivePayload = {
                                vrConnectionId: accountActionDefinitionEntity.ActionDefinitionSettings.VRConnectionId,
                                selectedIds: telesInfoEntity != undefined ? telesInfoEntity.TelesSiteId : undefined,
                                enterpriseId: telesInfoEntity != undefined ? telesInfoEntity.TelesEnterpriseId : undefined,
                                filter: {
                                    AccountBEDefinitionId: accountBEDefinitionId,
                                }
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(sitesDirectiveAPI, sitesDirectivePayload, sitesDirectiveLoadDeferred);
                    });

                    return sitesDirectiveLoadDeferred.promise;
                }

            }
            function loadUsersDirective() {
                if (telesInfoEntity != undefined) {
                    var usersDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultiplePromises([usersDirectiveReadyDeferred.promise, selectedSiteDeferred.promise]).then(function () {
                        selectedSiteDeferred = undefined;
                        var usersDirectivePayload;
                        if (accountActionDefinitionEntity != undefined && accountActionDefinitionEntity.ActionDefinitionSettings != undefined) {
                            usersDirectivePayload = {
                                vrConnectionId: accountActionDefinitionEntity.ActionDefinitionSettings.VRConnectionId,
                                selectedIds: telesInfoEntity != undefined ? telesInfoEntity.TelesUserId : undefined,
                                siteId: telesInfoEntity != undefined ? telesInfoEntity.TelesSiteId : undefined,
                                filter: {
                                    AccountBEDefinitionId: accountBEDefinitionId,
                                    Filters: [{
                                        $type: "Retail.Teles.Business.UsersNotMappedToAccountFilter,Retail.Teles.Business",
                                        EditedUserId: telesInfoEntity != undefined ? telesInfoEntity.TelesUserId : undefined,
                                        AccountId: accountId
                                    }],
                                }
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(usersDirectiveAPI, usersDirectivePayload, usersDirectiveLoadDeferred);
                    });

                    return usersDirectiveLoadDeferred.promise;
                }

            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDomainsDirective, loadEnterprisesDirective, loadSitesDirective, loadUsersDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

           
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return Retail_Teles_UserAPIService.MapUserToAccount(buildMapUserToAccountObjFromScope()).then(function (response) {
                if (response) {
                    switch (response.Result) {
                        case InsertOperationResultEnum.Succeeded.value:
                            VRNotificationService.showSuccess("User mapped successfully");
                            if ($scope.onMappingUser != undefined) {
                                $scope.onMappingUser(response.UpdatedObject);
                            }
                            $scope.modalContext.closeModal();
                            break;
                        case InsertOperationResultEnum.Failed.value:
                            VRNotificationService.showError("Failed to map user.");
                            break;
                        case InsertOperationResultEnum.SameExists.value:
                            VRNotificationService.showWarning("Same user mapped.");
                    }

                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildMapUserToAccountObjFromScope() {
            return {
                TelesDomainId: domainsDirectiveAPI.getSelectedIds(),
                TelesEnterpriseId: enterprisesDirectiveAPI.getSelectedIds(),
                TelesSiteId:sitesDirectiveAPI.getSelectedIds(),
                TelesUserId: usersDirectiveAPI.getSelectedIds(),
                AccountBEDefinitionId: accountBEDefinitionId,
                AccountId: accountId,
                ActionDefinitionId: actionDefinitionId
            };
        }
    }

    appControllers.controller('Retail_Teles_MappingTelesUserEditorController', MappingTelesUserEditorController);

})(appControllers);