
app.directive('vanriseToolsSchemaSelector', ['VRNotificationService', 'Vanrise_Tools_SchemaAPIService', 'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, Vanrise_Tools_SchemaAPIService, UtilsService, VRUIUtilsService) {
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

            var schemaSelector = new SchemaSelector(ctrl, $scope, $attrs);
            schemaSelector.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {

            return getSchemaTemplate(attrs);
        }
    };

    function getSchemaTemplate(attrs) {


        var multipleselection = "";
        var label = "Schema";
        if (attrs.ismultipleselection != undefined) {
            label = "Schema";
            multipleselection = "ismultipleselection";
        }

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns vr-disabled="ctrl.isdisabled" colnum="{{ctrl.normalColNum}}"  haschildcolumns  ><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Name" datavaluefield="SchemaId" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Schema" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '>'

            + '</vr-select></vr-columns>';
    };


    function SchemaSelector(ctrl, $scope, attrs) {


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

            api.load = function (payload) { //payload is an object that has selectedids and filter

                selectorAPI.clearDataSource();

                var selectedIds;
                var filter;
                if (payload != undefined) {
                    if (payload.selectedIds != undefined) {
                        selectedIds = [];
                        selectedIds.push(payload.selectedIds);
                    }
                    filter = payload.filter;
                }
                return Vanrise_Tools_SchemaAPIService.GetSchemasInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'SchemaId', attrs, ctrl);
                        }
                    }
                });
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('SchemaId', attrs, ctrl);
            };
            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        };

        this.initializeController = initializeController;
    };
    return directiveDefinitionObject;

}]);