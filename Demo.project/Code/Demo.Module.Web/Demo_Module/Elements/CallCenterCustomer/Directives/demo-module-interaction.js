'use strict';

app.directive('demoModuleInteraction', ['VRNotificationService', 'Demo_Module_InteractionService', 'Demo_Module_InteractionAPIService', 'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, Demo_Module_InteractionService, Demo_Module_InteractionAPIService, UtilsService, VRUIUtilsService) {

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
        templateUrl: "/Client/Modules/Demo_Module/Elements/CallCenterCustomer/Directives/Templates/InteractionTemplate.html"
    };

    function Contract($scope, attrs) {

        var gridApi;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.messages = [];
         
            Demo_Module_InteractionAPIService.GetMessages().then(function (response) {
                console.log(response);
                $scope.scopeModel.messages = response;

            });

            $scope.scopeModel.call = function () {
                Demo_Module_InteractionService.displayCall();

            }
        };

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
               
            };

            api.getData = function () {

            };

            return api;
        };

    };
    return directiveDefinitionObject;

}]);



