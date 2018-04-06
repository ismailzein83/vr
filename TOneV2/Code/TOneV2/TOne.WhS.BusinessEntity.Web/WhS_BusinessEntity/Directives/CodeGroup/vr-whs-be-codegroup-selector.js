'use strict';

app.directive('vrWhsBeCodegroupSelector', ['WhS_BE_CodeGroupAPIService', 'WhS_BE_CodeGroupService', 'UtilsService', '$compile', 'VRUIUtilsService',
    function (WhS_BE_CodeGroupAPIService, WhS_BE_CodeGroupService, UtilsService, $compile, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                onselectionchanged: '=',
                onselectitem: "=",
                ondeselectitem: "=",
                ondeselectallitems: "=",
                selectedvalues: "=",
                type: "=",
                label: "@",
                ismultipleselection: "@",
                hideselectedvaluessection: '@',
                isrequired: '@',
                isdisabled: "=",
                showaddbutton: '@',
                hidelabel: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];
                ctrl.datasource = [];

                ctrl.datasource = [];
                var ctor = new codeGroupCtor(ctrl, $scope, WhS_BE_CodeGroupAPIService, $attrs);
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

            return  '<vr-select ' + multipleselection + ' datasource="ctrl.datasource"  isrequired="ctrl.isrequired"  ' + hideselectedvaluessection + ' selectedvalues="ctrl.selectedvalues" ' + disabled +
                        ' onselectionchanged="ctrl.onselectionchanged" datatextfield="Name" datavaluefield="CodeGroupId" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ondeselectallitems="ctrl.ondeselectallitems" '
                       + 'entityname="Code Group" ' + label + ' ' + addCliked + '> ' +
                    '</vr-select>';
        }
        function codeGroupCtor(ctrl, $scope, WhS_BE_RateTypeAPIService, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.addNewCodeGroup = function () {
                    var onCodeGroupAdded = function (CodeGroupObj) {
                        ctrl.datasource.push(CodeGroupObj);
                        if ($attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(CodeGroupObj);
                        else
                            ctrl.selectedvalues = CodeGroupObj;
                    };
                    WhS_BE_RateTypeService.addCodeGroup(onCodeGroupAdded);
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

                    return WhS_BE_CodeGroupAPIService.GetAllCodeGroups().then(function (response) {
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
        }

        return directiveDefinitionObject;
    }]);

