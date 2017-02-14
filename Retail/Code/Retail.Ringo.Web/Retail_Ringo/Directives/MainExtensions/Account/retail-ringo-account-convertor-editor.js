'use strict';

app.directive('retailRingoAccountConvertorEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailBeAccountConvertorEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Ringo/Directives/MainExtensions/Account/Templates/AccountConvertorEditor.html"
        };

        function retailBeAccountConvertorEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var accountTypeId;
            var statusDefinitionId;

            var accountDefinitionSelectorApi;
            var accountDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var businessEntityDefinitionSelectionChangedDeferred;

            var statusDefinitionSelectorAPI;
            var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var financialDefinitionSelectorAPI;
            var financialDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var personalInfoDefinitionSelectorAPI;
            var personalInfoDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var activationDefinitionSelectorAPI;
            var activationDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var otherDefinitionSelectorAPI;
            var otherDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var residentialDefinitionSelectorAPI;
            var residentialDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var dealersDefinitionSelectorAPI;
            var dealersDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var accountTypeSelectorAPI;
            var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var agentDefinitionSelectorApi;
            var agentDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var posDefinitionSelectorApi;
            var posDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var distributorDefinitionSelectorApi;
            var distributorDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};

            $scope.scopeModel.onOtherDefinitionSelectorReady = function (api) {
                otherDefinitionSelectorAPI = api;
                otherDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onFinancialDefinitionSelectorReady = function (api) {
                financialDefinitionSelectorAPI = api;
                financialDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onPersonalInfoDefinitionSelectorReady = function (api) {
                personalInfoDefinitionSelectorAPI = api;
                personalInfoDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onActivationProfileDefinitionSelectorReady = function (api) {
                activationDefinitionSelectorAPI = api;
                activationDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onResidentialInfoDefinitionSelectorReady = function (api) {
                residentialDefinitionSelectorAPI = api;
                residentialDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onDealersDefinitionSelectorReady = function (api) {
                dealersDefinitionSelectorAPI = api;
                dealersDefinitionSelectorReadyDeferred.resolve();
            };

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

            $scope.scopeModel.onAgentDefinitionSelectorReady = function (api) {
                agentDefinitionSelectorApi = api;
                agentDefinitionSelectorPromiseDeferred.resolve();
            };

            $scope.scopeModel.onPosDefinitionSelectorReady = function (api) {
                posDefinitionSelectorApi = api;
                posDefinitionSelectorPromiseDeferred.resolve();
            };

            $scope.scopeModel.onDistributorDefinitionSelectorReady = function (api) {
                distributorDefinitionSelectorApi = api;
                distributorDefinitionSelectorPromiseDeferred.resolve();
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

                    promises.push(getAccountDefinitionSelectorLoad());

                    if (payload != undefined) {
                        accountTypeId = payload.AccountTypeId;
                        statusDefinitionId = payload.InitialStatusId;
                        accountBEDefinitionId = payload.AccountBEDefinitionId;

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

                    promises.push(loadFinancialDefinitionSelector());
                    promises.push(loadOtherDefinitionSelector());
                    promises.push(loadPersonalDefinitionSelector());
                    promises.push(loadActivationDefinitionSelector());
                    promises.push(loadResidentialDefinitionSelector());
                    promises.push(loadDealersDefinitionSelector());

                    var agentDefinitionSelectorLoadPromise = getAgentDefinitionSelectorLoadPromise();
                    promises.push(agentDefinitionSelectorLoadPromise);

                    var posDefinitionSelectorLoadPromise = getPosDefinitionSelectorLoadPromise();
                    promises.push(posDefinitionSelectorLoadPromise);

                    var distributorDefinitionSelectorLoadPromise = getDistributorDefinitionSelectorLoadPromise();
                    promises.push(distributorDefinitionSelectorLoadPromise);

                    function getAccountDefinitionSelectorLoad() {
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

                    function getAgentDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        agentDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };

                            if (payload != undefined) {
                                selectorPayload.selectedIds = payload.AgentBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(agentDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    };

                    function getPosDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        posDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };

                            if (payload != undefined) {
                                selectorPayload.selectedIds = payload.PosBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(posDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    };

                    function getDistributorDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        distributorDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };

                            if (payload != undefined) {
                                selectorPayload.selectedIds = payload.DistributorBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(distributorDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
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

                    function loadOtherDefinitionSelector() {
                        var selectorLoadDeferred = UtilsService.createPromiseDeferred();
                        otherDefinitionSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    AccountPartDefinitionIds: getAccountPartDefinitionIds('D64C95FC-5E4B-46B7-95CD-77082F91B07F')
                                },
                                selectedIds: payload != undefined ? payload.OtherPartDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(otherDefinitionSelectorAPI, selectorPayload, selectorLoadDeferred);
                        });
                        return selectorLoadDeferred.promise;
                    };

                    function loadPersonalDefinitionSelector() {
                        var selectorLoadDeferred = UtilsService.createPromiseDeferred();
                        personalInfoDefinitionSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    AccountPartDefinitionIds: getAccountPartDefinitionIds('3900317C-B982-4D8B-BD0D-01215AC1F3D9')
                                },
                                selectedIds: payload != undefined ? payload.PersonalInfoPartDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(personalInfoDefinitionSelectorAPI, selectorPayload, selectorLoadDeferred);
                        });
                        return selectorLoadDeferred.promise;
                    };

                    function loadActivationDefinitionSelector() {
                        var selectorLoadDeferred = UtilsService.createPromiseDeferred();
                        activationDefinitionSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    AccountPartDefinitionIds: getAccountPartDefinitionIds('A982BC4B-89B9-4A84-ABAE-78B1D1C37941')
                                },
                                selectedIds: payload != undefined ? payload.ActivationPartDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(activationDefinitionSelectorAPI, selectorPayload, selectorLoadDeferred);
                        });
                        return selectorLoadDeferred.promise;
                    };

                    function loadResidentialDefinitionSelector() {
                        var selectorLoadDeferred = UtilsService.createPromiseDeferred();
                        residentialDefinitionSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    AccountPartDefinitionIds: getAccountPartDefinitionIds('747D0C68-A508-4AA3-8D02-0D3CDFE72149')
                                },
                                selectedIds: payload != undefined ? payload.ResidentialProfilePartDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(residentialDefinitionSelectorAPI, selectorPayload, selectorLoadDeferred);
                        });
                        return selectorLoadDeferred.promise;
                    };

                    function loadDealersDefinitionSelector() {
                        var selectorLoadDeferred = UtilsService.createPromiseDeferred();
                        dealersDefinitionSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    AccountPartDefinitionIds: getAccountPartDefinitionIds('80B1AFC2-3222-41D5-84B6-7004838BFBA9')
                                },
                                selectedIds: payload != undefined ? payload.DealersPartDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(dealersDefinitionSelectorAPI, selectorPayload, selectorLoadDeferred);
                        });
                        return selectorLoadDeferred.promise;
                    };

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        businessEntityDefinitionSelectionChangedDeferred = undefined;
                    });
                };

                api.getData = function () {
                    var data = {
                        $type: "Retail.Ringo.MainExtensions.AccountConvertor, Retail.Ringo.MainExtensions",
                        Name: "Ringo Account Convertor",
                        AccountBEDefinitionId: accountDefinitionSelectorApi.getSelectedIds(),
                        InitialStatusId: statusDefinitionSelectorAPI.getSelectedIds(),
                        AccountTypeId: accountTypeSelectorAPI.getSelectedIds(),
                        DistributorBEDefinitionId: distributorDefinitionSelectorApi.getSelectedIds(),
                        AgentBEDefinitionId: agentDefinitionSelectorApi.getSelectedIds(),
                        PosBEDefinitionId: posDefinitionSelectorApi.getSelectedIds(),
                        ActivationPartDefinitionId: activationDefinitionSelectorAPI.getSelectedIds(),
                        OtherPartDefinitionId: otherDefinitionSelectorAPI.getSelectedIds(),
                        DealersPartDefinitionId: dealersDefinitionSelectorAPI.getSelectedIds(),
                        FinancialPartDefinitionId: financialDefinitionSelectorAPI.getSelectedIds(),
                        ResidentialProfilePartDefinitionId: residentialDefinitionSelectorAPI.getSelectedIds(),
                        PersonalInfoPartDefinitionId: personalInfoDefinitionSelectorAPI.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getAccountPartDefinitionIds(id) {
                var partDefinitionIds = [];
                partDefinitionIds.push(id);
                return partDefinitionIds;
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
        }

        return directiveDefinitionObject;
    }]);