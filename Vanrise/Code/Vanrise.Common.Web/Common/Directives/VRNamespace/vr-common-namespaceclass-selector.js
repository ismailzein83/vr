(function (app) {

    'use strict';

    ClassSelector.$inject = ['VRCommon_VRNamespaceAPIService', 'UtilsService', 'VRUIUtilsService'];

    function ClassSelector(VRCommon_VRNamespaceAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectitem: "=",
                ondeselectitem: "=",
                onselectionchanged: '=',
                ismultipleselection: "@",
                isrequired: "=",
                isdisabled: "=",
                customlabel: "@",
                normalColNum: '=',
                hidelabel: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

                var ctor = new NamespaceCtor(ctrl, $scope, $attrs);
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
                return getDirectiveTemplate(attrs);
            }
        };

        function NamespaceCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    var vrNamespaceId;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        vrNamespaceId = payload.vrNamespaceId;
                    }

                    return VRCommon_VRNamespaceAPIService.GetVRNamespaceClassesInfo(vrNamespaceId).then(function (response) {
                        selectorAPI.clearDataSource();
                        ctrl.datasource.length = 0;
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }
                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'Name', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('Name', attrs, ctrl);
                };

                return api;
            }
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = 'Class';
            if (attrs.ismultipleselection != undefined) {
                label = 'Classes';
                multipleselection = 'ismultipleselection';
            }

            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }

            var hidelabel = "";
            if (attrs.hidelabel != undefined)
                hidelabel = "hidelabel";

            var hideselectedvaluessection = (attrs.hideselectedvaluessection != undefined) ? 'hideselectedvaluessection' : '';

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : '';

            var haschildcolumns = "";
            if (attrs.usefullcolumn != undefined)
                haschildcolumns = "haschildcolumns";

            return '<vr-columns colnum="{{ctrl.normalColNum}}" ' + haschildcolumns + '>'
                + '<span vr-disabled="ctrl.isdisabled">'
                + '<vr-select on-ready="ctrl.onSelectorReady"'
                + ' datasource="ctrl.datasource"'
                + ' selectedvalues="ctrl.selectedvalues"'
                + ' onselectionchanged="ctrl.onselectionchanged"'
                + ' onselectitem="ctrl.onselectitem"'
                + ' ondeselectitem="ctrl.ondeselectitem"'
                + ' datavaluefield="Name"'
                + ' datatextfield="Name"'
                + ' ' + multipleselection
                + ' ' + hideselectedvaluessection
                + ' isrequired="ctrl.isrequired"'
                + ' ' + hideremoveicon
                + ' label="' + label + '"'
                + ' entityName="' + label + '" '
                + hidelabel
                + ' </vr-select>'
                + '</span>'
                + '</vr-columns>';
        }
    }

    app.directive('vrCommonNamespaceclassSelector', ClassSelector);

})(app);
