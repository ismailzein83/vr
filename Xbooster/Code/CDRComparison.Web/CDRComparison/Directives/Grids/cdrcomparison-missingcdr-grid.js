(function (app) {

    'use strict';

    MissingCDRGridDirective.$inject = ['CDRComparison_MissingCDRAPIService', 'VRNotificationService', 'DataGridRetrieveDataEventType'];

    function MissingCDRGridDirective(CDRComparison_MissingCDRAPIService, VRNotificationService, DataGridRetrieveDataEventType) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var missingCDRGrid = new MissingCDRGrid($scope, ctrl, $attrs);
                missingCDRGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/CDRComparison/Directives/Grids/Templates/MissingCDRGridTemplate.html'
        };

        function MissingCDRGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.missingCDRs = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady, retrieveDataContext) {
                    return CDRComparison_MissingCDRAPIService.GetFilteredMissingCDRs(dataRetrievalInput).then(function (response) {
                        
                        if (retrieveDataContext.eventType == DataGridRetrieveDataEventType.ExternalTrigger) {
                            if (response.Summary != undefined) {
                                gridAPI.setSummary(response.Summary);
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

    app.directive('cdrcomparisonMissingcdrGrid', MissingCDRGridDirective);

})(app);