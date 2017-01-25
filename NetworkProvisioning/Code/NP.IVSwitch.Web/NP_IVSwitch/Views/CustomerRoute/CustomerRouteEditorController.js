(function (appControllers) {

    "use strict";

    CustomerRouteEditorController.$inject = ['$scope', 'NP_IVSwitch_CustomerRouteAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService'];

    function CustomerRouteEditorController($scope, npIvSwitchCustomerRouteApiService, vrNotificationService, utilsService, vrNavigationService) {

        var gridApi;
        var isEditMode;
        var destinationCode;
        var customerId;
        var customerRouteEntity;

        loadParameters();

        defineScope();

        load();

        setTitle();

        function loadParameters() {
            var parameters = vrNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                destinationCode = parameters.Destination;
                customerId = parameters.CustomerId;
            }
            isEditMode = (destinationCode != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                //To Do
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.onGridReady = function (api) {
                gridApi = api;
                gridApi.load(getFilterObject());
            };
        }
        function getFilterObject() {
            return  {
                CustomerId: customerId,
                Destination: destinationCode
            };
        }

        function load() {

        }

        function setTitle() {
            if (isEditMode) {
                var description = (customerRouteEntity != undefined) ? customerRouteEntity.Description : null;
                $scope.title = utilsService.buildTitleForUpdateEditor(description, 'CustomerRoute');
            }
            else {
                $scope.title = utilsService.buildTitleForAddEditor('CustomerRoute');
            }
        }

    }
    appControllers.controller('NP_IVSwitch_CustomerRouteEditorController', CustomerRouteEditorController);

})(appControllers);