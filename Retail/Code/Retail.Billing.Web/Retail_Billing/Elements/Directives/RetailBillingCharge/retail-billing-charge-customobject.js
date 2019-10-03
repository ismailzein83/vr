"use strict";

app.directive('retailBillingChargeCustomobject', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailBillingChargeDirectiveCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/Retail_Billing/Elements/Directives/RetailBillingCharge/Templates/RetailBillingChargeCustomObjectTemplate.html'
        };

        function retailBillingChargeDirectiveCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var chargeTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var chargeDirectiveAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onChargeDirectiveReady = function (api) {
                    chargeDirectiveAPI = api;
                    chargeTypeSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var charge;

                    if (payload != undefined)
                        charge = payload.fieldValue;

                    promises.push(loadChargeDirective());

                    function loadChargeDirective() {
                        var loadChargeTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        chargeTypeSelectorReadyPromiseDeferred.promise.then(function () {

                            var payload;
                            if (charge != undefined)
                                payload = {
                                    charge: charge
                                };

                            VRUIUtilsService.callDirectiveLoad(chargeDirectiveAPI, payload, loadChargeTypeSelectorPromiseDeferred);
                        });
                        return loadChargeTypeSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode({
                        promises: promises
                    });
                };

                api.getData = function () {
                    return chargeDirectiveAPI.getData();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };
        }

        return directiveDefinitionObject;
    }]);