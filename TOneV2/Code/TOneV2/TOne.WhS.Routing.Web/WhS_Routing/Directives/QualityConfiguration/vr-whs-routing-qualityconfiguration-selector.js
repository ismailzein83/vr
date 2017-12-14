'use strict';

app.directive('vrWhsRoutingQualityconfigurationSelector',['WhS_Routing_QualityConfigurationAPIService','UtilsService', 'VRUIUtilsService',
    function (WhS_Routing_QualityConfigurationAPIService, UtilsService, VRUIUtilsService) {
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
                customvalidate: '=',
                hidelabel: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var qualityConfigurationSelector = new QualityConfigurationSelector(ctrl, $scope, $attrs);
                qualityConfigurationSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function QualityConfigurationSelector(ctrl, $scope, attrs) {
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
                    var selectedIds;
                    var filter;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    var serializedFilter = {};
                    if (filter != undefined)
                        serializedFilter = UtilsService.serializetoJson(filter);

                    return WhS_Routing_QualityConfigurationAPIService.GetQualityConfigurationInfo(serializedFilter).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++)
                                ctrl.datasource.push(response[i]);
                        }
                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'QualityConfigurationId', attrs, ctrl);
                        }
                    });

                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('QualityConfigurationId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                }
            }

        function getTemplate(attrs) {
            var multipleselection = "";
            var label = "Quality Configuration";

            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
                label = "Quality Configurations";
            }

            if (attrs.hidelabel != undefined)
                label = "";

            var hideremoveicon;
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select datatextfield="Name" datavaluefield="QualityConfigurationId" isrequired="ctrl.isrequired" datasource="ctrl.datasource" ' + multipleselection + '  on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged"' +
                       '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' customvalidate="ctrl.customvalidate">' +
                   '</vr-select></vr-columns>';
        }
    }]);