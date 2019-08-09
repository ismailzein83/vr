
appControllers.directive('vrDevtoolsDevprojectsSelector', ['VR_Devtools_DevProjectTemplateAPIService', 'VRUIUtilsService',
    function (VR_Devtools_DevProjectTemplateAPIService, VRUIUtilsService) {
        'use strict';

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                ondeselectallitems: '=',
                selectedvalues: '=',
                isrequired: "=",
                onselectitem: "=",
                ondeselectitem: "=",
                hideremoveicon: '@',
                normalColNum: '@',
                isdisabled: '=',
                label: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var devProjectsSelector = new DevProjectsSelector(ctrl, $scope, $attrs);
                devProjectsSelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {

                return getDevProjectTemplate(attrs);
            }
        };

        function getDevProjectTemplate(attrs) {


            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + '  on-ready="scopeModel.onSelectorReady" datatextfield="Name" datavaluefield="Name" label="{{ctrl.label}}" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" ondeselectallitems="ctrl.ondeselectallitems" onselectionchanged="ctrl.onselectionchanged" entityName="Columns"  onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' isrequired="ctrl.isrequired"></vr-select></vr-columns>';

        }


        function DevProjectsSelector(ctrl, $scope, attrs) {


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

                api.load = function (payload) { //payload is an object that has selectedids and filter
                    selectorAPI.clearDataSource();
                    var selectedIds;
                    var connectionId;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        connectionId = payload.connectionId;
                    }
                    return VR_Devtools_DevProjectTemplateAPIService.GetVRDevProjectsInfo(connectionId).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'Name', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    var selectedIds = [];
                    selectedIds = VRUIUtilsService.getIdSelectedIds('Name', attrs, ctrl);
                    return selectedIds;
                };

                api.clear = function () {
                    selectorAPI.clearDataSource();
                };
                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;

    }]);

