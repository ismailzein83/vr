'use strict';
app.directive('demoModuleCollegeSelector', ['VRNotificationService', 'Demo_Module_CollegeAPIService', 'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, Demo_Module_CollegeAPIService, UtilsService, VRUIUtilsService) {

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

            $scope.addNewCollege = function () {
                var onCollegeAdded = function (collegeObj) {
                    ctrl.datasource.push(collegeObj.Entity);
                    if ($attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(collegeObj.Entity);
                    else
                        ctrl.selectedvalues = collegeObj.Entity;
                };
                Demo_Module_CollegeService.addCollege(onCollegeAdded);
            };

            var collegeSelector = new CollegeSelector(ctrl, $scope, $attrs);
            collegeSelector.initializeController();
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
            return getCollegeTemplate(attrs);
        }
    };

    function getCollegeTemplate(attrs) {

        var multipleselection = "";
        var label = "College";
        if (attrs.ismultipleselection != undefined) {
            label = "College";
            multipleselection = "ismultipleselection";
        }

        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewCollege"';

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns  width="1/2row"  ><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Name" datavaluefield="CollegeId" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged=" ctrl.onselectionchanged" entityName="College" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '>'
            + '</vr-select></vr-columns>';
    }

    function CollegeSelector(ctrl, $scope, attrs) {

        var selectorAPI;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.onselection = function (selectedCollege) {
                ctrl.onselectionchanged(selectedCollege);
            };

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };

            $scope.scopeModel.onCancelSearch = function (api) {
                $scope.scopeModel.searchCollege = undefined;
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
                return getCollegesInfo(attrs, ctrl, selectedIds, filter);
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('CollegeId', attrs, ctrl);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    function getCollegesInfo(attrs, ctrl, selectedIds, filter) {
        return Demo_Module_CollegeAPIService.GetCollegesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
            ctrl.datasource.length = 0;
            angular.forEach(response, function (itm) {
                ctrl.datasource.push(itm);
            });

            if (selectedIds != undefined) {
                VRUIUtilsService.setSelectedValues(selectedIds, 'CollegeId', attrs, ctrl);
            }
        });
    }

    return directiveDefinitionObject;
}]);