"use strict";

app.directive("vrGenericdataGenericfinancialaccountConfiguration", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_GenericData_GenericBEDefinitionAPIService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericFinancialAccountDefinitionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericFinanialAccount/Templates/GenericFinancialAccountDefinition.html'
        };

        function GenericFinancialAccountDefinitionCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var dataRecordTypeId;
            var beDefinitionId;

            var beDefinitionSelectorAPI;
            var beDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var financialAccountFieldSelectorAPI;
            var financialAccountFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var selectedFinancialAccount;

            var accountNameFieldSelectorAPI;
            var accountNameFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var selectedAccountName;

            var statusNameFieldSelectorAPI;
            var statusNameFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var selectedStatusName;

            var bedFieldSelectorAPI;
            var bedFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var selectedBED;

            var eedFieldSelectorAPI;
            var eedFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var selectedEED;

            var currencyFieldSelectorAPI;
            var currencyFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var selectedCurrency;

            var onBusinessEntityDefinitionSelectionChangePromiseDeferred;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBEDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorAPI = api;
                    beDefinitionSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onFinancialAccountFieldSelectorReady = function (api) {
                    financialAccountFieldSelectorAPI = api;
                    financialAccountFieldSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onAccountNameFieldSelectorReady = function (api) {
                    accountNameFieldSelectorAPI = api;
                    accountNameFieldSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onStatusNameFieldSelectorReady = function (api) {
                    statusNameFieldSelectorAPI = api;
                    statusNameFieldSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onBEDFieldSelectorReady = function (api) {
                    bedFieldSelectorAPI = api;
                    bedFieldSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onEEDFieldSelectorReady = function (api) {
                    eedFieldSelectorAPI = api;
                    eedFieldSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onCurrencyFieldSelectorReady = function (api) {
                    currencyFieldSelectorAPI = api;
                    currencyFieldSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onBusinessEntityDefinitionSelectionChange = function (beDefinition) {
                    if (beDefinition != undefined) {
                        if (onBusinessEntityDefinitionSelectionChangePromiseDeferred != undefined) {
                            onBusinessEntityDefinitionSelectionChangePromiseDeferred.resolve();
                        }
                        else {
                            dataRecordTypeId = undefined;
                            beDefinitionId = beDefinition.BusinessEntityDefinitionId;
                            if (beDefinitionId != undefined) {
                                VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(beDefinitionId).then(function (response) {
                                    dataRecordTypeId = response.DataRecordTypeId;

                                    var financialAccountPayload = {
                                        dataRecordTypeId: dataRecordTypeId
                                    };
                                    var financialAccountSetLoader = function (value) { $scope.scopeModel.isFinancialAccountLoading = value; };
                                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, financialAccountFieldSelectorAPI, financialAccountPayload, financialAccountSetLoader);

                                    var accountNamePayload = {
                                        dataRecordTypeId: dataRecordTypeId
                                    };
                                    var accountNameSetLoader = function (value) { $scope.scopeModel.isAccountNameLoading = value; };
                                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, accountNameFieldSelectorAPI, accountNamePayload, accountNameSetLoader);

                                    var statusNamePayload = {
                                        dataRecordTypeId: dataRecordTypeId
                                    };
                                    var statusNameSetLoader = function (value) { $scope.scopeModel.isStatusNameLoading = value; };
                                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, statusNameFieldSelectorAPI, statusNamePayload, statusNameSetLoader);

                                    var bedPayload = {
                                        dataRecordTypeId: dataRecordTypeId
                                    };
                                    var bedAccountSetLoader = function (value) { $scope.scopeModel.isBEDLoading = value; };
                                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, bedFieldSelectorAPI, bedPayload, bedAccountSetLoader);

                                    var eedPayload = {
                                        dataRecordTypeId: dataRecordTypeId
                                    };
                                    var eedAccountSetLoader = function (value) { $scope.scopeModel.isEEDLoading = value; };
                                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, eedFieldSelectorAPI, financialAccountPayload, financialAccountSetLoader);

                                    var currencyPayload = {
                                        dataRecordTypeId: dataRecordTypeId
                                    };
                                    var currencySetLoader = function (value) { $scope.scopeModel.isCurrencyLoading = value; };
                                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, currencyFieldSelectorAPI, currencyPayload, currencySetLoader);
                                });
                            }
                        }
                    }
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {

                        onBusinessEntityDefinitionSelectionChangePromiseDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionId = payload.FinancialAccountBEDefinitionId;
                        selectedFinancialAccount = payload.FinancialAccountIdFieldName;
                        selectedAccountName = payload.AccountNameFieldName;
                        selectedStatusName = payload.StatusIdFieldName;
                        selectedBED = payload.BEDFieldName;
                        selectedEED = payload.EEDFieldName;
                        selectedCurrency = payload.CurrencyIdFieldName;
                    }

                    var rootPromiseNode = {
                        promises: [loadBEDefinitionSelector(), getDataRecordTypeId()],
                        getChildNode: function () {

                            var selectorsPromises = [];
                            selectorsPromises.push(loadEEDFieldSelectorDirective());
                            selectorsPromises.push(loadBEDFieldSelectorDirective());
                            selectorsPromises.push(loadStatusNameFieldSelectorDirective());
                            selectorsPromises.push(loadAccountNameFieldSelectorDirective());
                            selectorsPromises.push(loadFinancialAccountFieldSelectorDirective());
                            selectorsPromises.push(loadCurrencyFieldSelectorDirective());

                            return {
                                promises: selectorsPromises,
                                getChildNode: function () {
                                    onBusinessEntityDefinitionSelectionChangePromiseDeferred = undefined;
                                    return {
                                        promises: []
                                    };
                                }
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.GenericData.Entities.GenericFinancialAccountConfiguration,Vanrise.GenericData.Entities",
                        FinancialAccountBEDefinitionId: beDefinitionSelectorAPI.getSelectedIds(),
                        FinancialAccountIdFieldName: financialAccountFieldSelectorAPI.getSelectedIds(),
                        AccountNameFieldName: accountNameFieldSelectorAPI.getSelectedIds(),
                        StatusIdFieldName: statusNameFieldSelectorAPI.getSelectedIds(),
                        BEDFieldName: bedFieldSelectorAPI.getSelectedIds(),
                        EEDFieldName: eedFieldSelectorAPI.getSelectedIds(),
                        CurrencyIdFieldName: currencyFieldSelectorAPI.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function getDataRecordTypeId() {
                var dataRecordTypeGetPromise = UtilsService.createPromiseDeferred();

                VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(beDefinitionId).then(function (response) {
                    dataRecordTypeId = response.DataRecordTypeId;

                    return dataRecordTypeGetPromise.resolve();

                });
                return dataRecordTypeGetPromise.promise;
            }

            function loadEEDFieldSelectorDirective() {
                var eedFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                eedFieldSelectorReadyDeferred.promise.then(function () {
                    var eedFieldSelectorPayload = {
                        dataRecordTypeId: dataRecordTypeId
                    };

                    if (selectedEED != undefined) {
                        eedFieldSelectorPayload.selectedIds = selectedEED;
                    }
                    VRUIUtilsService.callDirectiveLoad(eedFieldSelectorAPI, eedFieldSelectorPayload, eedFieldSelectorLoadDeferred);
                });


                return eedFieldSelectorLoadDeferred.promise;
            }

            function loadBEDFieldSelectorDirective() {
                var bedFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                bedFieldSelectorReadyDeferred.promise.then(function () {
                    var bedFieldSelectorPayload = {
                        dataRecordTypeId: dataRecordTypeId
                    };

                    if (selectedBED != undefined) {
                        bedFieldSelectorPayload.selectedIds = selectedBED;
                    }
                    VRUIUtilsService.callDirectiveLoad(bedFieldSelectorAPI, bedFieldSelectorPayload, bedFieldSelectorLoadDeferred);
                });


                return bedFieldSelectorLoadDeferred.promise;
            }

            function loadStatusNameFieldSelectorDirective() {
                var statusNameFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                statusNameFieldSelectorReadyDeferred.promise.then(function () {
                    var statusNameFieldSelectorPayload = {
                        dataRecordTypeId: dataRecordTypeId
                    };

                    if (selectedStatusName != undefined) {
                        statusNameFieldSelectorPayload.selectedIds = selectedStatusName;
                    }
                    VRUIUtilsService.callDirectiveLoad(statusNameFieldSelectorAPI, statusNameFieldSelectorPayload, statusNameFieldSelectorLoadDeferred);
                });


                return statusNameFieldSelectorLoadDeferred.promise;
            }

            function loadAccountNameFieldSelectorDirective() {
                var accountNameFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                accountNameFieldSelectorReadyDeferred.promise.then(function () {
                    var accountNameFieldSelectorPayload = {
                        dataRecordTypeId: dataRecordTypeId
                    };

                    if (selectedAccountName != undefined) {
                        accountNameFieldSelectorPayload.selectedIds = selectedAccountName;
                    }
                    VRUIUtilsService.callDirectiveLoad(accountNameFieldSelectorAPI, accountNameFieldSelectorPayload, accountNameFieldSelectorLoadDeferred);
                });


                return accountNameFieldSelectorLoadDeferred.promise;
            }

            function loadFinancialAccountFieldSelectorDirective() {
                var financialAccountFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                financialAccountFieldSelectorReadyDeferred.promise.then(function () {
                    var financialAccountFieldSelectorPayload = {
                        dataRecordTypeId: dataRecordTypeId
                    };

                    if (selectedFinancialAccount != undefined) {
                        financialAccountFieldSelectorPayload.selectedIds = selectedFinancialAccount;
                    }
                    VRUIUtilsService.callDirectiveLoad(financialAccountFieldSelectorAPI, financialAccountFieldSelectorPayload, financialAccountFieldSelectorLoadDeferred);
                });

                return financialAccountFieldSelectorLoadDeferred.promise;
            }

            function loadBEDefinitionSelector() {
                var beDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                beDefinitionSelectorReadyDeferred.promise.then(function () {
                    var beDefinitionSelectorPayload;
                    if (beDefinitionId != undefined)
                        beDefinitionSelectorPayload = {
                            selectedIds: beDefinitionId
                        };
                    VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, beDefinitionSelectorPayload, beDefinitionSelectorLoadDeferred);
                });
                return beDefinitionSelectorLoadDeferred.promise;
            }

            function loadCurrencyFieldSelectorDirective() {
                var currencyFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                currencyFieldSelectorReadyDeferred.promise.then(function () {
                    var currencyFieldSelectorPayload = {
                        dataRecordTypeId: dataRecordTypeId
                    };

                    if (selectedCurrency != undefined) {
                        currencyFieldSelectorPayload.selectedIds = selectedCurrency;
                    }
                    VRUIUtilsService.callDirectiveLoad(currencyFieldSelectorAPI, currencyFieldSelectorPayload, currencyFieldSelectorLoadDeferred);
                });
                return currencyFieldSelectorLoadDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);