'use strict';

app.directive('whsAccountbalanceAccountSelector', ['WhS_AccountBalance_FinancialAccountAPIService', 'VR_GenericData_BusinessEntityDefinitionAPIService', 'WhS_AccountBalance_FinancialAccountCarrierTypeEnum', 'WhS_AccountBalance_FinancialAccountEffectiveStatusEnum', 'UtilsService', 'VRUIUtilsService', function (WhS_AccountBalance_FinancialAccountAPIService, VR_GenericData_BusinessEntityDefinitionAPIService, WhS_AccountBalance_FinancialAccountCarrierTypeEnum, WhS_AccountBalance_FinancialAccountEffectiveStatusEnum, UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            ismultipleselection: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

            var accountSelector = new AccountSelector($scope, ctrl, $attrs);
            accountSelector.initializeController();
        },
        controllerAs: "accountSelectorCtrl",
        bindToController: true,
        template: function (element, attributes) {
            return getTemplate(attributes);
        }
    };

    function AccountSelector($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var allAccounts = [];

        var carrierTypeSelectorAPI;
        var carrierTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var accountSelectorAPI;
        var accountSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var context;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.getCurrentOnly = true;
            $scope.scopeModel.accountDataTextField = 'Description';

            $scope.scopeModel.carrierTypes = [];
            ctrl.datasource = [];

            $scope.scopeModel.onSwitchValueChanged = function () {
                filterAccounts();
            };
            $scope.scopeModel.onCarrierTypeSelectorReady = function (api) {
                carrierTypeSelectorAPI = api;
                carrierTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onCarrierTypeChanged = function (selectedCarrierType) {
                $scope.scopeModel.accountDataTextField = (selectedCarrierType != undefined) ? 'Name' : 'Description';
                filterAccounts();
            };
            $scope.scopeModel.onAccountSelectorReady = function (api) {
                accountSelectorAPI = api;
                accountSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onAccountSelected = function (selectedAccount) {
                if (context != undefined && context.onAccountSelected != undefined)
                    context.onAccountSelected(selectedAccount.FinancialAccountId);
            };

            UtilsService.waitMultiplePromises([accountSelectorReadyDeferred.promise, carrierTypeSelectorReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                carrierTypeSelectorAPI.clearDataSource();
                allAccounts.length = 0;
                accountSelectorAPI.clearDataSource();

                var accountTypeId;
                var extendedSettings;
                var businessEntityDefinitionId;
                var selectedIds;
                if (payload != undefined) {
                    accountTypeId = payload.accountTypeId;
                    extendedSettings = payload.extendedSettings;
                    context = payload.context;
                    businessEntityDefinitionId = payload.businessEntityDefinitionId;
                    selectedIds = payload.selectedIds;
                }

                var promises = [];

                if (businessEntityDefinitionId != undefined) {
                    var getBusinessEntityDefinitionPromise = getBusinessEntityDefinition();
                    promises.push(getBusinessEntityDefinitionPromise);

                    var loadCarrierTypesAndFinancialAccountsDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadCarrierTypesAndFinancialAccountsDeferred.promise);

                    getBusinessEntityDefinitionPromise.then(function () {
                        loadCarrierTypesAndFinancialAccounts().then(function () {
                            loadCarrierTypesAndFinancialAccountsDeferred.resolve();
                        }).catch(function (error) {
                            loadCarrierTypesAndFinancialAccountsDeferred.reject(error);
                        });
                    });
                }
                else {
                    var loadCarrierTypesAndFinancialAccountsPromise = loadCarrierTypesAndFinancialAccounts();
                    promises.push(loadCarrierTypesAndFinancialAccountsPromise);
                }

                function getBusinessEntityDefinition() {
                    return VR_GenericData_BusinessEntityDefinitionAPIService.GetBusinessEntityDefinition(businessEntityDefinitionId).then(function (response) {
                        if (response != undefined && response.Settings != null) {
                            accountTypeId = response.Settings.AccountTypeId;
                        }
                    });
                }
                function loadCarrierTypesAndFinancialAccounts() {
                    return UtilsService.waitMultipleAsyncOperations([loadCarrierTypes, loadFinancialAccounts]);
                }
                function loadCarrierTypes() {
                    $scope.scopeModel.carrierTypes = UtilsService.getArrayEnum(WhS_AccountBalance_FinancialAccountCarrierTypeEnum);
                }
                function loadFinancialAccounts() {
                    var filter = {
                        AccountBalanceTypeId: accountTypeId
                    };
                    return WhS_AccountBalance_FinancialAccountAPIService.GetFinancialAccountsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        if (response != undefined) {
                            for (var i = 0; i < response.length; i++) {
                                var account = response[i];
                                allAccounts.push(account);
                                ctrl.datasource.push(account);
                            }
                        }
                        filterAccounts();
                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'FinancialAccountId', $attrs, ctrl);
                        }
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    selectedIds: VRUIUtilsService.getIdSelectedIds('FinancialAccountId', $attrs, ctrl)
                };
            };
            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('FinancialAccountId', $attrs, ctrl);
            };
            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }

        function filterAccounts() {
            var getCurrentOnly = $scope.scopeModel.getCurrentOnly;
            var carrierType = $scope.scopeModel.selectedCarrierType;

            ctrl.datasource.length = 0;

            if ($attrs.ismultipleselection != undefined) ctrl.selectedvalues.length = 0;
            else ctrl.selectedvalues = undefined;

            for (var i = 0; i < allAccounts.length; i++) {
                if (!effectiveStatusFilter(allAccounts[i]) || !carrierTypeFilter(allAccounts[i]))
                    continue;
                ctrl.datasource.push(allAccounts[i]);
            }

            function effectiveStatusFilter(targetAccount) {
                return (!getCurrentOnly || targetAccount.EffectiveStatus == WhS_AccountBalance_FinancialAccountEffectiveStatusEnum.Current.value);
            }
            function carrierTypeFilter(targetAccount) {
                return (carrierType == undefined || targetAccount.CarrierType == carrierType.value);
            }


        }
    }
    function getTemplate(attributes) {
        var isMultipleSelection = (attributes.ismultipleselection != undefined) ? 'ismultipleselection="accountSelectorCtrl.ismultipleselection"' : undefined;
        return '<vr-columns colnum="{{(accountSelectorCtrl.normalColNum / 2) | number: 0}}">\
                    <vr-switch label="Current Only" value="scopeModel.getCurrentOnly" onvaluechanged="scopeModel.onSwitchValueChanged"></vr-switch>\
                </vr-columns>\
                <vr-columns colnum="{{(accountSelectorCtrl.normalColNum / 2) | number: 0}}">\
                    <vr-select on-ready="scopeModel.onCarrierTypeSelectorReady"\
				        label="Carrier Type"\
				        datasource="scopeModel.carrierTypes"\
                        selectedvalues="scopeModel.selectedCarrierType"\
                        onselectionchanged="scopeModel.onCarrierTypeChanged"\
				        datavaluefield="value"\
				        datatextfield="description">\
			        </vr-select>\
                </vr-columns>\
                <vr-columns colnum="{{accountSelectorCtrl.normalColNum}}">\
                    <vr-select on-ready="scopeModel.onAccountSelectorReady"\
				        label="Financial Account"\
				        datasource="accountSelectorCtrl.datasource"\
                        selectedvalues="accountSelectorCtrl.selectedvalues"\
				        datavaluefield="FinancialAccountId"\
				        datatextfield="{{scopeModel.accountDataTextField}}"\
                        onselectitem="scopeModel.onAccountSelected"\
				        isrequired="accountSelectorCtrl.isrequired"\
				        hideremoveicon="accountSelectorCtrl.isrequired"\
                        ' + isMultipleSelection + '>\
                    </vr-select>\
                </vr-columns>';
    }
}]);