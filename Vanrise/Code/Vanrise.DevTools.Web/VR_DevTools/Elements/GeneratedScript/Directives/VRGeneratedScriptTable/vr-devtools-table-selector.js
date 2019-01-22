
appControllers.directive('vrDevtoolsTableSelector', ['VRNotificationService', 'VR_Devtools_TableAPIService', 'UtilsService', 'VRUIUtilsService',
    function (VRNotificationService, VR_Devtools_TableAPIService, UtilsService, VRUIUtilsService) {
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
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var tableSelector = new TableSelector(ctrl, $scope, $attrs);
                tableSelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {

                return getTableTemplate(attrs);
            }
        };

        function getTableTemplate(attrs) {


            var multipleselection = "";
            var label = "Table";
            if (attrs.ismultipleselection != undefined) {
                label = "Table";
                multipleselection = "ismultipleselection";
            }

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + '  on-ready="scopeModel.onSelectorReady" datatextfield="Name" datavaluefield="Name" label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Table" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' isrequired="ctrl.isrequired"></vr-select></vr-columns>';

        }


        function TableSelector(ctrl, $scope, attrs) {


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
                    var filter;
                    if (payload != undefined) {
                        if (payload.selectedIds != undefined) {
                            selectedIds = [];
                            selectedIds.push(payload.selectedIds);
                        }
                        filter = payload.filter;
                    }
                    return VR_Devtools_TableAPIService.GetTablesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
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

                api.clear = function () {
                    selectorAPI.clearDataSource();
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('Name', attrs, ctrl);
                };
                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;

    }]);