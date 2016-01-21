'use strict';
app.directive('vrSecGroupSelector', ['VR_Sec_GroupAPIService', 'VR_Sec_UserService', 'UtilsService', 'VRUIUtilsService',
    function (VR_Sec_GroupAPIService, VR_Sec_UserService, UtilsService, VRUIUtilsService) {

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
                isdisabled: "="
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new groupCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

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
                return getGroupTemplate(attrs);
            }

        };


        function getGroupTemplate(attrs) {

            var multipleselection = "";

            var label = "Group";
            if (attrs.ismultipleselection != undefined) {
                label = "Groups";
                multipleselection = "ismultipleselection";
            }

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewUser"';

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="UserId" isrequired="ctrl.isrequired"'
                + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Group" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>'
        }

        function groupCtor(ctrl, $scope, attrs) {

            var selectorApi;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    defineAPI();
                }
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }

                    return VR_Sec_UserAPIService.GetUsers().then(function (response) {
                        ctrl.datasource.length = 0;
                        angular.forEach(response, function (itm) {
                            ctrl.datasource.push(itm);
                        });

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'UserId', attrs, ctrl);
                        }
                    });
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('UserId', attrs, ctrl);
                }
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);