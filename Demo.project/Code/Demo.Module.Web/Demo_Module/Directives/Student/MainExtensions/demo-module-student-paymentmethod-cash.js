"use strict";

app.directive("demoModuleStudentPaymentmethodCash", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new CashMethod($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Demo_Module/Directives/Student/MainExtensions/Templates/CashMethodTemplate.html"
        };

        function CashMethod($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            var currencyDirectiveApi;
            var countryDirectiveApi;

            var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();

                $scope.scopeModel.onCurrencyDirectiveReady = function (api) {
                    currencyDirectiveApi = api;
                    currencyReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCountryDirectiveReady = function (api) {
                    countryDirectiveApi = api;
                    countryReadyPromiseDeferred.resolve();
                };
            }
           

            function defineAPI() {
                var api = {};
                var paymentPayload;
                api.load = function (payload) {
                    if (payload != undefined) {
                        paymentPayload = payload.paymentMethodEntity;
                    }
                    if (payload != undefined && payload.paymentMethodEntity != undefined) {
                        $scope.scopeModel.amount = payload.paymentMethodEntity.Amount;

                    }
                    var promises = [];
                    console.log("cash")

                    function loadCountrySelector() {
                        return countryDirectiveApi.load();
                    }
                    function loadCurrencyThenCountrySelectors() {
                        var promiseDeffered = UtilsService.createPromiseDeferred();

                        currencyDirectiveApi.load(paymentPayload).then(function (response) {
                            loadCountrySelector().then(function () {
                                promiseDeffered.resolve();
                            });
                        });


                        return promiseDeffered.promise;
                    }
                 
                    promises.push(loadCurrencyThenCountrySelectors());
                    return UtilsService.waitMultiplePromises(promises);
                };
            

             
          
                api.getData = function () {var currencyData=currencyDirectiveApi.getSelectedIds();
                    return {
                        $type: "Demo.Module.MainExtension.Student.CashMethod,Demo.Module.MainExtension",
                        Amount: $scope.scopeModel.amount,
                        CurrencyID: currencyData.ID,
                        CurrencyName: currencyData.Name


                    };
                };
                countryReadyPromiseDeferred.promise.then(function (response) {
                    currencyReadyPromiseDeferred.promise.then(function (response) {
                        if (ctrl.onReady != null)
                            ctrl.onReady(api);
                    });
                });
            }
        }

        return directiveDefinitionObject;

    }
]);