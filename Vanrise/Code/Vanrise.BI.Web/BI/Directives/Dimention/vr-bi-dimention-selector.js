'use strict';
app.directive('vrBiDimensionSelector', ['VR_BI_BIDimensionAPIService', 'UtilsService', 'VRUIUtilsService',
    function (VR_BI_BIDimensionAPIService, UtilsService, VRUIUtilsService) {

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
                customlabel: "@"
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new DimensionCtor(ctrl, $scope, $attrs);
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
                return getTimeEntityTemplate(attrs);
            }

        };


        function getTimeEntityTemplate(attrs) {

            var multipleselection = "";

            var label = ' label="Entity Item" ';
            if (attrs.ismultipleselection != undefined) {
                label =' label="Entity Items" ';
                multipleselection = "ismultipleselection";
            }

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;
            if (attrs.hidelabel != undefined)
                label = '';
            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="DimensionId" isrequired="ctrl.isrequired" '
                + label + ' datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Entity Value" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>';
        }

        function DimensionCtor(ctrl, $scope, attrs) {

            var selectorApi;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorApi.clearDataSource();
                    var selectedIds;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        if (payload.entityName != undefined) {
                            return VR_BI_BIDimensionAPIService.GetDimensionInfo(payload.entityName).then(function (response) {
                                ctrl.datasource.length = 0;
                                angular.forEach(response, function (itm) {
                                    ctrl.datasource.push(itm);
                                });

                                if (selectedIds != undefined) {
                                    VRUIUtilsService.setSelectedValues(selectedIds, 'DimensionId', attrs, ctrl);
                                }
                            });
                        }

                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('DimensionId', attrs, ctrl);
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);