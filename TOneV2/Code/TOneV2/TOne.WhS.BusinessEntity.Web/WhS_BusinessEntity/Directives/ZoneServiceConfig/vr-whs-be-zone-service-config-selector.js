﻿'use strict';
app.directive('vrWhsBeZoneServiceConfigSelector', [
    'WhS_BE_ZoneServiceConfigAPIService', 'WhS_BE_ZoneServiceConfigService', 'UtilsService', 'VRUIUtilsService',
    function (WhS_BE_ZoneServiceConfigAPIService, WhS_BE_ZoneServiceConfigService, UtilsService, VRUIUtilsService) {

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
                onblurdropdown: "=",
                hideremoveicon: '@',
                normalColNum: '@',
                customvalidate: '=',
                label: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                $scope.addNewZoneServiceConfig = function () {
                    var onZoneServiceConfigAdded = function (zoneServiceConfigObj) {
                        ctrl.datasource.push(zoneServiceConfigObj.Entity);
                        if ($attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(zoneServiceConfigObj.Entity);
                        else
                            ctrl.selectedvalues = zoneServiceConfigObj.Entity;
                    };
                    WhS_BE_ZoneServiceConfigService.addZoneServiceConfig(onZoneServiceConfigAdded);
                };
                ctrl.haspermission = function () {
                    return WhS_BE_ZoneServiceConfigAPIService.HasAddZoneServiceConfigPermission();
                };
                var ctor = new zoneServiceConfigCtor(ctrl, $scope, $attrs);
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
                return getBeZoneServiceConfigTemplate(attrs);
            }
        };
        function getBeZoneServiceConfigTemplate(attrs) {
            var multipleselection = "";
            var label = "Zone Service";
            if (attrs.ismultipleselection != undefined) {
                label = "Zone Services";
                multipleselection = "ismultipleselection";
            }

            if (attrs.label) {
                label = attrs.label;
            }

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewZoneServiceConfig"';

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

            return '<vr-columns colnum="{{ctrl.normalColNum}}"    ><vr-select on-ready="onSelectorReady" ' + multipleselection + '  datatextfield="Symbol" datavaluefield="ZoneServiceConfigId" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate"'
                + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Zone Service" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" onblurdropdown="ctrl.onblurdropdown" haspermission="ctrl.haspermission"' + hideremoveicon + '></vr-select></vr-columns>';
        }

        function zoneServiceConfigCtor(ctrl, $scope, attrs) {

            var selectorAPI;

            function initializeController() {
                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    selectorAPI.clearDataSource();

                    var selectedIds;
                    var filter;
                    var selectMinWeight
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                        selectMinWeight = payload.selectminweight;
                    }

                    var serializedFilter = {};
                    if (filter != undefined)
                        serializedFilter = UtilsService.serializetoJson(filter);
                    return getAllZoneServiceConfigs(attrs, ctrl, selectedIds, serializedFilter, selectMinWeight);
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('ZoneServiceConfigId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

            function getAllZoneServiceConfigs(attrs, ctrl, selectedIds, serializedFilter, selectMinWeight) {
                return WhS_BE_ZoneServiceConfigAPIService.GetAllZoneServiceConfigs(serializedFilter).then(function (response) {
                    var minWeigthItem;
                    angular.forEach(response, function (itm) {
                        if (minWeigthItem == undefined || minWeigthItem.Weight > itm.Weight)
                            minWeigthItem = itm;
                        ctrl.datasource.push(itm);
                    });
                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'ZoneServiceConfigId', attrs, ctrl);
                    }
                    if (selectedIds == undefined && selectMinWeight == true && minWeigthItem != undefined) {
                        selectorAPI.selectItem(minWeigthItem);
                    }
                });
            }
        }
        return directiveDefinitionObject;
    }]);

