'use strict';
app.directive('retailBeAccountRoottypeSelector', ['Retail_BE_RootSelectorTypeEnum', 'UtilsService', 'VRUIUtilsService',
    function (Retail_BE_RootSelectorTypeEnum, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: '=',
                isdisabled: "=",
                selectedvalues: "=",
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];
                var selector = new Selector(ctrl, $scope, $attrs);
                selector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
      
            template: function (element, attrs) {
                return getTemplate(attrs);
            }

        };

        function getTemplate(attrs) {

            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }
           
            return '<vr-select ' + multipleselection + ' hideremoveicon datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired" '
                + ' label=" " hidefilterbox="true" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="onSelectorReady"  onselectionchanged="ctrl.onselectionchanged"  onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>';

        }

        function Selector(ctrl, $scope, attrs) {

            var selectorAPI;
            function initializeController() {
                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};
                ctrl.datasource = UtilsService.getArrayEnum(Retail_BE_RootSelectorTypeEnum);

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };
                api.getSelectedValues = function () {
                    return VRUIUtilsService.getIdSelectedIds('description', attrs, ctrl);
                };
                api.load = function (payload) {
                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }
                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                    }
                    else
                        VRUIUtilsService.setSelectedValues(ctrl.datasource[0].value, 'value', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);
