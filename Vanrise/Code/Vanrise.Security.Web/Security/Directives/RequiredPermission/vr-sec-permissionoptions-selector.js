'use strict';
app.directive('vrSecPermissionoptionsSelector', ['UtilsService', 'VRUIUtilsService',

function (UtilsService, VRUIUtilsService) {

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
            customlabel: "@",
            onitemadded: "="
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];
            var selector = new requiredPermissionSelector(ctrl, $scope, $attrs);
            selector.initialize();
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
            return getDirectiveTemplate(attrs);
        }
    };

    function requiredPermissionSelector(ctrl, $scope, attrs) {
        this.initialize = initialize;

        var selectorAPI;

        function initialize() {
            ctrl.onSelectorReady = function (api) {
                selectorAPI = api;
                if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                    ctrl.onReady(getDirectiveAPI());
                }
            };
        }

        function getDirectiveAPI() {
            var directiveAPI = {};

            directiveAPI.load = function (payload) {
                selectorAPI.clearDataSource();

                var selectedIds;

                if (payload != undefined) {
                    selectedIds = payload.selectedIds;

                    if (payload && payload.datasource.length > 0) {
                        for (var i = 0; i < payload.datasource.length; i++) {
                            ctrl.datasource.push({ name: payload.datasource[i] });
                        }
                    }

                    if (selectedIds) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'name', attrs, ctrl);
                    }
                }

            };

            directiveAPI.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('name', attrs, ctrl);
            };

            return directiveAPI;
        }
    }

    function getDirectiveTemplate(attrs) {
        var label = attrs.label ? attrs.label : "Permmission";

        var ismultipleselection = '';
        if (attrs.ismultipleselection!=undefined) {
            label = "Permmissions";
            ismultipleselection = ' ismultipleselection';
        }

        var hideremoveicon = (attrs.hideremoveicon != undefined && attrs.hideremoveicon) != null ? 'hideremoveicon' : null;

        return '<vr-select on-ready="ctrl.onSelectorReady"'
                + ' label="' + label + '"'
                + ' datasource="ctrl.datasource"'
                + ' selectedvalues="ctrl.selectedvalues"'
                + ' onselectionchanged="ctrl.onselectionchanged"'
                + ' onselectitem="ctrl.onselectitem"'
                + ' ondeselectitem="ctrl.ondeselectitem"'
                + ' datavaluefield="name"'
                + ' datatextfield="name"'
                + ismultipleselection
                + ' vr-disabled="ctrl.isdisabled"'
                + ' isrequired="ctrl.isrequired"'
                + hideremoveicon
                + ' entityName="Permission">'
                + '</vr-select>';
    }

    return directiveDefinitionObject;
}]);
