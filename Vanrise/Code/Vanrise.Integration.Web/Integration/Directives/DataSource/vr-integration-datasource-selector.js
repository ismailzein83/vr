'use strict';
app.directive('vrIntegrationDatasourceSelector', ['VR_Integration_DataSourceAPIService', 'UtilsService', '$compile', 'VRUIUtilsService', 'VR_Integration_DataSourceService',
function (VR_Integration_DataSourceAPIService, UtilsService, $compile, VRUIUtilsService, VR_Integration_DataSourceService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            isdisabled: "=",
            onselectionchanged: '=',
            isrequired: "@",
            selectedvalues: '=',
            onaddclicked: '=',
            normalColNum: '@'

        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            ctrl.datasource = [];
            var ctor = new dataSourceCtor(ctrl, $scope, $attrs);
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
            return getTemplate(attrs);
        }

    };


    function getTemplate(attrs) {

        var multipleselection = "";
        if (attrs.ismultipleselection != undefined)
            multipleselection = "ismultipleselection";
        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";
        var disabled = "";
        var addDataSource="";
        if (attrs.adddatasource != undefined)
          addDataSource='onaddclicked="addDataSource"';

        return '<vr-columns colnum="{{ctrl.normalColNum}}" ><vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="DataSourceID" '
        + required + ' label="DataSource" datasource="ctrl.datasource" ' + addDataSource + ' selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged" vr-disabled="ctrl.isdisabled" "></vr-select></vr-columns>';

    }

    function dataSourceCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            $scope.addDataSource = function () {

                var onDataSourceAdded = function (dataSourceObj) {
                    ctrl.dataSources.push(dataSourceObj);
                };

                VR_Integration_DataSourceService.addDataSource(onDataSourceAdded);
            };
            var api = {};
            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('DataSourceID', $attrs, ctrl);
            };
            api.load = function (payload) {
                var filter = null;
                var selectedIds;
                if (payload != undefined) {
                    filter = payload.filter;
                    selectedIds = payload.selectedIds;
                }
                ctrl.datasource.length = 0;
                return VR_Integration_DataSourceAPIService.GetDataSources(UtilsService.serializetoJson(filter)).then(function (response) {
                    angular.forEach(response, function (item) {
                        ctrl.datasource.push(item);

                    });
                    if (selectedIds != undefined)
                        VRUIUtilsService.setSelectedValues(selectedIds, 'DataSourceID', $attrs, ctrl);

                });

            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);