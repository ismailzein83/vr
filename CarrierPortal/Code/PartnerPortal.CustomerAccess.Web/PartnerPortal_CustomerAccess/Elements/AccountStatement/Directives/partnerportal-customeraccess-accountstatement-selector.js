'use strict';
app.directive('partnerportalCustomeraccessAccountstatementSelector', ['PartnerPortal_CustomerAccess_AccountStatementAPIService', 'UtilsService', 'VRUIUtilsService', '$filter',
    function (PartnerPortal_CustomerAccess_AccountStatementAPIService, UtilsService, VRUIUtilsService, $filter) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: "=",
                onselectitem: "=",
                ondeselectitem: "=",
                isdisabled: "=",
                hideremoveicon: '@',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new getBalanceAccount(ctrl, $scope, $attrs);
                ctor.initializeController();

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
                return getBalanceAccountTemplate(attrs);
            }

        };


        function getBalanceAccountTemplate(attrs) {

            var multipleselection = "";
            var label = "Account";
            if (attrs.ismultipleselection != undefined) {
                label = "Accounts";
                multipleselection = "ismultipleselection";
            }

            return '<vr-columns colnum="{{ctrl.normalColNum}}" ng-show="ctrl.datasource.length > 1"><vr-select ' + multipleselection + '  on-ready="ctrl.onSelectorReady" datatextfield="Name" datavaluefield="PortalBalanceAccountId"\
                label="' + label + '"  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Account" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" isrequired="ctrl.isrequired"></vr-select></vr-columns>';
        }

        function getBalanceAccount(ctrl, $scope, attrs) {
            var selectorAPI;

            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };


            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var selectedIds;
                    var viewId;


                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        viewId = payload.viewId;
                    }
                    if (viewId != undefined)
                    {
                        var getInvoiceAccountsPromise = getInvoiceAccounts();
                        promises.push(getInvoiceAccountsPromise);

                    }
                  
                 

                    function getInvoiceAccounts() {
                        return PartnerPortal_CustomerAccess_AccountStatementAPIService.GetBalanceAccounts(viewId).then(function (response) {
                            selectorAPI.clearDataSource();

                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++)
                                    ctrl.datasource.push(response[i]);

                                if (selectedIds != undefined)
                                    VRUIUtilsService.setSelectedValues(selectedIds, 'PortalBalanceAccountId', attrs, ctrl);
                                else if (ctrl.datasource.length == 1) {
                                    var defaultValue = attrs.ismultipleselection != undefined ? [ctrl.datasource[0].PortalBalanceAccountId] : ctrl.datasource[0].PortalBalanceAccountId;
                                    VRUIUtilsService.setSelectedValues(defaultValue, 'PortalBalanceAccountId', attrs, ctrl);
                                }
                            }
                        });
                    }
                 
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('PortalBalanceAccountId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);