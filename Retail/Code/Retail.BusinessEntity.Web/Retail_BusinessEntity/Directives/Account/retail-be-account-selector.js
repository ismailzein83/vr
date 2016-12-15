'use strict';
app.directive('retailBeAccountSelector', ['Retail_BE_AccountAPIService', 'VRUIUtilsService', 'UtilsService',
    function (Retail_BE_AccountAPIService, VRUIUtilsService, UtilsService) {

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
                var accountCtor = new AccountCtor(ctrl, $scope, $attrs);
                accountCtor.initializeController();
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
                return getAccountTemplate(attrs);
            }

        };


        function getAccountTemplate(attrs) {
            var label = "Account";
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                label = "Accounts";
                multipleselection = "ismultipleselection";
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

        function AccountCtor(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var filter = {};
            var selectorAPI;

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
                    return Retail_BE_AccountAPIService.GetAccountsInfo(nameFilter, serializedFilter);
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;


                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

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

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                return api;

            }

            function GetAccountsInfo(attrs, ctrl, selectedIds) {
                ctrl.datasource = [];
                return Retail_BE_AccountAPIService.GetAccountsInfoByIds(selectedIds).then(function (response) {
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