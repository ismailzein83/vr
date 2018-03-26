'use strict';
app.directive('demoModuleUniversitySelector', ['VRNotificationService', 'Demo_Module_UniversityAPIService', 'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, Demo_Module_UniversityAPIService, UtilsService, VRUIUtilsService) {

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
            normalColNum: '@',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            $scope.addNewUniversity = function () {
                var onUniversityAdded = function (universityObj) {
                    ctrl.datasource.push(universityObj.Entity);
                    if ($attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(universityObj.Entity);
                    else
                        ctrl.selectedvalues = universityObj.Entity;
                };
                Demo_Module_UniversityService.addUniversity(onUniversityAdded);
            };

            var universitySelector = new UniversitySelector(ctrl, $scope, $attrs);
            universitySelector.initializeController();
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
            return getUniversityTemplate(attrs);
        }

    };

    function getUniversityTemplate(attrs) {

        var multipleselection = "";
        var label = "University";
        if (attrs.ismultipleselection != undefined) {
            label = "University";
            multipleselection = "ismultipleselection";
        }

        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewUniversity"';

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns  colnum="{{ctrl.normalColNum}}"  ><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Name" datavaluefield="UniversityId" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged=" ctrl.onselectionchanged" entityName="University" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '>'
            + '</vr-select></vr-columns>';
    }

    function UniversitySelector(ctrl, $scope, attrs) {

        var selectorAPI;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.onselection = function (selectedUniversity) {
                ctrl.onselectionchanged(selectedUniversity);
            };

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };

            $scope.scopeModel.onCancelSearch = function (api) {
                $scope.scopeModel.searchUniversity = undefined;
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
                return getUniversitiesInfo(attrs, ctrl, selectedIds, filter);
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('UniversityId', attrs, ctrl);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    function getUniversitiesInfo(attrs, ctrl, selectedIds, filter) {
        return Demo_Module_UniversityAPIService.GetUniversitiesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
            ctrl.datasource.length = 0;
            angular.forEach(response, function (itm) {
                ctrl.datasource.push(itm);
            });
         
            if (selectedIds != undefined) {
               VRUIUtilsService.setSelectedValues(selectedIds, 'UniversityId', attrs, ctrl);
            }
        });
    }

    return directiveDefinitionObject;
}]);