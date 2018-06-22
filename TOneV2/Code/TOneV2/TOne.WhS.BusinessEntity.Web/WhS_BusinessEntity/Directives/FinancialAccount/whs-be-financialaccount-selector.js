'use strict';

app.directive('whsBeFinancialaccountSelector', ['WhS_BE_FinancialAccountAPIService', 'WhS_BE_FinancialAccountCarrierTypeEnum', 'UtilsService', 'VRUIUtilsService', 'WHS_BE_FinancialAccountStatusEnum', 'WhS_BE_FinancialAccountDefinitionAPIService', 'VRDateTimeService',
    function (WhS_BE_FinancialAccountAPIService, WhS_BE_FinancialAccountCarrierTypeEnum, UtilsService, VRUIUtilsService, WHS_BE_FinancialAccountStatusEnum, WhS_BE_FinancialAccountDefinitionAPIService, VRDateTimeService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                ismultipleselection: '@',
                isrequired: '=',
                onselectionchanged: '=',
                selectedvalues: '='
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

            var selectedIds;
            var accountSelectorAPI;
            var accountSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var context;
            var filter;
            var businessEntityDefinitionId;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.getCurrentOnly = true;
                $scope.scopeModel.accountDataTextField = 'Description';

                $scope.scopeModel.carrierTypes = [];
                ctrl.datasource = [];

                $scope.scopeModel.onCarrierTypeSelectorReady = function (api) {
                    carrierTypeSelectorAPI = api;
                    carrierTypeSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onCarrierTypeChanged = function (selectedCarrierType) {
                    $scope.scopeModel.accountDataTextField = (selectedCarrierType != undefined) ? 'Name' : 'Description';
                };
                $scope.scopeModel.onAccountSelectorReady = function (api) {
                    accountSelectorAPI = api;
                    accountSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onOKSearch = function (api) {
                    return loadFinancialAccounts();
                };


                $scope.scopeModel.onCancelSearch = function (api) {
                    $scope.scopeModel.selectedCarrierType = undefined;
                    return loadFinancialAccounts();
                };

                UtilsService.waitMultiplePromises([accountSelectorReadyDeferred.promise, carrierTypeSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    carrierTypeSelectorAPI.clearDataSource();
                    accountSelectorAPI.clearDataSource();


                    $scope.scopeModel.selectedCarrierType = undefined;
                    var extendedSettings;
                    if (payload != undefined) {
                        filter = payload.filter;
                        extendedSettings = payload.extendedSettings;
                        context = payload.context;
                        selectedIds = payload.selectedIds;
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                    }
                    var promises = [];

                    promises.push(loadCarrierTypesAndFinancialAccounts());

                    function loadCarrierTypesAndFinancialAccounts() {
                        return UtilsService.waitMultipleAsyncOperations([loadCarrierTypes, loadFinancialAccounts]);
                    }

                    function loadCarrierTypes() {
                        $scope.scopeModel.carrierTypes = UtilsService.getArrayEnum(WhS_BE_FinancialAccountCarrierTypeEnum);
                    }

                    return UtilsService.waitMultiplePromises(promises);
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
                if (businessEntityDefinitionId != undefined) {
                    filter.FinancialAccountDefinitionId = businessEntityDefinitionId;
                    filter.Status = WHS_BE_FinancialAccountStatusEnum.Active.value;
                  //  filter.EffectiveDate = VRDateTimeService.getNowDateTime(); removed to get effective now and in future
                }
                filter.CarrierType = $scope.scopeModel.selectedCarrierType != undefined ? $scope.scopeModel.selectedCarrierType.value : undefined;
                return filter;
            }
            function loadFinancialAccounts() {
                var filter = getFilter();
                ctrl.datasource.length = 0;
                accountSelectorAPI.clearDataSource();

                return WhS_BE_FinancialAccountAPIService.GetFinancialAccountsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                    if (response != undefined) {
                        for (var i = 0; i < response.length; i++) {
                            var account = response[i];
                            ctrl.datasource.push(account);
                        }
                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'FinancialAccountId', $attrs, ctrl);
                        }
                    }
                });
            }
        }

        function getTemplate(attributes) {
            var isMultipleSelection = (attributes.ismultipleselection != undefined) ? 'ismultipleselection="accountSelectorCtrl.ismultipleselection"' : undefined;
            var label = (attributes.ismultipleselection != undefined) ? "Carriers" : "Carrier";
            return '<vr-columns colnum="{{accountSelectorCtrl.normalColNum}}"> '
                     + '<vr-select  on-ready="scopeModel.onAccountSelectorReady" '  
                                + ' includeadvancedsearch '
                                + ' onokhandler="scopeModel.onOKSearch" '
                                + ' oncancelhandler="scopeModel.onCancelSearch" '
				                +'  label="' + label + '"'
				                +' datasource="accountSelectorCtrl.datasource"'
                                +' selectedvalues="accountSelectorCtrl.selectedvalues"'
                                +' datavaluefield="FinancialAccountId"'
                                + ' datatooltipfield="AdditionalInfo"'
                                + 'datastylefield="ColorStyle" '
				                +'datatextfield="{{scopeModel.accountDataTextField}}"'
                                +'onselectionchanged="accountSelectorCtrl.onselectionchanged"'
				                +'isrequired="accountSelectorCtrl.isrequired"'
				                +'hideremoveicon="accountSelectorCtrl.isrequired"'
                                + isMultipleSelection + '>'
                                       +'   <vr-columns colnum="12">'
                                         +'     <vr-select on-ready="scopeModel.onCarrierTypeSelectorReady"'
				                                  +'       label="Carrier Type"'
				                                  +'     datasource="scopeModel.carrierTypes"'
                                                  +'   selectedvalues="scopeModel.selectedCarrierType"'
                                                    +' onselectionchanged="scopeModel.onCarrierTypeChanged"'
				                                 +' datavaluefield="value"'
				                                 +' datatextfield="description">'
			                                     +'</vr-select>'
                                         +'</vr-columns>'
                      +'</vr-select>'
                   +'</vr-columns>';
        }

    }]);