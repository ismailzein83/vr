﻿'use strict';
app.directive('vrNpSellingnumberplanSelector', ['Vr_NP_SellingNumberPlanAPIService', 'UtilsService', 'VRUIUtilsService',
    function (Vr_NP_SellingNumberPlanAPIService, UtilsService, VRUIUtilsService) {

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
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                ctrl.datasource = [];

                var ctor = new sellingNumberPlanCtor(ctrl, $scope, $attrs);
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
                return getBeSellingNumberPlansTemplate(attrs);
            }

        };


        function getBeSellingNumberPlansTemplate(attrs) {

            var multipleselection = "";
            var label = "Selling Number Plan";
            if (attrs.ismultipleselection != undefined) {
                label = "Selling Number Plans";
                multipleselection = "ismultipleselection";
            }


            return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
               + '<vr-select ' + multipleselection + '  isrequired="ctrl.isrequired" datatextfield="Name" datavaluefield="SellingNumberPlanId" '
               + ' label="' + label + '" datasource="ctrl.datasource"  selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" on-ready="ctrl.onSelectorReady" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
               + '</vr-columns>';
        }

        function sellingNumberPlanCtor(ctrl, $scope, attrs) {
            var selectorAPI;
            function initializeController() {
                ctrl.onSelectorReady = function(api){
                    selectorAPI =  api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    var selectIfSingleItem;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        selectIfSingleItem = payload.selectifsingleitem;

                    }

                    return Vr_NP_SellingNumberPlanAPIService.GetSellingNumberPlans().then(function (response) {
                        selectorAPI.clearDataSource();
                        angular.forEach(response, function (itm) {
                            ctrl.datasource.push(itm);
                        });

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'SellingNumberPlanId', attrs, ctrl);
                        }
                        else if (selectedIds == undefined && selectIfSingleItem == true) {
                            selectorAPI.selectIfSingleItem();
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SellingNumberPlanId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);