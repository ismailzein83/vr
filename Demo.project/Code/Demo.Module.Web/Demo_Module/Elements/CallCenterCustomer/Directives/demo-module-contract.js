'use strict';

app.directive('demoModuleContract', ['VRNotificationService', 'Demo_Module_ContractAPIService', 'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, Demo_Module_ContractAPIService, UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new Contract($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        //controllerAs: 'ctrl',
        //bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Elements/CallCenterCustomer/Directives/Templates/ContractTemplate.html"
    };

    function Contract($scope, attrs) {

        var gridApi;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel ={};
            $scope.scopeModel.onGridReady = function(api) {
                gridApi = api;

                if ($scope.onReady != undefined && typeof ($scope.onReady) == "function") {
                    $scope.onReady(defineAPI());
                }
            }


        };

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                gridApi.load();

                return Demo_Module_ContractAPIService.GetContract().then(function (response) {
                    if (response != null) {
                        $scope.scopeModel.MobileNumber = response.MobileNumber;
                        $scope.scopeModel.MSISDN = response.MSISDN;
                        $scope.scopeModel.RatePlan = response.RatePlan;
                    }
                });
            };

            api.getData = function () {

            };

            return api;
        };

    };
    return directiveDefinitionObject;

}]);