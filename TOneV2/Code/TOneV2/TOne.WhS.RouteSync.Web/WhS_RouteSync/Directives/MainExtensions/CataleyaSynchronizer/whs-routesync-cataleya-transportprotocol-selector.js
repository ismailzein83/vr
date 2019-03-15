'use strict';

app.directive('whsRoutesyncCataleyaTransportprotocolSelector', ['UtilsService', 'VRUIUtilsService', 'WhS_RouteSync_TransportProtocolEnum',
    function (UtilsService, VRUIUtilsService, WhS_RouteSync_TransportProtocolEnum) {
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
                customvalidate: '=',
                hidelabel:'@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new TransportProtocolsCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function TransportProtocolsCtor(ctrl, $scope, attrs) {
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
                    var setDefaultValue;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        setDefaultValue = payload.setDefaultValue;
                    }
                    var transportProtocols = UtilsService.getArrayEnum(WhS_RouteSync_TransportProtocolEnum);

                    if (transportProtocols != null) {
                        for (var i = 0; i < transportProtocols.length; i++) {
                            ctrl.datasource.push(transportProtocols[i]);
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                        }
                        else if (setDefaultValue) {
                            VRUIUtilsService.setSelectedValues(transportProtocols[0].value, 'value', attrs, ctrl);
                        }
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Transport Protocol";
            var hidelabel="";

            if (attrs.ismultipleselection != undefined) {
                label = "Transport Protocols";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = ' hideremoveicon ';

            if (attrs.hidelabel != undefined)
                hidelabel = "hidelabel";

            return '<vr-select ' + multipleselection + ' datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired" ' + hidelabel+ ' label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" '
                + ' selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" '
                + hideremoveicon + ' customvalidate="ctrl.customvalidate">' +
                '</vr-select>';
        }
    }]);