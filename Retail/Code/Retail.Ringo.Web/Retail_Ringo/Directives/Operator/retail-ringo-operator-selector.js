'use strict';
app.directive('retailRingoOperatorSelector', ['Retail_BE_AccountBEAPIService', 'VRUIUtilsService', 'UtilsService',
    function (Retail_BE_AccountBEAPIService, VRUIUtilsService, UtilsService) {

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
                var operatorCtor = new OperatorCtor(ctrl, $scope, $attrs);
                operatorCtor.initializeController();
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getAccountTemplate(attrs);
            }

        };


        function getAccountTemplate(attrs) {
            var label = "Operator";
            var multipleselection = "";

            if (attrs.ismultipleselection != undefined) {
                label = "Operators";
                multipleselection = "ismultipleselection";
            }

            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }
            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                   + '<vr-select on-ready="ctrl.onSelectorReady"'
                   + '  selectedvalues="ctrl.selectedvalues"'
                   + '  onselectionchanged="ctrl.onselectionchanged"'
                   + '  datasource="ctrl.search"'
                   + '  datavaluefield="AccountId"'
                   + '  datatextfield="Name"'
                   + '  ' + multipleselection
                   + '  isrequired="ctrl.isrequired"'
                   + '  label="' + label + '"'
                   + ' entityName="' + label + '"'
                   + '  >'
                   + '</vr-select>'
                   + '</vr-columns>';
        }

        function OperatorCtor(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var filter = {};
            var selectorAPI;
            var accountBeDefinitionId;
            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(defineAPI());
                    }
                };

                ctrl.search = function (nameFilter) {
                    var serializedFilter = "";
                    if (filter != undefined)
                        serializedFilter = UtilsService.serializetoJson(filter);
                    return Retail_BE_AccountBEAPIService.GetAccountsInfo(accountBeDefinitionId, nameFilter, serializedFilter);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;

                    if (payload != undefined) {
                        if (payload.businessEntityDefinitionId != undefined)
                            accountBeDefinitionId = payload.businessEntityDefinitionId;
                        else
                            accountBeDefinitionId = payload.AccountBEDefinitionId;
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
                    }
                    // check BEDefinitionID name
                    if (selectedIds != undefined) {
                        var selectedAccountIds = [];
                        if (attrs.ismultipleselection != undefined)
                            selectedAccountIds = selectedIds;
                        else
                            selectedAccountIds.push(selectedIds);

                        return GetAccountsInfo(attrs, ctrl, selectedAccountIds);
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('AccountId', attrs, ctrl);
                };

                api.getSelectedValues = function () {
                    var listValues = [];
                    if (ctrl.selectedvalues.Name == undefined) {
                        ctrl.selectedvalues.forEach(function(element) {
                                listValues.push(element.Name);
                            }
                        );
                    }
                    else
                        listValues.push(ctrl.selectedvalues.Name);
                    return listValues;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                return api;
            }

            function GetAccountsInfo(attrs, ctrl, selectedIds) {
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
        }

        return directiveDefinitionObject;

    }]);