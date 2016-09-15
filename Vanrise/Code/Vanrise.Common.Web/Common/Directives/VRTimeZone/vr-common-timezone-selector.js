'use strict';
app.directive('vrCommonTimezoneSelector', ['VRCommon_VRTimeZoneAPIService', 'VRCommon_VRTimeZoneService', 'UtilsService', 'VRUIUtilsService', function (VRCommon_VRTimeZoneAPIService, VRCommon_VRTimeZoneService, UtilsService, VRUIUtilsService) {

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

            $scope.addNewTimeZone = function () {
                var onTimeZoneAdded = function (timeZoneObj) {
                    ctrl.datasource.push(timeZoneObj.Entity);
                    if ($attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(timeZoneObj.Entity);
                    else
                        ctrl.selectedvalues = timeZoneObj.Entity ;
                };
                VRCommon_VRTimeZoneService.addTimeZone(onTimeZoneAdded);
            }

            ctrl.haspermission = function () {
                return VRCommon_VRTimeZoneAPIService.HasAddVRTimeZonePermission();
            };

            var ctor = new timeZoneCtor(ctrl, $scope, $attrs);
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
            return getTimeZoneTemplate(attrs);
        }

    };

    function getTimeZoneTemplate(attrs) {

        var multipleselection = "";
        var label = "TimeZone";
        if (attrs.ismultipleselection != undefined) {
            label = "TimeZones";
            multipleselection = "ismultipleselection";
        }

        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewTimeZone"';

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns colnum="{{ctrl.normalColNum}}"    ><vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="TimeZoneId" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="TimeZone" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" haspermission="ctrl.haspermission"' + hideremoveicon + '></vr-select></vr-columns>'
    }

    function timeZoneCtor(ctrl, $scope, attrs) {

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var selectedIds;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                }

                return getTimeZonesInfo(attrs, ctrl, selectedIds);
            }
            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('TimeZoneId', attrs, ctrl);
            }             
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    function getTimeZonesInfo(attrs, ctrl, selectedIds) {
        return VRCommon_VRTimeZoneAPIService.GetVRTimeZonesInfo().then(function (response) {
            ctrl.datasource.length = 0;
            angular.forEach(response, function (itm) {
                ctrl.datasource.push(itm);
            });

            if (selectedIds != undefined) {
                VRUIUtilsService.setSelectedValues(selectedIds, 'TimeZoneId', attrs, ctrl);
            }
        });
    }

    return directiveDefinitionObject;
}]);