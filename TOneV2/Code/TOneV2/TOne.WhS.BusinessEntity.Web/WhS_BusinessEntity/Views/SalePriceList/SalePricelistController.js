(function (appControllers) {

    "use strict";

    salePricelistController.$inject = ['$scope', 'VR_BE_SalePricelistAPIService', 'VRNotificationService'];

    function salePricelistController($scope, VR_BE_SalePricelistAPIService, VRNotificationService) {


        var gridApi;
        var filter = {};

        defineScope();

        load();

        function defineScope() {
            $scope.salepricelist = [];

            $scope.onGridReady = function (api) {
                gridApi = api;
                return gridApi.retrieveData(filter);
            };


            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_BE_SalePricelistAPIService.GetFilteredSalePricelists(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };




        };

        

        function load() {

        }

    }

    appControllers.controller('WhS_BE_SalePricelistController', salePricelistController);
})(appControllers);