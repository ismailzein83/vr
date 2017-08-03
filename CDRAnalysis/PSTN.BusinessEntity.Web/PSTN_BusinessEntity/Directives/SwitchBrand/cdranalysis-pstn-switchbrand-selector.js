'use strict';
app.directive('cdranalysisPstnSwitchbrandSelector', ['CDRAnalysis_PSTN_SwitchBrandAPIService', 'CDRAnalysis_PSTN_SwitchBrandService', 'UtilsService', 'VRUIUtilsService',
    function (CDRAnalysis_PSTN_SwitchBrandAPIService, CDRAnalysis_PSTN_SwitchBrandService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: '=',
                isdisabled: "=",
                selectedvalues: "=",
                showaddbutton: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.filter;
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                $scope.addNewBrand = function () {
                    var onBrandAdded = function (brandObj) {
                        ctrl.datasource.push(brandObj);
                        if ($attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(brandObj);
                        else
                            ctrl.selectedvalues = brandObj;
                    };
                    CDRAnalysis_PSTN_SwitchBrandService.addSwitchBrand(onBrandAdded);
                };
                var switchBrandSelector = new SwitchBrandSelector(ctrl, $scope, $attrs);
                switchBrandSelector.initializeController();


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
                return getSwitchBrandTemplate(attrs);
            }

        };
        function getSwitchBrandTemplate(attrs) {

            var multipleselection = "";
            var label = "Brand";
            if (attrs.ismultipleselection != undefined) {
                label = "Brands";
                multipleselection = "ismultipleselection";
            }

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewBrand"';

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="BrandId" isrequired="ctrl.isrequired" '
            + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="onSelectorReady" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Brand" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
               + '</div>';
        }
        function SwitchBrandSelector(ctrl, $scope, attrs) {

            var selectorAPI;

            function initializeController() {
                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};
                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('BrandId', attrs, ctrl);
                };
                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }
                    return getSwitchesBrandInfo(attrs, ctrl, selectedIds);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }

        function getSwitchesBrandInfo(attrs, ctrl, selectedIds ) {

            return CDRAnalysis_PSTN_SwitchBrandAPIService.GetBrands().then(function (response) {
                ctrl.datasource.length = 0;
                angular.forEach(response, function (itm) {
                    ctrl.datasource.push(itm);
                });

                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'BrandId', attrs, ctrl);
                }
            });
        }
        return directiveDefinitionObject;
    }]);

