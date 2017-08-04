(function (app) {

    'use strict';

    InvalidCDRGridDirective.$inject = ['CDRComparison_InvalidCDRAPIService', 'VRNotificationService'];

    function InvalidCDRGridDirective(CDRComparison_InvalidCDRAPIService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var invalidCDRGrid = new InvalidCDRGrid($scope, ctrl, $attrs);
                invalidCDRGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/CDRComparison/Directives/Grids/Templates/InvalidCDRGridTemplate.html'
        };

        function InvalidCDRGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.invalidCDRs = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return CDRComparison_InvalidCDRAPIService.GetFilteredInvalidCDRs(dataRetrievalInput).then(function (response) {
                        if (response != null && response.Data != null) {
                            if (response.Data.length > 0 && response.Summary != null) {
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

    app.directive('cdrcomparisonInvalidcdrGrid', InvalidCDRGridDirective);

})(app);