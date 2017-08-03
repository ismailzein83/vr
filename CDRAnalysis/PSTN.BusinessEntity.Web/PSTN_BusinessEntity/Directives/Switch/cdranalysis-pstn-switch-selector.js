'use strict';
app.directive('cdranalysisPstnSwitchSelector', ['CDRAnalysis_PSTN_SwitchAPIService', 'CDRAnalysis_PSTN_SwitchService', 'UtilsService', 'VRUIUtilsService',
    function (CDRAnalysis_PSTN_SwitchAPIService, CDRAnalysis_PSTN_SwitchService, UtilsService, VRUIUtilsService) {

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

                //$scope.addNewCity = function () {
                //    var onCityAdded = function (cityObj) {
                //        ctrl.datasource.push(cityObj.Entity);
                //        if ($attrs.ismultipleselection != undefined)
                //            ctrl.selectedvalues.push(cityObj.Entity);
                //        else
                //            ctrl.selectedvalues = cityObj.Entity;
                //    };

                //    if (ctrl.filter != undefined)
                //        var countryId = ctrl.filter.CountryId;
                //    VRCommon_CityService.addCity(onCityAdded, countryId);
                //}
                var switchSelector = new SwitchSelector(ctrl, $scope, $attrs);
                switchSelector.initializeController();


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
                return getSwitchTemplate(attrs);
            }

        };
        function getSwitchTemplate(attrs) {

            var multipleselection = "";
            var label = "Switch";
            if (attrs.ismultipleselection != undefined) {
                label = "Switches";
                multipleselection = "ismultipleselection";
            }

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewSwitch"';

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="SwitchId" isrequired="ctrl.isrequired" '
            + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="onSelectorReady" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Switch" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
               + '</div>';
        }
        function SwitchSelector(ctrl, $scope, attrs) {

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
                    return VRUIUtilsService.getIdSelectedIds('SwitchId', attrs, ctrl);
                };
                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    var filter;
                    var selectedIds;
                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }
                    var serializedFilter = {};
                    if (filter != undefined) {
                        serializedFilter = UtilsService.serializetoJson(filter);
                    }
                    return getSwitchesInfo(attrs, ctrl, selectedIds, serializedFilter);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }

        function getSwitchesInfo(attrs, ctrl, selectedIds , filter) {

            return CDRAnalysis_PSTN_SwitchAPIService.GetSwitchesInfo(filter).then(function (response) {
                ctrl.datasource.length = 0;
                angular.forEach(response, function (itm) {
                    ctrl.datasource.push(itm);
                });

                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'SwitchId', attrs, ctrl);
                }
            });
        }
        return directiveDefinitionObject;
    }]);

