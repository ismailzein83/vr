"use strict";

app.directive("whsSalepricelistNotificationScheduled", ["VRUIUtilsService", "UtilsService", "WhS_SalePricelistPeriodEnum",
    function (VRUIUtilsService, UtilsService, WhS_SalePricelistPeriodEnum) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DirectiveCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Scheduled/Templates/SalePricelistNotificationScheduledTemplate.html"
        };

        function DirectiveCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var customerSelectorAPI;
            var customerSelectorAPIReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onCustomerSelectorReady = function (api) {
                    customerSelectorAPI = api;
                    customerSelectorAPIReadyDeferred.resolve();
                };
                $scope.periods = UtilsService.getArrayEnum(WhS_SalePricelistPeriodEnum);

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var customerIds;

                    if (payload != undefined) {
                        if (payload.data != undefined) {
                            customerIds = payload.data.CustomerIds;
                            $scope.selectedPeriod = UtilsService.getItemByVal($scope.periods, payload.data.Period, 'value');
                        }
                    }

                    var promises = [];

                    var loadCustomerSelectorPromise = loadCustomerSelector();
                    promises.push(loadCustomerSelectorPromise);

                    function loadCustomerSelector() {
                        var CustomerSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        customerSelectorAPIReadyDeferred.promise.then(function () {

                            var customerSelectorPayload = {
                                selectedIds: customerIds
                            };
                            VRUIUtilsService.callDirectiveLoad(customerSelectorAPI, customerSelectorPayload, CustomerSelectorLoadDeferred);
                        });

                        return CustomerSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        $type: "TOne.WhS.BusinessEntity.BP.Arguments.SalePricelistEmailSenderProcessInput, TOne.WhS.BusinessEntity.BP.Arguments",
                        CustomerIds: customerSelectorAPI.getSelectedIds(),
                        Period: $scope.selectedPeriod.value
                    };
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);
