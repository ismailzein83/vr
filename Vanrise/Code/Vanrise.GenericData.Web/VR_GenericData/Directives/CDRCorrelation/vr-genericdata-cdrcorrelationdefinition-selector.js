'use strict';

app.directive('vrGenericdataCdrcorrelationdefinitionSelector', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_CDRCorrelationAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_CDRCorrelationAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new CDRCorrelationDefinitionSelectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function CDRCorrelationDefinitionSelectorCtor(ctrl, $scope, attrs) {
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
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    return VR_GenericData_CDRCorrelationAPIService.GetCDRCorrelationDefinitionsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'CDRCorrelationDefinitionId', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('CDRCorrelationDefinitionId', attrs, ctrl);
                };
               
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }


        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "CDR Correlation Definition";
            var hideremoveicon = '';

            if (attrs.ismultipleselection != undefined) {
                label = "CDR Correlation Definitions";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            if (attrs.hideremoveicon != undefined)
                hideremoveicon = 'hideremoveicon';

            return '<vr-select label="' + label + '" ' + multipleselection + ' ' + hideremoveicon + ' datatextfield="Name" datavaluefield="CDRCorrelationDefinitionId" isrequired="ctrl.isrequired" '
                       + ' datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" '
                       + ' customvalidate="ctrl.customvalidate">'
                    + '</vr-select>';
        }
    }]);