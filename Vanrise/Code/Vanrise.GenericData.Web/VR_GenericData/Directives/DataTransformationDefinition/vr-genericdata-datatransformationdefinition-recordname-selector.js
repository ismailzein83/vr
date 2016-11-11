'use strict';
app.directive('vrGenericdataDatatransformationdefinitionRecordnameSelector', ['UtilsService', '$compile', 'VRUIUtilsService', function (UtilsService, $compile, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            label: "@",
            ismultipleselection: "@",
            hideselectedvaluessection: '@',
            onselectionchanged: '=',
            isrequired: '@',
            isdisabled: "=",
            selectedvalues: "=",
            hidelabel: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];
            ctrl.datasource = [];



            ctrl.datasource = [];
            var ctor = new recordTypeCtor(ctrl, $scope, $attrs);
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
            return getTemplate(attrs);
        }

    };
    function getTemplate(attrs) {
        var label;
        if (attrs.hidelabel == undefined)
            label = 'label="Record Name"';

        if (attrs.label != undefined)
            label = 'label="'+attrs.label+'"';
        var disabled = "";
        if (attrs.isdisabled)
            disabled = "vr-disabled='true'";

        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";

        var hideselectedvaluessection = "";
        if (attrs.hideselectedvaluessection != undefined)
            hideselectedvaluessection = "hideselectedvaluessection";
        var multipleselection = "";
        if (attrs.ismultipleselection != undefined)
            multipleselection = "ismultipleselection";

        return ' <vr-select ' + multipleselection + ' on-ready="onSelectorReady" datasource="ctrl.datasource" ' + required + ' ' + hideselectedvaluessection + ' selectedvalues="ctrl.selectedvalues" ' + disabled + ' onselectionchanged="ctrl.onselectionchanged" datatextfield="Name" datavaluefield="Name"'
               + 'entityname="Record Name" ' + label + '></vr-select>';

    }

    function recordTypeCtor(ctrl, $scope, $attrs) {
        var selectorAPI;
        function initializeController() {
            $scope.onSelectorReady = function (api) {
                selectorAPI = api;
            };
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('Name', $attrs, ctrl);
            };

            api.load = function (payload) {
                if (selectorAPI != undefined)
                    selectorAPI.clearDataSource();

                var selectedIds;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    if (payload.context != undefined) {
                        if (payload.getArray && payload.getNonArray && payload.getDynamic)
                            ctrl.datasource = payload.context.getAllRecordNames();
                        else if (payload.getArray && payload.getNonArray)
                            ctrl.datasource = payload.context.getAllRecordNamesExceptDynamic();
                        else if (payload.getArray)
                            ctrl.datasource = payload.context.getArrayRecordNames();
                        else if (payload.getNonArray)
                            ctrl.datasource = payload.context.getRecordNames();
                    }
                    if (selectedIds != undefined)
                        VRUIUtilsService.setSelectedValues(selectedIds, 'Name', $attrs, ctrl);
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);

