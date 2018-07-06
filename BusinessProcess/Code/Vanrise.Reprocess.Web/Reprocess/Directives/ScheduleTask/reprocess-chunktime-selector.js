'use strict';

app.directive('reprocessChunktimeSelector', ['UtilsService', 'VRUIUtilsService', 'ReprocessChunkTimeEnum',
    function (UtilsService, VRUIUtilsService, ReprocessChunkTimeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                hideremoveicon: '@',
                normalColNum: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new ChunkTimeCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function ChunkTimeCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

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

                    var selectedIds;
                    var includedChunkTimes;
                    var selectIfSingleItem;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        includedChunkTimes = payload.includedChunkTimes;
                        selectIfSingleItem = payload.selectIfSingleItem;
                    }

                    var reprocessChunkTimes = UtilsService.getArrayEnum(ReprocessChunkTimeEnum);
                    if (includedChunkTimes != undefined) {
                        for (var index = 0; index < reprocessChunkTimes.length; index++) {
                            var currentReprocessChunkTime = reprocessChunkTimes[index];
                            if (UtilsService.contains(includedChunkTimes, currentReprocessChunkTime.value)) {
                                ctrl.datasource.push(currentReprocessChunkTime);
                            }
                        }
                    } else {
                        ctrl.datasource = reprocessChunkTimes;
                    }

                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                    }
                    else if (selectIfSingleItem == true) {
                        selectorAPI.selectIfSingleItem();
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };

                api.hasSingleItem = function () {
                    return ctrl.datasource.length == 1;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "ChunkTime";

            if (attrs.ismultipleselection != undefined) {
                label = "ChunkTimes";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                        '<vr-select ' + multipleselection + ' datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" '
                            + ' selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" '
                            + ' hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate">' +
                        '</vr-select>' +
                    '</vr-columns>';
        }
    }]);