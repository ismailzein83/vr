'use strict';


app.directive('vrDatagridBi', ['BIDataAPIService', 'BIUtilitiesService', 'BIVisualElementService1', 'VRModalService', function (BIDataAPIService, BIUtilitiesService, BIVisualElementService1, VRModalService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            settings: '=',
            filter:'='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var retrieveDataOnLoad = $scope.$parent.$eval($attrs.retrievedataonload);

            var biDataGrid = new BIDataGrid(ctrl, ctrl.settings, retrieveDataOnLoad, BIDataAPIService, BIVisualElementService1);
            biDataGrid.initializeController();

            biDataGrid.defineAPI();
            $scope.openReportEntityModal = function (item) {

                BIUtilitiesService.openEntityReport(item.EntityType, item.EntityId, item.EntityName);

            }

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                }
            }
        },
        template: function () {
            return '<vr-datagrid datasource="ctrl.data" on-ready="ctrl.onGridReady" maxheight="300px">'
                                        + '<vr-datagridcolumn ng-show="ctrl.isTopEntities" headertext="ctrl.entityType.description" field="\'EntityName\'" isclickable="\'true\'" \ onclicked="openReportEntityModal"></vr-datagridcolumn>'
                                        + '<vr-datagridcolumn ng-show="ctrl.isDateTimeGroupedData" headertext="\'Time\'" field="\'dateTimeValue\'"></vr-datagridcolumn>'
                                        + '<vr-datagridcolumn ng-repeat="measureType in ctrl.measureTypes" headertext="measureType" field="\'Values[\' + $index + \']\'" type="\'Number\'"></vr-datagridcolumn>'
                                    + '</vr-datagrid>';
        }

    };

    function BIDataGrid(ctrl, settings, retrieveDataOnLoad, BIDataAPIService, BIVisualElementService1) {

        var gridAPI;

        function initializeController() {

            ctrl.onGridReady = function (api) {
                gridAPI = api;
                if (retrieveDataOnLoad)
                    retrieveData();
            }

            ctrl.entityType = settings.EntityType;
            ctrl.measureTypes = settings.MeasureTypes;
            ctrl.data = [];
        }

        function defineAPI() {
            var api = {};
            api.retrieveData = retrieveData;
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function retrieveData() {
            return BIVisualElementService1.retrieveData1(ctrl, settings)
                        .then(function (response) {
                            if (ctrl.isDateTimeGroupedData)
                                BIUtilitiesService.fillDateTimeProperties(response, ctrl.filter.timeDimensionType.value, ctrl.filter.fromDate, ctrl.filter.toDate, true);
                            refreshDataGrid(response);
                        });
        }

        function refreshDataGrid(response) {
            ctrl.data.length = 0;
            gridAPI.addItemsToSource(response);
        }
        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }




    return directiveDefinitionObject;
}]);

