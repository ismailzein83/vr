(function (app) {

    'use strict';

    AnumberSuppliercodeGrid.$inject = ['WhS_BE_ANumberSupplierCodeAPIService', 'VRNotificationService'];

    function AnumberSuppliercodeGrid(WhS_BE_ANumberSupplierCodeAPIService, VRNotificationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var supplierCodeGrid = new SupplierCodeGrid($scope, ctrl, $attrs);
                supplierCodeGrid.initialize();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/ANumber/Runtime/Templates/ANumberSupplierCodeGridTemplate.html'
        };

        function SupplierCodeGrid($scope, ctrl, $attrs) {
            this.initialize = initialize;

            var gridAPI;

            function initialize() {
                ctrl.supplierCodes = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_BE_ANumberSupplierCodeAPIService.GetFilteredANumberSupplierCodes(dataRetrievalInput).then(function (response) {
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

    app.directive('whsBeAnumberSuppliercodeGrid', AnumberSuppliercodeGrid);

})(app);
