
app.directive('demoModuleSchoolSelector', ['VRNotificationService', 'Demo_Module_SchoolAPIService', 'Demo_Module_SchoolService', 'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, Demo_Module_SchoolAPIService, Demo_Module_SchoolService, UtilsService, VRUIUtilsService) {
    'use strict';
    
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
            isdisabled:'='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var schoolSelector = new SchoolSelector(ctrl, $scope, $attrs);
            schoolSelector.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
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


    function SchoolSelector(ctrl, $scope, attrs) {

        var selectorAPI;

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };

        };

        function defineAPI() {
            var api = {};
            var secondapi={};

            secondapi.consolelog = function () {

                console.log("secondapi.consolelog");
            };

            api.load = function (payload) { //payload is an object that has selectedids and filter

                selectorAPI.clearDataSource();

                var selectedIds;
                var filter;
                if (payload != undefined) {
                    if (payload.selectedIds != undefined) {
                        selectedIds = [];
                        selectedIds.push(payload.selectedIds);
                        console.log(selectedIds)
                    }
                    filter = payload.filter;
                }
                return Demo_Module_SchoolAPIService.GetSchoolsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]); 
                        }
                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'SchoolId', attrs, ctrl);
                        }
                    }
                });
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('SchoolId', attrs, ctrl);
            };

            api.clear = function () {
                selectorAPI.clearDataSource();

            }
            if (ctrl.onReady != null)
                ctrl.onReady(api,secondapi);
        };

        this.initializeController = initializeController;
    };
    return directiveDefinitionObject;

}]);