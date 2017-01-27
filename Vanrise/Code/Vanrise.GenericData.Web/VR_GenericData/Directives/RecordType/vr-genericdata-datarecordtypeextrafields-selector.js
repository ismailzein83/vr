'use strict';

app.directive('vrGenericdataDatarecordtypeextrafieldsSelector', ['VR_GenericData_DataRecordTypeAPIService', 'UtilsService', 'VRUIUtilsService',
    function (VR_GenericData_DataRecordTypeAPIService, UtilsService, VRUIUtilsService) {

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
                hideremoveicon: '@',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new extraFieldCtor(ctrl, $scope, $attrs);
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
                return getExtraFieldTemplate(attrs);
            }
        };

        function getExtraFieldTemplate(attrs) {

            var multipleselection = "";
            var label = "Record Type Extra Field";
            if (attrs.ismultipleselection != undefined) {
                label = "Record Type Extra Fields";
                multipleselection = "ismultipleselection";
            }

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + '  on-ready="ctrl.onSelectorReady" datatextfield="Name" datavaluefield="ExtensionConfigurationId" label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Extra Fields Type" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" isrequired="ctrl.isrequired"></vr-select></vr-columns>';
        };

        function extraFieldCtor(ctrl, $scope, attrs) {
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

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }
                    return VR_GenericData_DataRecordTypeAPIService.GetDataRecordTypeExtraFieldsTemplates().then(function (response) {
                        selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'ExtensionConfigurationId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('ExtensionConfigurationId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };

            this.initializeController = initializeController;
        };

        return directiveDefinitionObject;
    }]);