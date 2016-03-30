(function (app) {

    'use strict';

    PartialMatchCDRGridDirective.$inject = ['CDRComparison_PartialMatchCDRAPIService', 'VRNotificationService'];

    function PartialMatchCDRGridDirective(CDRComparison_PartialMatchCDRAPIService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var partialMatchCDRGrid = new PartialMatchCDRGrid($scope, ctrl, $attrs);
                partialMatchCDRGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/CDRComparison/Directives/Grids/Templates/PartialMatchCDRGridTemplate.html'
        };

        function PartialMatchCDRGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.partialMatchCDRs = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return CDRComparison_PartialMatchCDRAPIService.GetFilteredPartialMatchCDRs(dataRetrievalInput).then(function (response) {
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

    app.directive('cdrcomparisonPartialmatchcdrGrid', PartialMatchCDRGridDirective);

})(app);