'use strict';
app.directive('vrCpCustomerSelector', ['CP_SupplierPricelist_CustomerManagmentAPIService', 'UtilsService', 'VRUIUtilsService',
    function (customerManagmentAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: '=',
                isdisabled: "=",
                selectedvalues: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];


                var selector = new CustomerSelector(ctrl, $scope, $attrs);
                selector.initializeController();


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
                return getCustomerInfoTemplate(attrs);
            }

        };

        function getCustomerInfoTemplate(attrs) {

            var multipleselection = "";
            var label = "Customer";
            if (attrs.ismultipleselection != undefined) {
                label = "Customers";
                multipleselection = "ismultipleselection";
            }

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="CustomerId" isrequired="ctrl.isrequired" '
                + ' label="' + label + '"  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="onSelectorReady" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="'+ label +'" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>';
        }
        function CustomerSelector(ctrl, $scope, attrs) {

            var selectorAPI;
            function initializeController() {
                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                }
            }

            function defineAPI() {
                var api = {};
                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('CustomerId', attrs, ctrl);
                }
                api.load = function (payload) {
                    var selectedIds;
                    var filter;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;

                    }
                    var serializedFilter = {};
                    if (filter != undefined)
                        serializedFilter = UtilsService.serializetoJson(filter);
                    return customerManagmentAPIService.GetCustomerInfos(serializedFilter).then(function (response) {
                        ctrl.datasource.length = 0;
                        ctrl.selectedvalues.length = 0;
                        angular.forEach(response, function (item) {
                            ctrl.datasource.push(item);
                        });

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'CustomerId', attrs, ctrl);
                        }
                    });


                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }


        return directiveDefinitionObject;
 }]);