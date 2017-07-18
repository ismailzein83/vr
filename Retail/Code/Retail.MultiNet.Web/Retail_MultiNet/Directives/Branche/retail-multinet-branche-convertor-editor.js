'use strict';

app.directive('retailMultinetBrancheConvertorEditor', ['UtilsService', 'VRUIUtilsService', 'Retail_BE_EntityTypeEnum', 'VRNotificationService',
    function (UtilsService, VRUIUtilsService, Retail_BE_EntityTypeEnum, VRNotificationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailMultiNetBrancheConvertorEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_MultiNet/Directives/Branche/Templates/BrancheConvertorEditor.html"
        };

        function retailMultiNetBrancheConvertorEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var accountTypeId;
            var statusDefinitionId;

            var accountDefinitionSelectorApi;
            var accountDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var businessEntityDefinitionSelectionChangedDeferred;

            var statusDefinitionSelectorAPI;
            var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var accountTypeSelectorAPI;
            var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

       

            var companyProfileDefinitionSelectorAPI;
            var companyProfileDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var financialDefinitionSelectorAPI;
            var financialDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var branchInfoDefinitionSelectorAPI;
            var branchInfoDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

          

            $scope.scopeModel = {};

            $scope.scopeModel.onAccountDefinitionSelectorReady = function (api) {
                accountDefinitionSelectorApi = api;
                accountDefinitionSelectorPromiseDeferred.resolve();
            };

            $scope.scopeModel.onInitialStatusDefinitionSelectorReady = function (api) {
                statusDefinitionSelectorAPI = api;
                statusDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeSelectorAPI = api;
                accountTypeSelectorReadyDeferred.resolve();
            };

        
            $scope.scopeModel.onCompanyProfileDefinitionSelectorReady = function (api) {
                companyProfileDefinitionSelectorAPI = api;
                companyProfileDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onFinancialDefinitionSelectorReady = function (api) {
                financialDefinitionSelectorAPI = api;
                financialDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.oBranchInfoPartDefinitionSelectorReady = function (api) {
                branchInfoDefinitionSelectorAPI = api;
                branchInfoDefinitionSelectorReadyDeferred.resolve();
            };

           
            $scope.scopeModel.onAccountDefinitionSelectionChanged = function (selectedItem) {
                if (selectedItem != undefined && businessEntityDefinitionSelectionChangedDeferred == undefined) {
                    businessEntityDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                    businessEntityDefinitionSelectionChangedDeferred.resolve();

                    $scope.scopeModel.isAccountTypeSelectorLoading = true;
                    var promises = [];
                    
                    var accountTypeSelectorPayload = {
                        filter: {
                            AccountBEDefinitionId: selectedItem.BusinessEntityDefinitionId
                        },
                        selectedIds: accountTypeId
                    };
                    var statusSelectorPayload = {
                        filter: {
                            Filters: [{
                                $type: "Retail.BusinessEntity.Business.AccountBEStatusDefinitionFilter, Retail.BusinessEntity.Business",
                                AccountBEDefinitionId: selectedItem.BusinessEntityDefinitionId

                            }]
                        },
                        selectedIds: statusDefinitionId
                    };
                    promises.push(loadAccountTypeSelector(accountTypeSelectorPayload));
                    promises.push(loadStatusDefinitionSelector(statusSelectorPayload));

                    UtilsService.waitMultiplePromises(promises).then(function () {
                        businessEntityDefinitionSelectionChangedDeferred = undefined;
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.scopeModel.isAccountTypeSelectorLoading = false;
                    });
                } else if (businessEntityDefinitionSelectionChangedDeferred != undefined) {
                    businessEntityDefinitionSelectionChangedDeferred.resolve();
                }
            };

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var accountTypeSelectorPayload;
                    var statusDefinitionSelectorPayload;

                    promises.push(loadAccountDefinitionSelectorLoad());

                    if (payload != undefined) {
                        accountTypeId = payload.AccountTypeId;
                        statusDefinitionId = payload.InitialStatusId;
                        accountBEDefinitionId = payload.AccountBEDefinitionId;

                        $scope.scopeModel.branchColumnName = payload.BranchIdColumnName;
                        $scope.scopeModel.companyColumnName = payload.CompanyIdColumnName;
                        $scope.scopeModel.accountHolderColumnName = payload.AccountHolderColumnName;
                        $scope.scopeModel.accountStateColumnName = payload.AccountStateColumnName;
                        $scope.scopeModel.insertDateColumnName = payload.InsertDateColumnName;
                        $scope.scopeModel.branchCodeColumnName = payload.BranchCodeColumnName;
                        $scope.scopeModel.contractRefNoColumnName = payload.ContractRefNoColumnName;
                        $scope.scopeModel.currencyIdColumnName = payload.CurrencyIdColumnName;
                        $scope.scopeModel.emailColumnName = payload.EmailColumnName;
                        $scope.scopeModel.activationDateColumnName = payload.ActivationDateColumnName;
                        $scope.scopeModel.smaOwnerIdColumnName = payload.SmaOwnerIdColumnName;
                        $scope.scopeModel.smaAddressColumnName = payload.SmaAddressColumnName;
                        $scope.scopeModel.atTypeIdColumnName = payload.AtTypeIdColumnName;
                        $scope.scopeModel.smpOwnerIdColumnName = payload.SmpOwnerIdColumnName;
                        $scope.scopeModel.phoneTypeColumnName = payload.PhoneTypeColumnName;
                        $scope.scopeModel.smpPhoneNumberColumnName = payload.SmpPhoneNumberColumnName;
                        $scope.scopeModel.identityIdColumnName = payload.IdentityIdColumnName;
                        $scope.scopeModel.smniOwnerIdColumnName = payload.SmniOwnerIdColumnName;
                        $scope.scopeModel.smniValueColumnName = payload.SmniValueColumnName;


                       

                        accountTypeSelectorPayload = {
                            filter: {
                                AccountBEDefinitionId: accountBEDefinitionId
                            },
                            selectedIds: payload.AccountTypeId
                        };

                        statusDefinitionSelectorPayload = {
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.AccountBEStatusDefinitionFilter, Retail.BusinessEntity.Business",
                                    AccountBEDefinitionId: payload.AccountBEDefinitionId

                                }]
                            },
                            selectedIds: payload.InitialStatusId
                        };

                        businessEntityDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadAccountTypeSelector(accountTypeSelectorPayload));
                        promises.push(loadStatusDefinitionSelector(statusDefinitionSelectorPayload));
                    }

                    promises.push(loadCompanyProfileDefinitionSelector());
                    promises.push(loadFinancialDefinitionSelector());
                    promises.push(loadBranchInfoPartDefinitionSelector());

                    function loadAccountDefinitionSelectorLoad() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        accountDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };

                            if (payload != undefined) {
                                selectorPayload.selectedIds = payload.AccountBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(accountDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    };

                    function loadCompanyProfileDefinitionSelector() {
                        var companyProfileSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        companyProfileDefinitionSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    AccountPartDefinitionIds: getAccountPartDefinitionIds('1AFF2BF7-1F15-4E0B-ACCF-457EDF36A342')
                                },
                                selectedIds: payload != undefined ? payload.CompanyProfilePartDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(companyProfileDefinitionSelectorAPI, selectorPayload, companyProfileSelectorLoadDeferred);
                        });
                        return companyProfileSelectorLoadDeferred.promise;
                    };

                    function loadFinancialDefinitionSelector() {
                        var financialSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        financialDefinitionSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    AccountPartDefinitionIds: getAccountPartDefinitionIds('BA425FA1-13CA-4F44-883A-2A12B7E3F988')
                                },
                                selectedIds: payload != undefined ? payload.FinancialPartDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(financialDefinitionSelectorAPI, selectorPayload, financialSelectorLoadDeferred);
                        });
                        return financialSelectorLoadDeferred.promise;
                    };

                    function loadBranchInfoPartDefinitionSelector() {
                        var brancheInfoSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        branchInfoDefinitionSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    AccountPartDefinitionIds: getAccountPartDefinitionIds('F82D421E-443E-418C-8CC6-E10597A46442')
                                },
                                selectedIds: payload != undefined ? payload.BranchInfoPartDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(branchInfoDefinitionSelectorAPI, selectorPayload, brancheInfoSelectorLoadDeferred);
                        });
                        return brancheInfoSelectorLoadDeferred.promise;
                    };
                    
                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        businessEntityDefinitionSelectionChangedDeferred = undefined;
                    });
                };


                api.getData = function () {
                    var data = {
                        $type: "Retail.MultiNet.Business.Convertors.BranchConvertor, Retail.MultiNet.Business",
                        Name: "MultiNet Branch Convertor",
                        AccountBEDefinitionId: accountDefinitionSelectorApi.getSelectedIds(),
                        InitialStatusId: statusDefinitionSelectorAPI.getSelectedIds(),
                        AccountTypeId: accountTypeSelectorAPI.getSelectedIds(),
                        CompanyProfilePartDefinitionId: companyProfileDefinitionSelectorAPI.getSelectedIds(),
                        FinancialPartDefinitionId: financialDefinitionSelectorAPI.getSelectedIds(),
                        BranchInfoPartDefinitionId: branchInfoDefinitionSelectorAPI.getSelectedIds(),
                        BranchIdColumnName: $scope.scopeModel.branchColumnName,
                        CompanyIdColumnName: $scope.scopeModel.companyColumnName,
                        AccountHolderColumnName: $scope.scopeModel.accountHolderColumnName,
                        AccountStateColumnName: $scope.scopeModel.accountStateColumnName,
                        InsertDateColumnName: $scope.scopeModel.insertDateColumnName,
                        BranchCodeColumnName: $scope.scopeModel.branchCodeColumnName,
                        ContractRefNoColumnName: $scope.scopeModel.contractRefNoColumnName,
                        CurrencyIdColumnName: $scope.scopeModel.currencyIdColumnName,
                        EmailColumnName: $scope.scopeModel.emailColumnName,
                        ActivationDateColumnName: $scope.scopeModel.activationDateColumnName,
                        SmaOwnerIdColumnName: $scope.scopeModel.smaOwnerIdColumnName,
                        SmaAddressColumnName: $scope.scopeModel.smaAddressColumnName,
                        AtTypeIdColumnName: $scope.scopeModel.atTypeIdColumnName,
                        SmpOwnerIdColumnName: $scope.scopeModel.smpOwnerIdColumnName,
                        PhoneTypeColumnName: $scope.scopeModel.phoneTypeColumnName,
                        SmpPhoneNumberColumnName: $scope.scopeModel.smpPhoneNumberColumnName,
                        IdentityIdColumnName: $scope.scopeModel.identityIdColumnName,
                        SmniOwnerIdColumnName: $scope.scopeModel.smniOwnerIdColumnName,
                        SmniValueColumnName: $scope.scopeModel.smniValueColumnName


                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function loadAccountTypeSelector(accountTypeSelectorPayload) {
                var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([accountTypeSelectorReadyDeferred.promise, businessEntityDefinitionSelectionChangedDeferred.promise]).then(function () {

                    VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, accountTypeSelectorPayload, accountTypeSelectorLoadDeferred);
                });

                return accountTypeSelectorLoadDeferred.promise;
            }

            function loadStatusDefinitionSelector(accountTypeSelectorPayload) {
                var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([statusDefinitionSelectorReadyDeferred.promise, businessEntityDefinitionSelectionChangedDeferred.promise]).then(function () {

                    VRUIUtilsService.callDirectiveLoad(statusDefinitionSelectorAPI, accountTypeSelectorPayload, accountTypeSelectorLoadDeferred);
                });

                return accountTypeSelectorLoadDeferred.promise;
            }

            function getAccountPartDefinitionIds(id) {
                var partDefinitionIds = [];
                partDefinitionIds.push(id);
                return partDefinitionIds;
            }
        }
        return directiveDefinitionObject;
    }]);