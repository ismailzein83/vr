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

        var carrierTypeSelectorAPI;
        var carrierTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var accountSelectorAPI;
        var accountSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var context;
        var filter;
        var accountTypeId;
        var selectedIds;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.carrierTypes = [];

            ctrl.datasource = [];
          
            $scope.scopeModel.onCarrierTypeSelectorReady = function (api) {
                carrierTypeSelectorAPI = api;
                carrierTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountSelectorReady = function (api) {
                accountSelectorAPI = api;
                accountSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountSelected = function (selectedAccount) {
                if (context != undefined && context.onAccountSelected != undefined)
                    context.onAccountSelected(selectedAccount.FinancialAccountId);
            };

            $scope.scopeModel.onOKSearch = function (api) {
                return loadFinancialAccounts();
            };

            $scope.scopeModel.onCancelSearch = function (api) {
                $scope.scopeModel.selectedCarrierType = undefined;
                return loadInvoiceAccounts();
            };

            UtilsService.waitMultiplePromises([accountSelectorReadyDeferred.promise, carrierTypeSelectorReadyDeferred.promise]).then(function () {
                defineAPI();
            });

            ctrl.fieldTitle = "Financial Account";
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                carrierTypeSelectorAPI.clearDataSource();
                accountSelectorAPI.clearDataSource();
                ctrl.datasource.length = 0;
                selectedIds = undefined;
                var extendedSettings;
                var businessEntityDefinitionId;
                if (payload != undefined) {
                    accountTypeId = payload.accountTypeId;
                    filter = payload.filter;
                    extendedSettings = payload.extendedSettings;
                    context = payload.context;
                    if( payload.fieldTitle != undefined)
                      ctrl.fieldTitle = payload.fieldTitle;
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
        function getFilter() {
            if (filter == undefined)
                filter = {};
            filter.AccountBalanceTypeId = accountTypeId;
            filter.CarrierType = $scope.scopeModel.selectedCarrierType != undefined ? $scope.scopeModel.selectedCarrierType.value : undefined;
            return filter;
        }

        function loadFinancialAccounts() {
            accountSelectorAPI.clearDataSource();
            return WhS_AccountBalance_FinancialAccountAPIService.GetFinancialAccountsInfo(UtilsService.serializetoJson(getFilter())).then(function (response) {
                if (response != undefined) {
                    for (var i = 0; i < response.length; i++) {
                        var account = response[i];
                        ctrl.datasource.push(account);
                    }
                }
                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'FinancialAccountId', $attrs, ctrl);
                }
            });
        }
    }
    function getTemplate(attributes) {
        var isMultipleSelection = (attributes.ismultipleselection != undefined) ? 'ismultipleselection="accountSelectorCtrl.ismultipleselection"' : undefined;
        return '<vr-columns colnum="{{accountSelectorCtrl.normalColNum}}"> \
                    <vr-label>{{accountSelectorCtrl.fieldTitle}}</vr-label>\
                    <vr-select on-ready="scopeModel.onAccountSelectorReady" includeadvancedsearch onokhandler="scopeModel.onOKSearch" oncancelhandler="scopeModel.onCancelSearch" \
				        datasource="accountSelectorCtrl.datasource"\
                        selectedvalues="accountSelectorCtrl.selectedvalues"\
				        datavaluefield="FinancialAccountId"\
				        datatextfield="Description"\
                        onselectitem="scopeModel.onAccountSelected"\
				        isrequired="accountSelectorCtrl.isrequired"\
				        hideremoveicon="accountSelectorCtrl.isrequired"\
                        ' + isMultipleSelection + '>\
                            <vr-columns colnum="12">\
                                <vr-select on-ready="scopeModel.onCarrierTypeSelectorReady"\
				                    label="Carrier Type"\
				                    datasource="scopeModel.carrierTypes"\
                                    selectedvalues="scopeModel.selectedCarrierType"\
                                    onselectionchanged="scopeModel.onCarrierTypeChanged"\
				                    datavaluefield="value"\
				                    datatextfield="description">\
			                    </vr-select>\
                            </vr-columns>\
                    </vr-select>\
                </vr-columns>';
    }
}]);