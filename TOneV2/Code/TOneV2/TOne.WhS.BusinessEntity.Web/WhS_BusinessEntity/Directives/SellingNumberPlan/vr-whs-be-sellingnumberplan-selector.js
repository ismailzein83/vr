'use strict';
app.directive('vrWhsBeSellingnumberplanSelector', ['WhS_BE_SellingNumberPlanAPIService', 'UtilsService', 'VRUIUtilsService', 'WhS_BE_SellingNumberPlanService',
    function (WhS_BE_SellingNumberPlanAPIService, UtilsService, VRUIUtilsService, WhS_BE_SellingNumberPlanService) {

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
                normalColNum: '@',
                hideremoveicon: '@',
                onbeforeselectionchanged: '=',
                customlabel: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                ctrl.datasource = [];

                $scope.addNewSellingNumberPlan = function () {
                    var onSellingNumberPlanAdded = function (sellingNumberPlanObj) {
                        ctrl.datasource.push(sellingNumberPlanObj.Entity);
                        if ($attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(sellingNumberPlanObj.Entity);
                        else
                            ctrl.selectedvalues = sellingNumberPlanObj.Entity;
                    };
                    WhS_BE_SellingNumberPlanService.addSellingNumberPlan(onSellingNumberPlanAdded);
                };
                ctrl.haspermission = function () {
                    return WhS_BE_SellingNumberPlanAPIService.HasAddSellingNumberPlanPermission();
                };
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

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewSellingNumberPlan"';

            var hideremoveicon = "";
            if (attrs.hideremoveicon)
                hideremoveicon = "hideremoveicon";

            var customlabel = '';
            if (attrs.customlabel != undefined)
                customlabel = 'customlabel="{{ctrl.customlabel}}"';

            return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                       + '<vr-select ' + multipleselection + ' ' + addCliked + '  isrequired="ctrl.isrequired" datatextfield="Name" datavaluefield="SellingNumberPlanId" on-ready="ctrl.onSelectorReady" '
                           + ' label="' + label + '" datasource="ctrl.datasource"  selectedvalues="ctrl.selectedvalues" onbeforeselectionchanged="ctrl.onbeforeselectionchanged" onselectionchanged="ctrl.onselectionchanged"'
                           + ' onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" haspermission="ctrl.haspermission" ' + hideremoveicon + ' ' + customlabel + '>'
                       + '</vr-select>' +
                   '</vr-columns>';
        }

        function sellingNumberPlanCtor(ctrl, $scope, attrs) {
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
                    selectorAPI.clearDataSource();
                    var selectIfSingleItem;
                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        selectIfSingleItem = payload.selectifsingleitem;
                    }

                    return WhS_BE_SellingNumberPlanAPIService.GetSellingNumberPlans().then(function (response) {
                        ctrl.datasource.length = 0;
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

                api.setSelectedIds = function (selectedIds) {
                    setTimeout(function () {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'SellingNumberPlanId', attrs, ctrl);
                        UtilsService.safeApply($scope);
                    });
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);