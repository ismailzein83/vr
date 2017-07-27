"use strict";
app.directive("demoModuleCashPayment", ["UtilsService",
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var cashPayment = new CashPayment($scope, ctrl, $attrs);
                cashPayment.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Demo_Module/Directives/Student/MainExtensions/Payment/Template/CashPaymentTemplate.html"

        };
        function CashPayment($scope, ctrl, $attrs) {
            var cashPaymentAPI;
            this.initializeController = initializeController;
            function initializeController() {
                $scope.amount = null;
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.amount = payload.Amount;
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.Entities.Cash,Demo.Module.Entities",
                        Amount: $scope.amount
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);