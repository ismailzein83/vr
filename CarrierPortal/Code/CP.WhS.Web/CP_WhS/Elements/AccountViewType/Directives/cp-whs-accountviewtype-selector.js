'use strict';
app.directive('cpWhsAccountviewtypeSelector', ['CP_WhS_AccountViewTypeEnum', 'UtilsService', 'VRUIUtilsService',
    function (CP_WhS_AccountViewTypeEnum, UtilsService, VRUIUtilsService) {

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
                var selector = new AccountViewTypeSelector(ctrl, $scope, $attrs);
                selector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            template: function (element, attrs) {
                return getAccountViewTypeTemplate(attrs);
            }

        };

        function getAccountViewTypeTemplate(attrs) {

            var multipleselection = "";
            var label = "Account Type";
            if (attrs.ismultipleselection != undefined) {
                label = "Account Types";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                + '<vr-select ' + multipleselection + '  datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired" '
                + ' label="' + label + '"  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="onSelectorReady" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</vr-columns>';
        }

        function AccountViewTypeSelector(ctrl, $scope, attrs) {

            var selectorAPI;
            function initializeController() {
                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};
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
                    ctrl.datasource = UtilsService.getArrayEnum(CP_WhS_AccountViewTypeEnum);
                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);
