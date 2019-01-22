
appControllers.directive('vrDevtoolsGeneratedScriptTypeSelector', ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VR_Devtools_GeneratedScriptTypeEnum',
    function (VRNotificationService, UtilsService, VRUIUtilsService, VR_Devtools_GeneratedScriptTypeEnum) {
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

                var typeSelector = new TypeSelector(ctrl, $scope, $attrs);
                typeSelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {

                return getGeneratedScriptTypeTemplate(attrs);
            }
        };

        function getGeneratedScriptTypeTemplate(attrs) {


            var multipleselection = "";
            var label = "Generated Script Type";
            if (attrs.ismultipleselection != undefined) {
                label = "Generated Script Type";
                multipleselection = "ismultipleselection";
            }

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + '  on-ready="scopeModel.onSelectorReady" datatextfield="description" datavaluefield="value" label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Schema" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' isrequired="ctrl.isrequired"></vr-select></vr-columns>';

        }


        function TypeSelector(ctrl, $scope, attrs) {


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

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }
                    ctrl.datasource = UtilsService.getArrayEnum(VR_Devtools_GeneratedScriptTypeEnum);
                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };
                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;

    }]);