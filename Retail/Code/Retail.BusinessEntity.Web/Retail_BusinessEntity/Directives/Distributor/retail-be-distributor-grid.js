'use strict';

app.directive('retailBeDistributorGrid', ['Retail_BE_DistributorAPIService', 'Retail_BE_DistributorService', 'VRNotificationService', function (Retail_BE_DistributorAPIService, Retail_BE_DistributorService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var distributorGrid = new DistributorGrid($scope, ctrl, $attrs);
            distributorGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Distributor/Templates/DistributorGridTemplate.html'
    };

    function DistributorGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.distributors = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_DistributorAPIService.GetFilteredDistributors(dataRetrievalInput).then(function (response) {
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };

            defineMenuActions();
        }
        function defineAPI() {
            var api = {};

            api.loadGrid = function (query) {
                return gridAPI.retrieveData(query);
            };

            api.onDistributorAdded = function (addedDistributor) {
                gridAPI.itemAdded(addedDistributor);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {
            $scope.scopeModel.menuActions.push({
                name: 'Edit',
                clicked: editDistributor,
                haspermission: hasEditDistributorPermission
            });
        }
        function editDistributor(distributor) {
            var onDistributorUpdated = function (updatedDistributor) {
                gridAPI.itemUpdated(updatedDistributor);
            };
            Retail_BE_DistributorService.editDistributor(distributor.Entity.Id, onDistributorUpdated);
        }
        function hasEditDistributorPermission() {
            return Retail_BE_DistributorAPIService.HasUpdateDistributorPermission();
        }
    }
}]);
