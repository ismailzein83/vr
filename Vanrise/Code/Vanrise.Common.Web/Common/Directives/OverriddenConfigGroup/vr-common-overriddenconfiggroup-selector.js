'use strict';
app.directive('vrCommonOverriddenconfiggroupSelector', ['VRCommon_OverriddenConfigGroupAPIService', 'VRCommon_OverriddenConfigGroupService', 'UtilsService', 'VRUIUtilsService', function (VRCommon_OverriddenConfigGroupAPIService, VRCommon_OverriddenConfigGroupService, UtilsService, VRUIUtilsService) {

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
            hideremoveicon: '@',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.datasource = [];
            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            $scope.addNewOverriddenConfigGroup = function () {
                var onOverriddenConfigGroupAdded = function (overriddenConfigGroupObj) {
                    ctrl.datasource.push(overriddenConfigGroupObj);
                    if ($attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(overriddenConfigGroupObj);
                    else
                        ctrl.selectedvalues = overriddenConfigGroupObj;
                };
                VRCommon_OverriddenConfigGroupService.addOverriddenConfigGroup(onOverriddenConfigGroupAdded);
            };

            var ctor = new overriddenConfigGroupCtor(ctrl, $scope, $attrs);
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
            return getOverriddenConfigGroupTemplate(attrs);
        }

    };

    function getOverriddenConfigGroupTemplate(attrs) {

        var multipleselection = "";
        var label = "Overridden Configuration Group";
        if (attrs.ismultipleselection != undefined) {
            label = "Overridden Configuration Groups";
            multipleselection = "ismultipleselection";
        }

        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewOverriddenConfigGroup"';

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns colnum="{{ctrl.normalColNum}}"    ><vr-select  onokhandler="scopeModel.onOKSearch" oncancelhandler="scopeModel.onCancelSearch" on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Name" datavaluefield="OverriddenConfigurationGroupId" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="OverriddenConfigurationGroup" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + '>'
                + '<div><vr-row>'
                     + '<vr-columns width="fullrow">'
                        + ' <vr-label>Overridden Configuration Group</vr-label>'
                        + ' <vr-textbox value="scopeModel.searchOverriddenConfigurationGroup"></vr-textbox>'
                     + '</vr-columns>'
                     + '<vr-columns width="fullrow">'
                        + ' <vr-datetimepicker label="Date" isrequired type="date" value="scopeModel.testdate"></vr-datetimepicker>'
                     + '</vr-columns>'
                + ' </vr-row></div>'
            + '</vr-select></vr-columns>';
    }

    function overriddenConfigGroupCtor(ctrl, $scope, attrs) {

        var selectorAPI;

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };

            $scope.scopeModel.onOKSearch = function (api) {
                console.log($scope.scopeModel.searchOverriddenConfigurationGroup);
                console.log($scope.scopeModel.testdate);

            };
            $scope.scopeModel.onCancelSearch = function (api) {
                $scope.scopeModel.searchOverriddenConfigurationGroup = undefined;
            };
            $scope.scopeModel.customdata = [
                { id: 1, name: "test 1" },
                { id: 2, name: "test 2" }
            ];
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                selectorAPI.clearDataSource();

                var selectedIds;
                var filter;
               
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    filter = payload.filter;
                }
                return getOverriddenConfigurationGroupInfo(attrs, ctrl, selectedIds, filter);
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('OverriddenConfigurationGroupId', attrs, ctrl);
            };

            api.getAllOverriddenConfigurationGroup = function () {
                var allOverriddenConfigurationGroups;
                if (ctrl.datasource != undefined) {
                    allOverriddenConfigurationGroups = [];
                    for (var i = 0; i < ctrl.datasource.length; i++) {
                        var overriddenConfigurationGroup = ctrl.datasource[i];
                        allOverriddenConfigurationGroups.push(overriddenConfigurationGroup);
                    }
                }
                return allOverriddenConfigurationGroups;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    function getOverriddenConfigurationGroupInfo(attrs, ctrl, selectedIds, filter) {
        return VRCommon_OverriddenConfigGroupAPIService.GetOverriddenConfigurationGroupInfo(UtilsService.serializetoJson(filter)).then(function (response) {
            ctrl.datasource.length = 0;
            angular.forEach(response, function (itm) {
                ctrl.datasource.push(itm);
            });

            if (selectedIds != undefined) {
                VRUIUtilsService.setSelectedValues(selectedIds, 'OverriddenConfigurationGroupId', attrs, ctrl);
            }
        });
    }

    return directiveDefinitionObject;
}]);