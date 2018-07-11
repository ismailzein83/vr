'use strict';

app.directive('demoModuleCustomerinfo', ['VRNotificationService', 'Demo_Module_CustomerInfoAPIService', 'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, Demo_Module_CustomerInfoAPIService, UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new CustomerInfo($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Elements/CallCenterCustomer/Directives/Templates/CustomerInfoTemplate.html"
    };

    function CustomerInfo(ctrl, $scope, attrs) {

        
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            defineAPI();

        };
        

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                return Demo_Module_CustomerInfoAPIService.GetCustomerInfo().then(function (response) {
                    if (response != null) {
                        ctrl.customerName = response.Name;
                        ctrl.customerGender = response.Gender;
                        ctrl.customerAge = response.Age;
                        ctrl.customerAddress = response.Address;
                        ctrl.customerNumber = response.MobileNumber;
                        ctrl.customerPhoto = response.Photo;


                      
                        ctrl.customerEmail = response.Email;
                    }
                });
            };

            api.getData = function () {

            };

            if ($scope.onReady != null) {
                $scope.onReady(api);
            }
        };

    };
    return directiveDefinitionObject;

}]);