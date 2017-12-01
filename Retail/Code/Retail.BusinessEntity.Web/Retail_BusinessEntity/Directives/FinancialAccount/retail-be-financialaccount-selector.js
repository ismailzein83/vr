'use strict';
app.directive('retailBeFinancialaccountSelector', ['Retail_BE_FinancialAccountAPIService', 'VRUIUtilsService',
    function (Retail_BE_FinancialAccountAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "=",
                isdisabled: "=",
                selectedvalues: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];
                var ctor = new FinancialAccountSelectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };


        function getTemplate(attrs) {
            var label = "Financial Account";
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                label = "Financial Accounts";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                       + ' <vr-select on-ready="ctrl.onSelectorReady"'
                       + '  selectedvalues="ctrl.selectedvalues"'
                       + '  onselectionchanged="ctrl.onselectionchanged"'
                       + '  datasource="ctrl.datasource"'
                       + '  datavaluefield="FinancialAccountId"'
                       + '  datatextfield="Description"'
                       + '  ' + multipleselection
                       + '  ' + hideremoveicon
                       + '  isrequired="ctrl.isrequired"'
                       + '  label="' + label + '"'
                       + ' entityName="' + label + '"'
                       + '  >'
                       + '</vr-select>'
              + '</vr-columns>';
        }

        function FinancialAccountSelectorCtor(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var selectorAPI;
            var setItemsSelected;
            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

            }

            function defineAPI() {
                var api = {};

                api.clearDataSource = function () {
                    ctrl.datasource.length = 0;
                    if (attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.length = 0;
                    else
                        ctrl.selectedvalues = undefined;
                    selectorAPI.clearDataSource();
                };

                api.load = function (payload) {
                    ctrl.datasource.length = 0;
                    var accountBEDefinitionId;
                    var selectedIds;
                    var filter;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        setItemsSelected = payload.setItemsSelected;
                    }

                    return Retail_BE_FinancialAccountAPIService.GetFinancialAccountsInfo(accountBEDefinitionId, filter).then(function (response) {
                        selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'FinancialAccountId', attrs, ctrl);
                            } else if (setItemsSelected)
                            {
                                for (var i = 0, length = ctrl.datasource.length; i < length; i++) {
                                    var item = ctrl.datasource[i];
                                    if (attrs.ismultipleselection == undefined) {
                                        if (item.IsEffectiveAndActive) {
                                            selectedIds = item.FinancialAccountId;
                                            break;
                                        }
                                    } else {
                                        if (selectedIds == undefined)
                                            selectedIds = [];
                                        selectedIds.push(item.FinancialAccountId);
                                    }
                                }
                                VRUIUtilsService.setSelectedValues(selectedIds, 'FinancialAccountId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('FinancialAccountId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }]);