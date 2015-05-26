'use strict';


app.directive('vrBiDatagrid', ['BIAPIService', 'BIUtilitiesService', 'BIVisualElementService', function (BIAPIService, BIUtilitiesService, BIVisualElementService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            settings: '=',
            timedimensiontype: '=',
            fromdate: '=',
            todate: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var retrieveDataOnLoad = $scope.$parent.$eval($attrs.retrievedataonload);

            var biDataGrid = new BIDataGrid(ctrl, ctrl.settings, retrieveDataOnLoad, BIAPIService, BIVisualElementService);
            biDataGrid.initializeController();

            biDataGrid.defineAPI();

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
                                        + '<vr-datagridcolumn ng-show="ctrl.isTopEntities" headertext="ctrl.entityType.description" field="\'EntityName\'"></vr-datagridcolumn>'
                                        + '<vr-datagridcolumn ng-show="ctrl.isDateTimeGroupedData" headertext="\'Time\'" field="\'dateTimeValue\'"></vr-datagridcolumn>'
                                        + '<vr-datagridcolumn ng-repeat="measureType in ctrl.measureTypes" headertext="measureType.description" field="\'Values[\' + $index + \']\'" type="\'Number\'"></vr-datagridcolumn>'
                                    + '</vr-datagrid>';
        }

    };

    function BIDataGrid(ctrl, settings, retrieveDataOnLoad, BIAPIService, BIVisualElementService) {

        var gridAPI;

        function initializeController() {            

            ctrl.onGridReady = function (api) {
                gridAPI = api;
                if (retrieveDataOnLoad)
                    retrieveData();
            }

            ctrl.entityType = settings.entityType;
            ctrl.measureTypes = settings.measureTypes;
        }

        function defineAPI() {
            var api = {};
            api.retrieveData = retrieveData;
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function retrieveData() {
            return BIVisualElementService.retrieveData(ctrl, settings)
                        .then(function (response) {
                            if (ctrl.isDateTimeGroupedData)
                                BIUtilitiesService.fillDateTimeProperties(response, ctrl.timedimensiontype.value, ctrl.fromdate, ctrl.todate, true);
                            refreshDataGrid(response);
                        });
        }

        function refreshDataGrid(response) {
            ctrl.data = response;
        }

        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }




    return directiveDefinitionObject;
}]);

