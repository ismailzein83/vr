'use strict';
app.directive('vrTimedimensionSelector', ['UtilsService', 'VRUIUtilsService','TimeDimensionTypeEnum',
function (UtilsService, VRUIUtilsService, TimeDimensionTypeEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            isdisabled: "=",
            onselectionchanged: '=',
            isrequired: "=",
            selectedvalues: '=',
            hideremoveicon: "@",
            label:'@'
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            ctrl.datasource = [];
            var ctor = new periodCtor(ctrl, $scope, $attrs);
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
        var multipleselection = "";
        var label = "Time Dimension";
        var hideremoveicon = "";

        if (attrs.ismultipleselection != undefined) {
            label = "Time Dimensions";
            multipleselection = "ismultipleselection";
        }
        else if (attrs.hideremoveicon != undefined) {
            hideremoveicon = "hideremoveicon";
        }

        if (attrs.label != undefined)
            label = "{{ctrl.label}}";

        return '<div  vr-loader="isLoadingDirective">'
            + '<vr-select  isrequired="ctrl.isrequired" ' + multipleselection + ' ' + hideremoveicon + ' datatextfield="description" datavaluefield="value" on-ready="ctrl.onSelectorReady"' 
        + ' label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged"  vr-disabled="ctrl.isdisabled"></vr-select>'
            + '</div>';
    }

    function periodCtor(ctrl, $scope, $attrs) {

        function initializeController() {
            ctrl.onSelectorReady= function(api)
            {
                defineAPI();
            }
           
        }

        function defineAPI() {
            var api = {};
            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('value', $attrs, ctrl);
            };
            api.getSelectedValues = function () {
                return ctrl.selectedvalues;
            };
            api.load = function (payload) {
                var selectedIds;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                }

                ctrl.datasource = UtilsService.getArrayEnum(TimeDimensionTypeEnum);


                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'value', $attrs, ctrl);
                }

            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);