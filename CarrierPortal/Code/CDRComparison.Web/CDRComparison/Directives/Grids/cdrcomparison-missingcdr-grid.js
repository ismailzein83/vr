(function (app) {

    'use strict';

    MissingCDRGridDirective.$inject = ['CDRComparison_MissingCDRAPIService', 'VRNotificationService'];

    function MissingCDRGridDirective(CDRComparison_MissingCDRAPIService, VRNotificationService) {
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

                    function getDirectiveAPI() {

                        var directiveAPI = {};

                        directiveAPI.loadGrid = function (query) {
                            
                        }

                        directiveAPI.onUserAdded = function (userObject) {
                            gridAPI.itemAdded(userObject);
                        }

                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return CDRComparison_MissingCDRAPIService.GetFilteredMissingCDRs(dataRetrievalInput).then(function (response) {
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