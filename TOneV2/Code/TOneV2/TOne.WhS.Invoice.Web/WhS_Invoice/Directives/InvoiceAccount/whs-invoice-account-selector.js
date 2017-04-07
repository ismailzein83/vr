'use strict';

app.directive('whsInvoiceAccountSelector', ['WhS_Invoice_InvoiceAccountAPIService', 'WhS_Invoice_InvoiceAccountCarrierTypeEnum', 'WhS_Invoice_InvoiceAccountEffectiveStatusEnum', 'UtilsService', 'VRUIUtilsService', function (WhS_Invoice_InvoiceAccountAPIService, WhS_Invoice_InvoiceAccountCarrierTypeEnum, WhS_Invoice_InvoiceAccountEffectiveStatusEnum, UtilsService, VRUIUtilsService) {
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

        var invoiceTypeId;
        var carrierTypeSelectorAPI;
        var carrierTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var carrierTypeSelectedPromiseDeferred;
        var switchValueSelectedPromiseDeferred;

        var selectedIds;
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
                if (switchValueSelectedPromiseDeferred != undefined)
                    switchValueSelectedPromiseDeferred.resolve();
                else {
                    loadInvoiceAccounts().finally(function () {
                        reloadContextFunctions();
                    });
                }
            };
            $scope.scopeModel.onCarrierTypeSelectorReady = function (api) {
                carrierTypeSelectorAPI = api;
                carrierTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onCarrierTypeChanged = function (selectedCarrierType) {
                if (carrierTypeSelectedPromiseDeferred != undefined)
                    carrierTypeSelectedPromiseDeferred.resolve();
                else
                {
                    $scope.scopeModel.accountDataTextField = (selectedCarrierType != undefined) ? 'Name' : 'Description';
                    loadInvoiceAccounts().finally(function () {
                        reloadContextFunctions();

                        if (context != undefined && context.reloadPregeneratorActions != undefined) {
                            context.reloadPregeneratorActions();
                        }
                    });
                }
            };
            $scope.scopeModel.onAccountSelectorReady = function (api) {
                accountSelectorAPI = api;
                accountSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onAccountSelected = function (selectedAccount) {
                if (selectedAccount != undefined)
                {
                    reloadContextFunctions(selectedAccount);
                }
            };

            UtilsService.waitMultiplePromises([accountSelectorReadyDeferred.promise, carrierTypeSelectorReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function reloadContextFunctions(selectedAccount)
        {
            if (context != undefined) {
                if (context.onAccountSelected != undefined) {
                    var invoiceAccountId = selectedAccount != undefined ? selectedAccount.InvoiceAccountId : undefined;
                    context.onAccountSelected(invoiceAccountId);
                }
               
                if (context.setTimeZone != undefined) {
                    var timeZoneId = selectedAccount != undefined ? selectedAccount.TimeZoneId : undefined;
                    context.setTimeZone(timeZoneId);
                }
                if (context.reloadBillingPeriod != undefined) {
                    context.reloadBillingPeriod();
                }
            }
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                carrierTypeSelectorAPI.clearDataSource();
                accountSelectorAPI.clearDataSource();

                var extendedSettings;
                if (payload != undefined) {
                    invoiceTypeId = payload.invoiceTypeId;
                    extendedSettings = payload.extendedSettings;
                    context = payload.context;
                    selectedIds = payload.selectedIds;
                    carrierTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                    switchValueSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                }

                function loadCarrierTypes() {
                    $scope.scopeModel.carrierTypes = UtilsService.getArrayEnum(WhS_Invoice_InvoiceAccountCarrierTypeEnum);
                }

                return UtilsService.waitMultipleAsyncOperations([loadCarrierTypes, loadInvoiceAccounts]);
            };

            api.getData = function () {
                return {
                    selectedIds: VRUIUtilsService.getIdSelectedIds('InvoiceAccountId', $attrs, ctrl)
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }

        function getFilter() {
            var filter = {
                InvoiceTypeId: invoiceTypeId,
                GetCurrentOnly: $scope.scopeModel.getCurrentOnly,
                CarrierType: $scope.scopeModel.selectedCarrierType != undefined ? $scope.scopeModel.selectedCarrierType.value : undefined
            };
            return filter;
        }
        function loadInvoiceAccounts() {
            var filter = getFilter();
            ctrl.datasource.length = 0;
            accountSelectorAPI.clearDataSource();

            return WhS_Invoice_InvoiceAccountAPIService.GetInvoiceAccountsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                if (response != undefined) {
                    for (var i = 0; i < response.length; i++) {
                        var account = response[i];
                        ctrl.datasource.push(account);
                    }
                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'InvoiceAccountId', $attrs, ctrl);
                    }
                    carrierTypeSelectedPromiseDeferred = undefined;
                    switchValueSelectedPromiseDeferred = undefined;
                }
            });
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
				        label="Invoice Account"\
				        datasource="accountSelectorCtrl.datasource"\
                        selectedvalues="accountSelectorCtrl.selectedvalues"\
				        datavaluefield="InvoiceAccountId"\
				        datatextfield="{{scopeModel.accountDataTextField}}"\
                        onselectionchanged="scopeModel.onAccountSelected"\
				        isrequired="accountSelectorCtrl.isrequired"\
				        hideremoveicon="accountSelectorCtrl.isrequired"\
                        ' + isMultipleSelection + '>\
                    </vr-select>\
                </vr-columns>';
    }
}]);