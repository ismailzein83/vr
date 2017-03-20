'use strict';

app.directive('whsAccountbalanceAccountSelector', ['WhS_AccountBalance_FinancialAccountAPIService', 'WhS_AccountBalance_FinancialAccountCarrierTypeEnum', 'WhS_AccountBalance_FinancialAccountEffectiveStatusEnum', 'UtilsService', 'VRUIUtilsService', function (WhS_AccountBalance_FinancialAccountAPIService, WhS_AccountBalance_FinancialAccountCarrierTypeEnum, WhS_AccountBalance_FinancialAccountEffectiveStatusEnum, UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            ismultipleselection: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var accountSelectorCtrl = this;
            accountSelectorCtrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

            var accountSelector = new AccountSelector($scope, accountSelectorCtrl, $attrs);
            accountSelector.initializeController();
        },
        controllerAs: "accountSelectorCtrl",
        bindToController: true,
        template: function (element, attributes) {
            return getTemplate(attributes);
        }
    };

    function AccountSelector($scope, accountSelectorCtrl, $attrs) {

        this.initializeController = initializeController;

        var allAccounts = [];

        var carrierTypeSelectorAPI;
        var carrierTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var accountSelectorAPI;
        var accountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.getCurrentOnly = true;
            $scope.scopeModel.accountDataTextField = 'Description';

            $scope.scopeModel.carrierTypes = [];
            $scope.scopeModel.filteredAccounts = [];

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

                if (payload != undefined) {
                    accountTypeId = payload.accountTypeId;
                    extendedSettings = payload.extendedSettings;
                }

                $scope.scopeModel.carrierTypes = UtilsService.getArrayEnum(WhS_AccountBalance_FinancialAccountCarrierTypeEnum);

                return WhS_AccountBalance_FinancialAccountAPIService.GetFinancialAccountsInfo(accountTypeId).then(function (response) {
                    if (response != undefined) {
                        for (var i = 0; i < response.length; i++) {
                            var account = response[i];
                            allAccounts.push(account);
                            $scope.scopeModel.filteredAccounts.push(account);
                        }
                    }
                    filterAccounts();
                });
            };

            api.getData = function () {
                return {
                    selectedIds: VRUIUtilsService.getIdSelectedIds('FinancialAccountId', $attrs, accountSelectorCtrl)
                };
            };

            if (accountSelectorCtrl.onReady != null) {
                accountSelectorCtrl.onReady(api);
            }
        }

        function filterAccounts() {
            var getCurrentOnly = $scope.scopeModel.getCurrentOnly;
            var carrierType = $scope.scopeModel.selectedCarrierType;

            $scope.scopeModel.filteredAccounts.length = 0;

            if ($attrs.ismultipleselection != undefined) accountSelectorCtrl.selectedvalues.length = 0;
            else accountSelectorCtrl.selectedvalues = undefined;

            for (var i = 0; i < allAccounts.length; i++) {
                if (!effectiveStatusFilter(allAccounts[i]) || !carrierTypeFilter(allAccounts[i]))
                    continue;
                $scope.scopeModel.filteredAccounts.push(allAccounts[i]);
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
        return '<vr-columns colnum="{{accountSelectorCtrl.normalColNum / 2}}">\
                    <vr-switch label="Current Only" value="scopeModel.getCurrentOnly" onvaluechanged="scopeModel.onSwitchValueChanged"></vr-switch>\
                </vr-columns>\
                <vr-columns colnum="{{accountSelectorCtrl.normalColNum / 2}}">\
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
				        datasource="scopeModel.filteredAccounts"\
                        selectedvalues="accountSelectorCtrl.selectedvalues"\
				        datavaluefield="FinancialAccountId"\
				        datatextfield="{{scopeModel.accountDataTextField}}"\
				        isrequired="accountSelectorCtrl.isrequired"\
				        hideremoveicon="accountSelectorCtrl.isrequired"\
                        ' + isMultipleSelection + '>'
        '</vr-select>\
                </vr-columns>';
    }
}]);