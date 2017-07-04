﻿'use strict';
app.directive('vrCommonBankDetailSelector', ['VRCommon_BankDetailAPIService', 'UtilsService', 'VRUIUtilsService','VRCommon_BankDetailService',
function (VRCommon_BankDetailAPIService, UtilsService, VRUIUtilsService,VRCommon_BankDetailService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            isdisabled: "=",
            onselectionchanged: '=',
            isrequired: "@",
            selectedvalues: '=',
            normalColNum: '@'

        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            ctrl.datasource = [];


            $scope.addNewBank = function () {
                var onBankAdded = function (bankObj) {
                    ctrl.datasource.push(bankObj);
                    if ($attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(bankObj);
                    else
                        ctrl.selectedvalues = bankObj;
                };
                VRCommon_BankDetailService.addBankDetail(onBankAdded, true);
            };

            var ctor = new bankDetailCtor(ctrl, $scope, $attrs);
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

        var multipleselection = "";
        var label = "Bank Details";
        if (attrs.ismultipleselection != undefined) {
            label = "Bank Details";
            multipleselection = "ismultipleselection";
        }

        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";

        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewBank"';

        return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
            + '<vr-select ' + multipleselection + ' ' + addCliked + ' datatextfield="Bank" datavaluefield="BankDetailId" '
        + required + ' label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"   onselectionchanged="ctrl.onselectionchanged" vr-disabled="ctrl.isdisabled"></vr-select>'
           + '</vr-columns>';
    }

    function bankDetailCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('BankDetailId', $attrs, ctrl);
            };
            api.load = function (payload) {

                var selectedIds;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                }

                return VRCommon_BankDetailAPIService.GetBankDetailsInfo().then(function (response) {
                    angular.forEach(response, function (item) {
                        ctrl.datasource.push(item);
                    });
                    if (selectedIds != undefined)
                        VRUIUtilsService.setSelectedValues(selectedIds, 'BankDetailId', $attrs, ctrl);

                });
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);