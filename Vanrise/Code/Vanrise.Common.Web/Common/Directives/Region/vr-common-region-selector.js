'use strict';
app.directive('vrCommonRegionSelector', ['VRCommon_RegionAPIService', 'VRCommon_RegionService', 'UtilsService', 'VRUIUtilsService', function (VRCommon_RegionAPIService, VRCommon_RegionService, UtilsService, VRUIUtilsService) {

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

            $scope.addNewRegion = function () {
                var onRegionAdded = function (regionObj) {
                    ctrl.datasource.push(regionObj.Entity);
                    if ($attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(regionObj.Entity);
                    else
                        ctrl.selectedvalues = regionObj.Entity;
                };
                VRCommon_RegionService.addRegion(onRegionAdded);
            };

            ctrl.haspermission = function () {
                return VRCommon_RegionAPIService.HasAddRegionPermission();
            };

            var ctor = new regionCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getRegionTemplate(attrs);
        }

    };

    function getRegionTemplate(attrs) {

        var multipleselection = "";
        var label = "Region";
        if (attrs.ismultipleselection != undefined) {
            label = "Regions";
            multipleselection = "ismultipleselection";
        }

        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewRegion"';

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns colnum="{{ctrl.normalColNum}}"    ><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Name" datavaluefield="RegionId" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Region" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" haspermission="ctrl.haspermission"' + hideremoveicon + '>'

            + '</vr-select></vr-columns>';
    }

    function regionCtor(ctrl, $scope, attrs) {

        var selectorAPI;

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onSelectorReady = function (api) {
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

                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    filter = payload.filter;
                }
                return getRegionsInfo(attrs, ctrl, selectedIds, filter);
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('RegionId', attrs, ctrl);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    function getRegionsInfo(attrs, ctrl, selectedIds, filter) {
        return VRCommon_RegionAPIService.GetRegionsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
            ctrl.datasource.length = 0;
            angular.forEach(response, function (itm) {
                ctrl.datasource.push(itm);
            });

            if (selectedIds != undefined) {
                VRUIUtilsService.setSelectedValues(selectedIds, 'RegionId', attrs, ctrl);
            }
        });
    }

    return directiveDefinitionObject;
}]);