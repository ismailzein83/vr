'use strict';
app.directive('vrSummaryBi', ['BIDataAPIService', 'BIUtilitiesService', 'BIVisualElementService1', 'VRModalService', function (BIDataAPIService, BIUtilitiesService, BIVisualElementService1, VRModalService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            settings: '=',
            filter: '=',
            previewmode: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var retrieveDataOnLoad = $scope.$parent.$eval($attrs.retrievedataonload);

            var biSummary = new BISummary(ctrl, ctrl.settings, retrieveDataOnLoad, BIDataAPIService, BIVisualElementService1);
            biSummary.initializeController();

            biSummary.defineAPI();
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
        template: function (element, attrs) {
            return getSummaryTemplate(attrs.previewmode);
            return
        }

    };

    function getSummaryTemplate(previewmode) {
        console.log(previewmode);
        if (previewmode != 'true') {
            return '<vr-label></vr-label>';
        }
        else
            return '';



    }
    function BISummary(ctrl, settings, retrieveDataOnLoad, BIDataAPIService, BIVisualElementService1) {

        var summaryAPI;

        function initializeController() {

            ctrl.onGridReady = function (api) {
                summaryAPI = api;
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
                                BIUtilitiesService.fillDateTimeProperties(response, ctrl.filter.fromDate, ctrl.filter.toDate, true);
                            refreshDataGrid(response);
                        });
        }

        function refreshDataGrid(response) {
            ctrl.data.length = 0;
            summaryAPI.addItemsToSource(response);
        }
        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }




    return directiveDefinitionObject;
}]);

