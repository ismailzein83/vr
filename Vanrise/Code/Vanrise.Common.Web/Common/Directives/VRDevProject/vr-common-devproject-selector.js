'use strict';

app.directive('vrCommonDevprojectSelector', ['UtilsService', 'VRUIUtilsService','VR_Common_DevProjectAPIService',

    function (UtilsService, VRUIUtilsService, VR_Common_DevProjectAPIService) {
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
                ctrl.label = "Project";
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined) {
                    ctrl.label = "Projects";
                    ctrl.selectedvalues = [];
                }
                    

                var devProjectSelector = new DevProjectSelector(ctrl, $scope, $attrs);
                devProjectSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function DevProjectSelector(ctrl, $scope, attrs) {

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
                    var selectfirstitem;
                    var filter;
                    if (payload != undefined) {
                      
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    return VR_Common_DevProjectAPIService.GetVRDevProjectsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'VRDevProjectID', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('VRDevProjectID', attrs, ctrl);
                };
                api.clearDataSource = function () {
                    ctrl.datasource.length = 0;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "{{ctrl.label}}";

            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                '<vr-select ' + multipleselection + ' datatextfield="Name" datavaluefield="VRDevProjectID" isrequired="ctrl.isrequired" label="' + label +
                       '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Project" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate">' +
                   '</vr-select>' +
                   '</vr-columns>';
        }

    }]);