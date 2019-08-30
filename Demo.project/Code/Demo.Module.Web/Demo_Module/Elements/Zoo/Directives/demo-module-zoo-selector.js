app.directive('demoModuleZooSelector', ['VRUIUtilsService', 'Demo_Module_ZooAPIService','UtilsService',
    function (VRUIUtilsService, Demo_Module_ZooAPIService, UtilsService) {

        'use strict';

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
                hideremoveicon: '@',
                normalColNum: '@',
                isdisabled: '='
            },
            controller: function ($scope, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var zooSelector = new ZooSelector(ctrl, $scope, $attrs);
                zooSelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {

            var label = 'Zoo';

            var multipleselection = '';
            if (attrs.ismultipleselection != undefined) {
                label = 'Zoos';
                multipleselection = 'ismultipleselection';
            }

            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined) {
                hideremoveicon = 'hideremoveicon';
            }

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + '<span vr-disabled="ctrl.isdisabled">'
                + '<vr-select on-ready="scopeModel.onSelectorReady" ' + multipleselection + ' datatextfield="Name" datavaluefield="ZooId"  isrequired="ctrl.isrequired" '
                + ' label="' + label + '" ' + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Zoo" onselectitem="ctrl.onselectitem" '
                + ' ondeselectitem = "ctrl.ondeselectitem"' + hideremoveicon + ' >'
                + '</vr-select>'
                + '</span>'
                + '</vr-columns>';
        }

        function ZooSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var selectedIds;
                    var filter;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    return Demo_Module_ZooAPIService.GetZoosInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'ZooId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('ZooId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);