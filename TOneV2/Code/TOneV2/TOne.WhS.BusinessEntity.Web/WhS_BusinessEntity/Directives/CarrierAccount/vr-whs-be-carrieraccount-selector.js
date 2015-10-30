'use strict';
app.directive('vrWhsBeCarrieraccountSelector', ['WhS_BE_CarrierAccountAPIService', 'UtilsService', '$compile', function (WhS_BE_CarrierAccountAPIService, UtilsService, $compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            getcustomers: "@",
            getsuppliers: "@",
            ismultipleselection: "@",
            hideselectedvaluessection: '@',
            onselectionchanged: '=',
            isrequired: '@',
            isdisabled: "=",
            selectedvalues: "=",
            hideremoveicon: "@"
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            ctrl.selectedCarrierValues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedCarrierValues = [];
            ctrl.datasource = [];

            ctrl.selectedvalues = ctrl.selectedCarrierValues;

            var ctor = new carriersCtor(ctrl, $scope, WhS_BE_CarrierAccountAPIService, $attrs);
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
            return getTemplate(attrs);
        }
    };

    function getTemplate(attrs) {
        var label;
        if (attrs.ismultipleselection != undefined) {
            label = (attrs.getcustomers != undefined) ? "Customers" : "Suppliers";
        }
        else {
            label = (attrs.getcustomers != undefined) ? "Customer" : "Supplier";
        }

        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";

        var hideselectedvaluessection = "";
        if (attrs.hideselectedvaluessection != undefined)
            hideselectedvaluessection = "hideselectedvaluessection";

        var hideremoveicon = "";
        if (attrs.hideremoveicon != undefined)
            hideremoveicon = "hideremoveicon";

        //To be added on multiple selection to add grouping functionality, the style section is to be added to the outer div
        //var groupStyle = 'style="display:inline-block;width: calc(100% - 18px);"';
        //var groupHtml = ' <span class="glyphicon glyphicon-th hand-cursor"  aria-hidden="true" ng-click="openTreePopup()"></span></div>';

        var ismultipleselection = "";
        if (attrs.ismultipleselection != undefined)
            ismultipleselection = "ismultipleselection";

        return '<div><vr-select datasource="ctrl.datasource" selectedvalues="ctrl.selectedCarrierValues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" datatextfield="Name" datavaluefield="CarrierAccountId" label="'
            + label + '" ' + required + ' ' + hideselectedvaluessection + ' entityname="' + label + '" ' + hideremoveicon + ' ' + ismultipleselection + '></vr-select></div>'
    }

    function carriersCtor(ctrl, $scope, WhS_BE_CarrierAccountAPIService, attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.loadDir = function (payload)
            {
                payload.filter.GetCustomers = attrs.getcustomers != undefined;
                payload.filter.GetSuppliers = attrs.getsuppliers != undefined;

                return WhS_BE_CarrierAccountAPIService.GetCarrierAccountInfo(angular.toJson(payload.filter)).then(function (response) {
                    angular.forEach(response, function (itm) {
                        ctrl.datasource.push(itm);
                    });

                    if (payload.selectedIds != undefined)
                    {
                        if (attrs.ismultipleselection != undefined) {
                            for (var i = 0; i < payload.selectedIds.length; i++) {
                                var selectedCarrierValue = UtilsService.getItemByVal(ctrl.datasource, payload.selectedIds[i], "CarrierAccountId");
                                if (selectedCarrierValue != null)
                                    ctrl.selectedCarrierValues.push(selectedCarrierValue);
                            }
                        } else {
                            var selectedCarrierValue = UtilsService.getItemByVal(ctrl.datasource, payload.selectedIds, "CarrierAccountId");
                            if (selectedCarrierValue != null)
                                ctrl.selectedCarrierValues = selectedCarrierValue;
                        }
                    }
                });
            }

            api.load = function () {

                var filter = {
                    GetCustomers: attrs.getcustomers != undefined,
                    GetSuppliers: attrs.getsuppliers != undefined
                }

                return WhS_BE_CarrierAccountAPIService.GetCarrierAccountInfo(angular.toJson(filter)).then(function (response) {
                    angular.forEach(response, function (itm) {
                        ctrl.datasource.push(itm);
                    });
                });
            }

            api.getSelectedIds = function () {
                if (attrs.ismultipleselection != undefined)
                    return UtilsService.getPropValuesFromArray(ctrl.selectedCarrierValues, 'CarrierAccountId');
                else if (ctrl.selectedCarrierValues != undefined)
                    return ctrl.selectedCarrierValues.CarrierAccountId;
                 
                return undefined;
            }

            api.getData = function ()
            {
                return ctrl.selectedCarrierValues;
            }

            api.setData = function (selectedIds) {
                if (attrs.ismultipleselection!=undefined) {
                    for (var i = 0; i < selectedIds.length; i++) {
                        var selectedCarrierValue = UtilsService.getItemByVal(ctrl.datasource, selectedIds[i], "CarrierAccountId");
                        if (selectedCarrierValue != null)
                            ctrl.selectedCarrierValues.push(selectedCarrierValue);
                    }
                } else {
                    var selectedCarrierValue = UtilsService.getItemByVal(ctrl.datasource, selectedIds, "CarrierAccountId");
                    if (selectedCarrierValue != null)
                        ctrl.selectedCarrierValues = selectedCarrierValue;
                }
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);

