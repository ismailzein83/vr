'use strict';
app.directive('vrCommonCurrencySelector', ['VRCommon_CurrencyAPIService', 'VRCommon_CurrencyService', 'UtilsService', 'VRUIUtilsService',
    function (VRCommon_CurrencyAPIService, VRCommon_CurrencyService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: "@",
                onselectitem: "=",
                ondeselectitem: "=",
                isdisabled: "=",
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];
           
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                $scope.addNewCurrency = function () {
                    var onCurrencyAdded = function (currencyObj) {
                        return getAllCurrencies($attrs,ctrl,currencyObj.CurrencyId);
                    };
                    VRCommon_CurrencyService.addCurrency(onCurrencyAdded);
                }
                

                var ctor = new currencyCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
                
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            template: function (element, attrs) {
                return getCurrencyTemplate(attrs);
            }

        };


        function getCurrencyTemplate(attrs) {

            var multipleselection = "";
            var label = "Currency";
            if (attrs.ismultipleselection != undefined)
            {
                label = "Currencies";
                multipleselection = "ismultipleselection";
            }

            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";
            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewCurrency"';

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="CurrencyId" '
            + required + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Currency" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
               + '</div>'
        }

        function currencyCtor(ctrl, $scope, attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }

                    return getAllCurrencies(attrs, ctrl, selectedIds);
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('CurrencyId', attrs, ctrl);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            
            this.initializeController = initializeController;
        }

        function getAllCurrencies(attrs, ctrl, selectedIds) {
            return VRCommon_CurrencyAPIService.GetAllCurrencies().then(function (response) {
                ctrl.datasource.length = 0;
                angular.forEach(response, function (itm) {
                    ctrl.datasource.push(itm);
                });

                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'CurrencyId', attrs, ctrl);
                }
            });
        }
        return directiveDefinitionObject;
    }]);