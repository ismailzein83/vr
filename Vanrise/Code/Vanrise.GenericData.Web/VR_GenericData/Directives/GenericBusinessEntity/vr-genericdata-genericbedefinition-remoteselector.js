'use strict';

app.directive('vrGenericdataGenericbedefinitionRemoteselector', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBEDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService) {
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

                var ctor = new GenericBEDefinitionRemoteSelectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function GenericBEDefinitionRemoteSelectorCtor(ctrl, $scope, attrs) {
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
                    var connectionId;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        connectionId = payload.connectionId;
                    }
                    var filter = {
                        Filters: [{
                            $type: "Vanrise.GenericData.Business.GenericBusinessEntityDefinitionFilter, Vanrise.GenericData.Business"
                        }]
                    };
                    if (connectionId != undefined) {
                        var input = {
                            VRConnectionId: connectionId,
                            Filter: UtilsService.serializetoJson(filter)
                        };
                        return VR_GenericData_GenericBEDefinitionAPIService.GetRemoteGenericBEDefinitionInfo(input).then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    ctrl.datasource.push(response[i]);
                                }

                                if (selectedIds != undefined) {
                                    VRUIUtilsService.setSelectedValues(selectedIds, 'BusinessEntityDefinitionId', attrs, ctrl);
                                }
                            }
                        });
                    }
                    else {
                        ctrl.datasource.length = 0;
                    }
                 
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('BusinessEntityDefinitionId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Remote Generic BE Definition";

            if (attrs.ismultipleselection != undefined) {
                label = "Remote Generic BE Definitions";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                        '<vr-select ' + multipleselection + ' datatextfield="Name" datavaluefield="BusinessEntityDefinitionId" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" '
                            + ' selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" '
                            + ' hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate">' +
                        '</vr-select>' +
                    '</vr-columns>';
        }
    }]);