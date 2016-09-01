'use strict';

app.directive('retailBeServicetypeGrid', ['Retail_BE_ServiceTypeAPIService', 'Retail_BE_ServiceTypeService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService',
    function (Retail_BE_ServiceTypeAPIService, Retail_BE_ServiceTypeService, UtilsService, VRUIUtilsService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var serviceTypeGrid = new ServiceTypeGrid($scope, ctrl, $attrs);
            serviceTypeGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/ServiceType/Templates/ServiceTypeGridTemplate.html'
    };

    function ServiceTypeGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.serviceTypes = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_ServiceTypeAPIService.GetFilteredServiceTypes(dataRetrievalInput).then(function (response) {
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

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {
            $scope.scopeModel.menuActions.push({
                name: 'Edit',
                clicked: editServiceType,
                haspermission: hasEditServiceTypePermission
            });
        }
        function hasEditServiceTypePermission() {
            return Retail_BE_ServiceTypeAPIService.HasUpdateServiceTypePermission();
        }
        function editServiceType(serviceType) {
            var onServiceTypeUpdated = function (updatedServiceType) {
                gridAPI.itemUpdated(updatedServiceType);
            };

            Retail_BE_ServiceTypeService.editServiceType(serviceType.Entity.ServiceTypeId, onServiceTypeUpdated);
        }
    }
}]);
