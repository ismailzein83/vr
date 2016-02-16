﻿'use strict';
app.directive('vrDemoOperatoraccountSelector', ['Demo_OperatorAccountAPIService', 'UtilsService', 'VRUIUtilsService', '$compile',
    function (Demo_OperatorAccountAPIService, UtilsService, VRUIUtilsService, $compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            getcustomers: "@",
            getsuppliers: "@",
            ismultipleselection: "@",
            hideselectedvaluessection: '@',
            onselectionchanged: '=',
            isrequired: '=',
            isdisabled: "=",
            selectedvalues: "=",
            hideremoveicon: "@",
            onselectitem: "=",
            ondeselectitem:"="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            ctrl.datasource = [];
            var ctor = new operatorsCtor(ctrl, $scope, Demo_OperatorAccountAPIService, $attrs);
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
            label = (attrs.getcustomers != undefined && attrs.getsuppliers != undefined) ? "Operators" : label;
        }
        else {
            label = (attrs.getcustomers != undefined) ? "Customer" : "Supplier";
            label = (attrs.getcustomers != undefined && attrs.getsuppliers != undefined) ? "Operator" : label;
        }

        //var required = "";
        //console.log(attrs.isrequired);
        //if (attrs.isrequired != undefined)
        //    required = 'isrequired="' + attrs.isrequired + '"';

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

        return '<vr-select isrequired="ctrl.isrequired" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" onselectitem="ctrl.onselectitem"  ondeselectitem="ctrl.ondeselectitem" datatextfield="Name" datavaluefield="OperatorAccountId" label="'
            + label + '" ' + hideselectedvaluessection + ' entityname="' + label + '" ' + hideremoveicon + ' ' + ismultipleselection + '></vr-select>'
    }

    function operatorsCtor(ctrl, $scope, Demo_OperatorAccountAPIService, attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                
                var filter;
                var selectedIds;
                if (payload != undefined) {
                    filter = payload.filter;
                    selectedIds = payload.selectedIds;
                }

                if (filter == undefined)
                    filter = {};
                filter.GetCustomers = attrs.getcustomers != undefined;
                filter.GetSuppliers = attrs.getsuppliers != undefined;

                var serializedFilter = {};
                if (filter != undefined)
                    serializedFilter = UtilsService.serializetoJson(filter);

                return Demo_OperatorAccountAPIService.GetOperatorAccountInfo(serializedFilter).then(function (response) {
                    ctrl.datasource.length = 0;
                    angular.forEach(response, function (itm) {
                        ctrl.datasource.push(itm);
                    });

                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'OperatorAccountId', attrs, ctrl);
                    }
                });
            }

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('OperatorAccountId', attrs, ctrl);
            }

            api.getSelectedValues = function () {
                return ctrl.selectedvalues;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);