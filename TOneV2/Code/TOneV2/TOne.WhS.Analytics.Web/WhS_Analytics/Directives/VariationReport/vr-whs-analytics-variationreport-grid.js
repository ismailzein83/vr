(function (app) {

    'use strict';

    VariationReportGridDirective.$inject = ['WhS_Analytics_VariationReportAPIService', 'VRNotificationService'];

    function VariationReportGridDirective(WhS_Analytics_VariationReportAPIService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var variationReportGrid = new VariationReportGrid($scope, ctrl, $attrs);
                variationReportGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Analytics/Directives/VariationReport/Templates/VariationReportGridTemplate.html'
        };

        function VariationReportGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataSource = [];
                $scope.scopeModel.dimensionTitle;
                $scope.scopeModel.timePeriodDefinitions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_Analytics_VariationReportAPIService.GetFilteredVariationReportRecords(dataRetrievalInput).then(function (response) {
                        if (response != null) {
                            $scope.scopeModel.dimensionTitle = response.DimensionTitle;

                            if (response.TimePeriods != null) {
                                $scope.scopeModel.timePeriodDefinitions.length = 0;
                                for (var i = 0; i < response.TimePeriods.length; i++) {
                                    $scope.scopeModel.timePeriodDefinitions.push(response.TimePeriods[i]);
                                }
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsAnalyticsVariationreportGrid', VariationReportGridDirective);

})(app);