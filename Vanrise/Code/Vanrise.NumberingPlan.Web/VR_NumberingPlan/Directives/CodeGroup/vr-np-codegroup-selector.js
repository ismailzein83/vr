'use strict';
app.directive('vrNpCodegroupSelector', ['Vr_NP_CodeGroupAPIService', 'Vr_NP_CodeGroupService', 'UtilsService',  'VRUIUtilsService',

    function (Vr_NP_CodeGroupAPIService, Vr_NP_CodeGroupService, UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            type: "=",
            onReady: '=',
            label: "@",
            ismultipleselection: "@",
            hideselectedvaluessection: '@',
            onselectionchanged: '=',
            isrequired: '@',
            isdisabled: "=",
            selectedvalues: "=",
            showaddbutton: '@',
            hidelabel:'@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];
            ctrl.datasource = [];

            var ctor = new codeGroupCtor(ctrl, $scope, Vr_NP_CodeGroupAPIService, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }

    };
    function getTemplate(attrs) {
        var label;
        if (attrs.hidelabel == undefined)
            label = 'label="Code Group"';
        var disabled = "";
        if (attrs.isdisabled)
            disabled = "vr-disabled='true'";

        

        var hideselectedvaluessection = "";
        if (attrs.hideselectedvaluessection != undefined)
            hideselectedvaluessection = "hideselectedvaluessection";
        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="ctrl.addNewCodeGroup"';

        var multipleselection = "";
        if (attrs.ismultipleselection != undefined)
            multipleselection = "ismultipleselection";

        return ' <vr-select ' + multipleselection + ' datasource="ctrl.datasource"  isrequired="ctrl.isrequired"  ' + hideselectedvaluessection + ' selectedvalues="ctrl.selectedvalues" ' + disabled + ' onselectionchanged="ctrl.onselectionchanged" datatextfield="Name" datavaluefield="CodeGroupId"'
                   + 'entityname="Code Group" ' + label + ' ' + addCliked + '></vr-select>';
       
    }
    function codeGroupCtor(ctrl, $scope, Vr_NP_RateTypeAPIService, $attrs) {

        function initializeController() {
            ctrl.addNewCodeGroup = function () {
                var onCodeGroupAdded = function (CodeGroupObj) {
                    ctrl.datasource.push(CodeGroupObj);
                    if ($attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(CodeGroupObj);
                    else
                        ctrl.selectedvalues = CodeGroupObj;
                };
                Vr_NP_RateTypeService.addCodeGroup(onCodeGroupAdded);
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('CodeGroupId', $attrs, ctrl);
            };
            api.load = function (payload) {

                var selectedIds;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                }

                return Vr_NP_CodeGroupAPIService.GetAllCodeGroups().then(function (response) {
                    angular.forEach(response, function (item) {
                        ctrl.datasource.push(item);

                    });
                    if (selectedIds != undefined)
                        VRUIUtilsService.setSelectedValues(selectedIds, 'CodeGroupId', $attrs, ctrl);

                });
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);

