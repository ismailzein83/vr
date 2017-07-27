'use strict';
app.directive('demoModuleBuildingSelector', ['VRNotificationService', 'Demo_Module_BuildingAPIService', 'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, Demo_Module_BuildingAPIService, UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            onselectionchangeda: '=',
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

            $scope.addNewBuilding = function () {
                var onBuildingAdded = function (buildingObj) {
                    ctrl.datasource.push(buildingObj.Entity);
                    if ($attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(buildingObj.Entity);
                    else
                        ctrl.selectedvalues = buildingObj.Entity;
                };
                Demo_Module_BuildingService.addBuilding(onBuildingAdded);
            };

            var buildingSelector = new BuildingSelector(ctrl, $scope, $attrs);
            buildingSelector.initializeController();
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
            return getBuildingTemplate(attrs);
        }

    };

    function getBuildingTemplate(attrs) {

        var multipleselection = "";
        var label = "Building";
        if (attrs.ismultipleselection != undefined) {
            label = "Buildings";
            multipleselection = "ismultipleselection";
        }

        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewBuilding"';

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns  width="1/2row"  ><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Name" datavaluefield="BuildingId" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged=" ctrl.onselectionchangeda" entityName="Building" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '>'

            + '</vr-select></vr-columns>';
    }

    function BuildingSelector(ctrl, $scope, attrs) {

        var selectorAPI;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.onselectionch = function (selectedBuilding)
            {
                ctrl.onselectionchangeda(selectedBuilding);
            }

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };

            $scope.scopeModel.onCancelSearch = function (api) {
                $scope.scopeModel.searchbuilding = undefined;
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                selectorAPI.clearDataSource();

                var selectedIds;
                var filter;

                if (payload != undefined) {
                    selectedIds = [];
                    selectedIds.push(payload.selectedIds);
                    filter = payload.filter;
                }
                return getBuildingsInfo(attrs, ctrl, selectedIds, filter);
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('BuildingId', attrs, ctrl);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    function getBuildingsInfo(attrs, ctrl, selectedIds, filter) {
        return Demo_Module_BuildingAPIService.GetBuildingsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
            ctrl.datasource.length = 0;
            angular.forEach(response, function (itm) {
                ctrl.datasource.push(itm);
            });

            if (selectedIds != undefined && selectedIds != null) {
                VRUIUtilsService.setSelectedValues(selectedIds, 'BuildingId', attrs, ctrl);
            }
        });
    }

    return directiveDefinitionObject;
}]);