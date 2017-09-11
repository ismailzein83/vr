'use strict';

app.directive('retailMultinetAccountConvertorEditor', ['UtilsService', 'VRUIUtilsService', 'Retail_BE_EntityTypeEnum', 'VRNotificationService',
    function (UtilsService, VRUIUtilsService, Retail_BE_EntityTypeEnum, VRNotificationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailMultiNetAccountConvertorEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_MultiNet/Directives/Account/Templates/AccountConvertorEditor.html"
        };

        function retailMultiNetAccountConvertorEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var accountTypeId;
            var statusDefinitionId;

            var accountDefinitionSelectorApi;
            var accountDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var businessEntityDefinitionSelectionChangedDeferred;

            var accountTypeSelectorAPI;
            var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();



            var companyProfileDefinitionSelectorAPI;
            var companyProfileDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var financialDefinitionSelectorAPI;
            var financialDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var accountInfoDefinitionSelectorAPI;
            var accountInfoDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();



            $scope.scopeModel = {};

            $scope.scopeModel.onAccountDefinitionSelectorReady = function (api) {
                accountDefinitionSelectorApi = api;
                accountDefinitionSelectorPromiseDeferred.resolve();
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

            $scope.scopeModel.oAccountPartDefinitionSelectorReady = function (api) {
                accountInfoDefinitionSelectorAPI = api;
                accountInfoDefinitionSelectorReadyDeferred.resolve();
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

                    promises.push(loadAccountDefinitionSelectorLoad());

                    if (payload != undefined) {
                        accountTypeId = payload.AccountTypeId;
                        accountBEDefinitionId = payload.AccountBEDefinitionId;


                        $scope.scopeModel.accountIdColumnName = payload.AccountIdColumnName;
                        $scope.scopeModel.customerIdColumnName = payload.CustomerIdColumnName;
                        $scope.scopeModel.accountNameColumnName = payload.AccountNameColumnName;
                        $scope.scopeModel.accountStatusColumnName = payload.AccountStatusColumnName;
                        $scope.scopeModel.registrationColumnName = payload.RegistrationColumnName;
                        $scope.scopeModel.cNICColumnName = payload.CNICColumnName;
                        $scope.scopeModel.cNICExpiryDateColumnName = payload.CNICExpiryDateColumnName;
                        $scope.scopeModel.currencyIdColumnName = payload.CurrencyIdColumnName;
                        $scope.scopeModel.nTNColumnName = payload.NTNColumnName;
                        $scope.scopeModel.dueDateColumnName = payload.DueDateColumnName;
                        $scope.scopeModel.billingPeriodColumnName = payload.BillingPeriodColumnName;


                        $scope.scopeModel.countryColumnName = payload.CountryColumnName;
                        $scope.scopeModel.cityColumnName = payload.CityColumnName;
                        $scope.scopeModel.streetColumnName = payload.StreetColumnName;
                        $scope.scopeModel.townColumnName = payload.TownColumnName;
                        $scope.scopeModel.websiteColumnName = payload.WebsiteColumnName;
                        $scope.scopeModel.addressColumnName = payload.AddressColumnName;
                        $scope.scopeModel.pOBoxColumnName = payload.POBoxColumnName;


                        $scope.scopeModel.phoneColumnName = payload.PhoneColumnName;
                        $scope.scopeModel.mobileColumnName = payload.MobileColumnName;
                        $scope.scopeModel.faxColumnName = payload.FaxColumnName;


                        $scope.scopeModel.mainContactSalutaionColumnName = payload.MainContactSalutaionColumnName;
                        $scope.scopeModel.mainContactNameTitleColumnName = payload.MainContactNameTitleColumnName;
                        $scope.scopeModel.mainContactNameColumnName = payload.MainContactNameColumnName;
                        $scope.scopeModel.mainContactEmailColumnName = payload.MainContactEmailColumnName;
                        $scope.scopeModel.mainContactPhoneColumnName = payload.MainContactPhoneColumnName;
                        $scope.scopeModel.mainContactReligionColumnName = payload.MainContactReligionColumnName;


                        $scope.scopeModel.financeContactSalutaionColumnName = payload.FinanceContactSalutaionColumnName;
                        $scope.scopeModel.financeContactNameTitleColumnName = payload.FinanceContactNameTitleColumnName;
                        $scope.scopeModel.financeContactNameColumnName = payload.FinanceContactNameColumnName;
                        $scope.scopeModel.financeContactEmailColumnName = payload.FinanceContactEmailColumnName;
                        $scope.scopeModel.financeContactPhoneColumnName = payload.FinanceContactPhoneColumnName;
                        $scope.scopeModel.financeContactReligionColumnName = payload.FinanceContactReligionColumnName;



                        $scope.scopeModel.technicalContactSalutaionColumnName = payload.TechnicalContactSalutaionColumnName;
                        $scope.scopeModel.technicalContactNameTitleColumnName = payload.TechnicalContactNameTitleColumnName;
                        $scope.scopeModel.technicalContactNameColumnName = payload.TechnicalContactNameColumnName;
                        $scope.scopeModel.technicalContactEmailColumnName = payload.TechnicalContactEmailColumnName;
                        $scope.scopeModel.technicalContactPhoneColumnName = payload.TechnicalContactPhoneColumnName;
                        $scope.scopeModel.technicalContactReligionColumnName = payload.TechnicalContactReligionColumnName;

                        accountTypeSelectorPayload = {
                            filter: {
                                AccountBEDefinitionId: accountBEDefinitionId
                            },
                            selectedIds: payload.AccountTypeId
                        };

                        businessEntityDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadAccountTypeSelector(accountTypeSelectorPayload));
                    }

                    promises.push(loadCompanyProfileDefinitionSelector());
                    promises.push(loadFinancialDefinitionSelector());
                    promises.push(loadAccountInfoPartDefinitionSelector());

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

                    function loadAccountInfoPartDefinitionSelector() {
                        var accountInfoSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        accountInfoDefinitionSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    AccountPartDefinitionIds: getAccountPartDefinitionIds('F82D421E-443E-418C-8CC6-E10597A46442')
                                },
                                selectedIds: payload != undefined ? payload.AccountInfoPartDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(accountInfoDefinitionSelectorAPI, selectorPayload, accountInfoSelectorLoadDeferred);
                        });
                        return accountInfoSelectorLoadDeferred.promise;
                    };

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        businessEntityDefinitionSelectionChangedDeferred = undefined;
                    });
                };


                api.getData = function () {
                    var data = {
                        $type: "Retail.MultiNet.Business.Convertors.AccountConvertor, Retail.MultiNet.Business",
                        Name: "MultiNet Account Convertor",
                        AccountBEDefinitionId: accountDefinitionSelectorApi.getSelectedIds(),
                        AccountTypeId: accountTypeSelectorAPI.getSelectedIds(),
                        CompanyProfilePartDefinitionId: companyProfileDefinitionSelectorAPI.getSelectedIds(),
                        FinancialPartDefinitionId: financialDefinitionSelectorAPI.getSelectedIds(),
                        AccountInfoPartDefinitionId: accountInfoDefinitionSelectorAPI.getSelectedIds(),
                        AccountIdColumnName: $scope.scopeModel.accountIdColumnName,
                        CustomerIdColumnName: $scope.scopeModel.customerIdColumnName,
                        AccountNameColumnName: $scope.scopeModel.accountNameColumnName,
                        AccountStatusColumnName: $scope.scopeModel.accountStatusColumnName,
                        RegistrationColumnName: $scope.scopeModel.registrationColumnName,
                        CNICColumnName: $scope.scopeModel.cNICColumnName,
                        CNICExpiryDateColumnName: $scope.scopeModel.cNICExpiryDateColumnName,
                        CurrencyIdColumnName: $scope.scopeModel.currencyIdColumnName,
                        NTNColumnName: $scope.scopeModel.nTNColumnName,
                        DueDateColumnName: $scope.scopeModel.dueDateColumnName,
                        BillingPeriodColumnName: $scope.scopeModel.billingPeriodColumnName,
                        CountryColumnName: $scope.scopeModel.countryColumnName,
                        CityColumnName: $scope.scopeModel.cityColumnName,
                        StreetColumnName: $scope.scopeModel.streetColumnName,
                        TownColumnName: $scope.scopeModel.townColumnName,
                        WebsiteColumnName: $scope.scopeModel.websiteColumnName,
                        AddressColumnName: $scope.scopeModel.addressColumnName,
                        POBoxColumnName: $scope.scopeModel.pOBoxColumnName,
                        PhoneColumnName: $scope.scopeModel.phoneColumnName,
                        MobileColumnName: $scope.scopeModel.mobileColumnName,
                        FaxColumnName: $scope.scopeModel.faxColumnName,
                        MainContactSalutaionColumnName: $scope.scopeModel.mainContactSalutaionColumnName,
                        MainContactNameTitleColumnName: $scope.scopeModel.mainContactNameTitleColumnName,
                        MainContactNameColumnName: $scope.scopeModel.mainContactNameColumnName,
                        MainContactEmailColumnName: $scope.scopeModel.mainContactEmailColumnName,
                        MainContactPhoneColumnName: $scope.scopeModel.mainContactPhoneColumnName,
                        MainContactReligionColumnName: $scope.scopeModel.mainContactReligionColumnName,
                        FinanceContactSalutaionColumnName: $scope.scopeModel.financeContactSalutaionColumnName,
                        FinanceContactNameTitleColumnName: $scope.scopeModel.financeContactNameTitleColumnName,
                        FinanceContactNameColumnName: $scope.scopeModel.financeContactNameColumnName,
                        FinanceContactEmailColumnName: $scope.scopeModel.financeContactEmailColumnName,
                        FinanceContactPhoneColumnName: $scope.scopeModel.financeContactPhoneColumnName,
                        FinanceContactReligionColumnName: $scope.scopeModel.financeContactReligionColumnName,
                        TechnicalContactSalutaionColumnName: $scope.scopeModel.technicalContactSalutaionColumnName,
                        TechnicalContactNameTitleColumnName: $scope.scopeModel.technicalContactNameTitleColumnName,
                        TechnicalContactNameColumnName: $scope.scopeModel.technicalContactNameColumnName,
                        TechnicalContactEmailColumnName: $scope.scopeModel.technicalContactEmailColumnName,
                        TechnicalContactPhoneColumnName: $scope.scopeModel.technicalContactPhoneColumnName,
                        TechnicalContactReligionColumnName: $scope.scopeModel.technicalContactReligionColumnName
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