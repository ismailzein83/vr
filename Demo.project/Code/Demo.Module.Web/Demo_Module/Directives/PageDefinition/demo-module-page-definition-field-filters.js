
app.directive('demoModulePageDefinitionFieldFilters', ['VRNotificationService',  'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, UtilsService, VRUIUtilsService) {
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

            var fieldFiltersSelector = new FieldFiltersSelector(ctrl, $scope, $attrs);
            fieldFiltersSelector.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {

            return getFieldFiltersTemplate(attrs);
        }
    };

    function getFieldFiltersTemplate(attrs) {


        var multipleselection = "";
        var label = "Field Filters";
        if (attrs.ismultipleselection != undefined) {
            label = "Field Filters";
            multipleselection = "ismultipleselection";
        }

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns vr-disabled="ctrl.isdisabled" colnum="{{ctrl.normalColNum}}"  haschildcolumns  ><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Title" datavaluefield="Name" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="FieldFilters" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '>'

            + '</vr-select></vr-columns>';
    };


    function FieldFiltersSelector(ctrl, $scope, attrs) {


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

            api.load = function (payload) {
                console.log(payload.FieldFilters)

                selectorAPI.clearDataSource();

                var FieldFilters = [];
                
                if (payload != undefined) {
                    if (payload.Fields != undefined) {
                        for (var i = 0; i < payload.Fields.length; i++) {
                            var filter = { Name: payload.Fields[i].Name, Title: payload.Fields[i].Title }
                            ctrl.datasource.push(payload.Fields[i]);
                        } 
                    }
                    if (payload.FieldFilters != undefined) {
                         FieldFilters = [];
                         for (var j = 0; j < payload.FieldFilters.length; j++) {
                             var filter = payload.FieldFilters[j];
                             FieldFilters.push(filter); 

                         } VRUIUtilsService.setSelectedValues(FieldFilters, 'Name', attrs, ctrl);

                    }
                   
                }
            };

            api.getSelectedFilters = function () {
                
                return VRUIUtilsService.getIdSelectedIds('Name', attrs, ctrl);
            };
            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        };

        this.initializeController = initializeController;
    };
    return directiveDefinitionObject;

}]);