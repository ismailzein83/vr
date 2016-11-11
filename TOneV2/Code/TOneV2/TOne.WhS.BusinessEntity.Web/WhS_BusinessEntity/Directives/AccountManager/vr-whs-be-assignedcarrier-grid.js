(function (app) {

    'use strict';

    AssignedCarrierGrid.$inject = ['WhS_BE_AccountManagerAPIService', 'VRNotificationService'];

    function AssignedCarrierGrid(WhS_BE_AccountManagerAPIService, VRNotificationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var assignedCarrierGrid = new CarrierGrid($scope, ctrl, $attrs);
                assignedCarrierGrid.initialize();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/AccountManager/Templates/AssignedCarrierGrid.html'
        };

        function CarrierGrid($scope, ctrl, $attrs) {
            this.initialize = initialize;

            var gridAPI;

            function initialize() {
                ctrl.assignedCarriers = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_BE_AccountManagerAPIService.GetFilteredAssignedCarriers(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            response.Data = getMappedAssignedCarriers(response.Data);
                        }
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

            function getMappedAssignedCarriers(assignedCarriers) {
                var mappedCarriers = [];

                angular.forEach(assignedCarriers, function (item) {
                    var gridObject = {
                        Entity: { CarrierAccountId: item.Entity.CarrierAccountId },
                        CarrierName: item.CarrierName,
                        IsCustomerAssigned: item.IsCustomerAssigned,
                        IsSupplierAssigned: item.IsSupplierAssigned,
                        IsCustomerInDirect: (item.IsCustomerInDirect) ? ' (Indirect)' : '',
                        IsSupplierInDirect: (item.IsSupplierInDirect) ? ' (Indirect)' : '',
                    };

                    mappedCarriers.push(gridObject);
                });

                return mappedCarriers;
            }
        }

        return directiveDefinitionObject;
    }

    app.directive('vrWhsBeAssignedcarrierGrid', AssignedCarrierGrid);

})(app);
