'use strict';

app.directive('partnerportalCustomeraccessRetailsubaccountsSelector', ['PartnerPortal_CustomerAccess_RetailAccountUserAPIService', 'UtilsService', 'VRUIUtilsService',
    function (PartnerPortal_CustomerAccess_RetailAccountUserAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                hideremoveicon: '@',
                normalColNum: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.selectedvalues;
                ctrl.datasource = [];
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];
                var ctor = new RetailSubAccountsSelectorCtor(ctrl, $scope, $attrs);
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
                return getAccountStatementHandlerTemplate(attrs);
            }
        };

        function getAccountStatementHandlerTemplate(attrs) {
            var multipleselection = "";
            var label = "Account";

            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
                label = "Accounts";
            }

            var hideremoveicon;
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                   + '<vr-select datatextfield="Name" datavaluefield="AccountId" isrequired="ctrl.isrequired" label="' + label +
                       '" datasource="ctrl.datasource" ' + multipleselection + ' on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged"' +
                       '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' customvalidate="ctrl.customvalidate">' +
                   '</vr-select>'
                   + '</vr-columns>';
        }

        function RetailSubAccountsSelectorCtor(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;

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

                    var selectedIds;
                    var businessEntityDefinitionId;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                    }
                    if (businessEntityDefinitionId != undefined)
                    {
                        return PartnerPortal_CustomerAccess_RetailAccountUserAPIService.GetClientChildAccountsInfo(businessEntityDefinitionId).then(function (response) {
                            ctrl.datasource.length = 0;
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    ctrl.datasource.push(response[i]);
                                }
                                if (selectedIds != undefined) {
                                    VRUIUtilsService.setSelectedValues(selectedIds, 'AccountId', $attrs, ctrl);
                                }
                            }
                        });
                    }

                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('AccountId', $attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);