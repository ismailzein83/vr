'use strict';
app.directive('retailBeAccountSelector', ['Retail_BE_AccountBEAPIService', 'VRUIUtilsService', 'UtilsService','Retail_BE_AccountBEDefinitionAPIService',
    function (Retail_BE_AccountBEAPIService, VRUIUtilsService, UtilsService, Retail_BE_AccountBEDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "=",
                isdisabled: "=",
                selectedvalues: '=',
                normalColNum: '@',
                customlabel: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var accountCtor = new AccountCtor(ctrl, $scope, $attrs);
                accountCtor.initializeController();
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getAccountTemplate(attrs);
            }

        };


        function getAccountTemplate(attrs) {
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }

          
            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                   + '<vr-label>{{ctrl.fieldTitle}}</vr-label>'
                   + '<vr-select on-ready="ctrl.onSelectorReady" ng-if="ctrl.useRemoteSelector"' //Remote Selector
                   + '  selectedvalues="ctrl.selectedvalues"'
                   + '  onselectionchanged="ctrl.onselectionchanged"'
                   + '  datasource="ctrl.search"'
                   + '  datavaluefield="AccountId"'
                   + '  datatextfield="Name"'
                   + '  ' + multipleselection
                   + '  isrequired="ctrl.isrequired"'
            //       + ' entityName="ctrl.fieldTitle"'
                   + '  >'
                   + '</vr-select>'

                  + '<vr-select ng-if="!ctrl.useRemoteSelector" on-ready="ctrl.onSelectorReady"' //DataSource Selector
                   + '  selectedvalues="ctrl.selectedvalues"'
                   + '  onselectionchanged="ctrl.onselectionchanged"'
                   + '  datasource="ctrl.datasource"'
                   + '  datavaluefield="AccountId"'
                   + '  datatextfield="Name"'
                   + '  ' + multipleselection
                   + '  isrequired="ctrl.isrequired"'
            //       + ' entityName="ctrl.fieldTitle"'
                   + '  >'
                   + '</vr-select>'

                   + '</vr-columns>';
        }

        function AccountCtor(ctrl, $scope, attrs) {

            this.initializeController = initializeController;
            ctrl.useRemoteSelector = false;
            var filter = {};
            var selectorAPI;
            var accountBeDefinitionId;
            function initializeController() {
                ctrl.datasource = [];

                ctrl.fieldTitle = "Account";

                 if (attrs.ismultipleselection != undefined) {
                    ctrl.fieldTitle = "Accounts";
                }

                if (attrs.customlabel != undefined) {
                    ctrl.fieldTitle = attrs.customlabel;
                }

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(defineAPI());
                    }
                };

                ctrl.search = function (nameFilter) {
                    return GetAccountsInfo(nameFilter);
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var selectedIds;

                    if (payload != undefined) {
                        if (payload.businessEntityDefinitionId != undefined)
                            accountBeDefinitionId = payload.businessEntityDefinitionId;
                        else
                            accountBeDefinitionId = payload.AccountBEDefinitionId;

                        var loadSelectorPromise = UtilsService.createPromiseDeferred();

                        Retail_BE_AccountBEDefinitionAPIService.CheckUseRemoteSelector(accountBeDefinitionId).then(function (response) {
                            ctrl.useRemoteSelector = response;

                            if (payload.fieldTitle != undefined)
                                ctrl.fieldTitle = payload.fieldTitle;
                            selectedIds = payload.selectedIds;
                            filter = payload.filter;

                            if (payload.beFilter != undefined) {
                                if (filter == undefined)
                                    filter = {};
                                if (filter.Filters == undefined)
                                    filter.Filters = [];
                                filter.Filters.push({
                                    $type: "Retail.BusinessEntity.Business.AccountConditionAccountFilter,Retail.BusinessEntity.Business",
                                    AccountCondition: payload.beFilter
                                });
                            }

                            if (payload.beRuntimeSelectorFilter != undefined) {
                                if (filter == undefined)
                                    filter = {};
                                if (filter.Filters == undefined)
                                    filter.Filters = [];
                                filter.Filters.push({
                                    $type: "Retail.BusinessEntity.Business.AccountConditionAccountFilter,Retail.BusinessEntity.Business",
                                    AccountCondition: payload.beRuntimeSelectorFilter.AccountCondition
                                });
                            }

                            // check BEDefinitionID name
                            if (ctrl.useRemoteSelector) {
                                if (selectedIds != undefined) {
                                    var selectedAccountIds = [];
                                    if (attrs.ismultipleselection != undefined)
                                        selectedAccountIds = selectedIds;
                                    else
                                        selectedAccountIds.push(selectedIds);

                                    GetAccountsInfoByIds(attrs, ctrl, selectedAccountIds).then(function () {
                                        loadSelectorPromise.resolve();
                                    });
                                }else
                                {
                                    loadSelectorPromise.resolve();
                                }
                            } else {
                                GetAccountsInfo(null, selectedIds).then(function (response) {
                                    angular.forEach(response, function (item) {
                                        ctrl.datasource.push(item);
                                    });
                                    if (selectedIds != undefined)
                                        VRUIUtilsService.setSelectedValues(selectedIds, 'AccountId', attrs, ctrl);
                                    loadSelectorPromise.resolve();
                                });
                            }

                        }).catch(function (error) {
                            loadSelectorPromise.reject(error);
                        });
                        return loadSelectorPromise.promise;
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('AccountId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                return api;

            }

            function GetAccountsInfoByIds(attrs, ctrl, selectedIds) {
                ctrl.datasource = [];
                var filter = {
                    AccountBEDefinition: accountBeDefinitionId,
                    AccountIds: selectedIds
                };
                return Retail_BE_AccountBEAPIService.GetAccountsInfoByIds(filter).then(function (response) {
                    angular.forEach(response, function (item) {
                        ctrl.datasource.push(item);
                    });
                    if (selectedIds != undefined)
                        VRUIUtilsService.setSelectedValues(selectedIds, 'AccountId', attrs, ctrl);
                });
            }

            function GetAccountsInfo(nameFilter, selectedIds)
            {
                var serializedFilter = "";
                if (filter != undefined)
                    serializedFilter = UtilsService.serializetoJson(filter);
                return Retail_BE_AccountBEAPIService.GetAccountsInfo(accountBeDefinitionId, nameFilter, serializedFilter);
            }
        }

        return directiveDefinitionObject;

    }]);