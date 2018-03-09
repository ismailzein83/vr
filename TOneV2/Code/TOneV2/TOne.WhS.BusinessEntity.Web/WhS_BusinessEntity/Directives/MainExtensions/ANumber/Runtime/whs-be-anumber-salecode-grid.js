(function (app) {

    'use strict';

    AnumberSalecodeGrid.$inject = ['WhS_BE_AccountManagerAPIService', 'VRNotificationService'];

    function AnumberSalecodeGrid(WhS_BE_AccountManagerAPIService, VRNotificationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var saleCodeGrid = new SaleCodeGrid($scope, ctrl, $attrs);
                saleCodeGrid.initialize();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/ANumber/Runtime/Templates/ANumberSaleCodeGridTemplate.html'
        };

        function SaleCodeGrid($scope, ctrl, $attrs) {
            this.initialize = initialize;

            var gridAPI;

            function initialize() {
                ctrl.saleCodes = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_BE_AccountManagerAPIService.GetFilteredAssignedCarriers(dataRetrievalInput).then(function (response) {                      
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };
            }

            function getDirectiveAPI() {
                var directiveAPI = {};

                directiveAPI.loadGrid = function (query) {
                    return gridAPI.retrieveData(query);
                };

                return directiveAPI;
            }
        }

        return directiveDefinitionObject;
    }

    app.directive('whsBeAnumberSalecodeGrid', AnumberSalecodeGrid);

})(app);
