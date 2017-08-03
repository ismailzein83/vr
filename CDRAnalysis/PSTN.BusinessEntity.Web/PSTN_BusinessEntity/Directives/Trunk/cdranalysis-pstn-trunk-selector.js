'use strict';
app.directive('cdranalysisPstnTrunkSelector', ['CDRAnalysis_PSTN_TrunkAPIService',  'UtilsService', 'VRUIUtilsService',
    function (CDRAnalysis_PSTN_TrunkAPIService, UtilsService, VRUIUtilsService) {

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
                var trunkSelector = new TrunkSelector(ctrl, $scope, $attrs);
                trunkSelector.initializeController();


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
                return getTrunkTemplate(attrs);
            }

        };
        function getTrunkTemplate(attrs) {

            var multipleselection = "";
            var label = "Trunk";
            if (attrs.ismultipleselection != undefined) {
                label = "Trunks";
                multipleselection = "ismultipleselection";
            }
            if (attrs.label != undefined)
                label = attrs.label;


            var addCliked = '';
            //if (attrs.showaddbutton != undefined)
            //    addCliked = 'onaddclicked="addNewSwitch"';

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="TrunkId" isrequired="ctrl.isrequired" '
            + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="onSelectorReady" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Trunk" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
               + '</div>';
        }
        function TrunkSelector(ctrl, $scope, attrs) {

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
                    return VRUIUtilsService.getIdSelectedIds('TrunkId', attrs, ctrl);
                };
                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    var filter;
                    var selectedIds;
                    var excludedId;
                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                        excludedId = payload.excludedId;
                    }
                    var serializedFilter = {};
                    ctrl.filter = undefined;
                    if (filter != undefined) {
                        serializedFilter = UtilsService.serializetoJson(filter);
                    }

                    return getTrunksInfo(attrs, ctrl, selectedIds, serializedFilter, excludedId);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }

        function getTrunksInfo(attrs, ctrl, selectedIds, filter, excludedId) {

            return CDRAnalysis_PSTN_TrunkAPIService.GetTrunksInfo(filter).then(function (response) {
                ctrl.datasource.length = 0;
                angular.forEach(response, function (itm) {
                    if (itm.TrunkId != excludedId)
                        ctrl.datasource.push(itm);
                });

                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'TrunkId', attrs, ctrl);
                }
            });
        }
        return directiveDefinitionObject;
    }]);

