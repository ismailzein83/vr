app.directive('demoModuleSchoolTrialSelector', ['VRNotificationService', 'Demo_Module_SchoolAPIService', 'Demo_Module_SchoolService', 'UtilsService', 'VRUIUtilsService',
    function (VRNotificationService, Demo_Module_SchoolAPIService, Demo_Module_SchoolService, UtilsService, VRUIUtilsService) {
        'use strict'
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
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

                var schoolSelector = new schoolSelector(ctrl, $scope, $attrs)
                schoolSelector.initializeController();


            },

            controllerAs: 'ctrl',
            bindtocontroller: true,
            template: function (element, attrs) {
                return getSchoolTemplate(attrs);
            }
        };

        function getSchoolTemplate(attrs) {

            var multipleselection = "";
            var label = "School";
            if (attrs.ismultipleselection != undefined) {
                label = "School";
                multipleselection = "ismultipleselection";
            }

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

            return '<vr-columns vr-disabled="ctrl.isdisabled" colnum="{{ctrl.normalColNum}}"  haschildcolumns  ><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Name" datavaluefield="SchoolId" isrequired="ctrl.isrequired"'
                + ' label="' + label + '" ' + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="School" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '>'

                + '</vr-select></vr-columns>';
        };





    }])