"use strict";
app.directive("demoModuleCardPayment", ["UtilsService",
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var cardPayment = new CardPayment($scope, ctrl, $attrs);
                cardPayment.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Demo_Module/Directives/Student/MainExtensions/Payment/Template/CardPaymentTemplate.html"

        };
        function CardPayment($scope, ctrl, $attrs) {
            var cardPaymentAPI;
            this.initializeController = initializeController;
            function initializeController() {
  
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload != null) {
                        $scope.type = payload.Type;
                        $scope.CardNumber = payload.CardNumber;
                        $scope.CVV = payload.CVV;
                        $scope.ExpirationDate = payload.ExpirationDate;
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.Entities.Card,Demo.Module.Entities",
                        Type: $scope.type,
                        CardNumber: $scope.CardNumber,
                        CVV: $scope.CVV,
                        ExpirationDate: $scope.ExpirationDate
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);