
appControllers.directive('vrDevtoolsTemplateTablesSelector', ['VR_Devtools_DevProjectTemplateAPIService', 'VRUIUtilsService',
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

                var templateTablesSelector = new TemplateTablesSelector(ctrl, $scope, $attrs);
                templateTablesSelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {

                return getTemplateTables(attrs);
            }
        };

        function getTemplateTables(attrs) {


            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + '  on-ready="scopeModel.onSelectorReady" datatextfield="Name" datavaluefield="Name" label="{{ctrl.label}}" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" ondeselectallitems="ctrl.ondeselectallitems" onselectionchanged="ctrl.onselectionchanged" entityName="Tables"  onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' isrequired="ctrl.isrequired"></vr-select></vr-columns>';

        }


        function TemplateTablesSelector(ctrl, $scope, attrs) {

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
                  
                    return VR_Devtools_DevProjectTemplateAPIService.GetDevProjectTableNames().then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    var selectedIds = [];
                    selectedIds = VRUIUtilsService.getIdSelectedIds('Name', attrs, ctrl);
                    return selectedIds;                };

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

