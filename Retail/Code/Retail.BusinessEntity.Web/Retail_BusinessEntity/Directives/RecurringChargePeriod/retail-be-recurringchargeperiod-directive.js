'use strict';

app.directive('retailBeRecurringchargeperiodDirective', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RecurringChargePeriod(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/RecurringChargePeriod/Templates/RecurringChargePeriodDirectiveTemplate.html"
        };

        function RecurringChargePeriod(ctrl, $scope, $attrs) {

            var recurringChargePeriodDirectiveAPI;
            var recurringChargePeriodDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRecurringChargePeriodDirectiveReady = function (api) {
                    recurringChargePeriodDirectiveAPI = api;
                    recurringChargePeriodDirectiveReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var fieldValue;
                    if (payload != undefined) {
                        fieldValue = payload.fieldValue;
                    }

                    var promises = [];

                    promises.push(loadRecurringChargePeriodDirective());

                    function loadRecurringChargePeriodDirective() {
                        var recurringChargePeriodDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        recurringChargePeriodDirectiveReadyPromiseDeferred.promise.then(function () {
                            var recurringChargePeriodDirectivePayload;

                            if (fieldValue != undefined) {
                                recurringChargePeriodDirectivePayload = {
                                    selectedValues: {
                                        RecurringChargePeriod: fieldValue
                                    }
                                };
                            }

                            VRUIUtilsService.callDirectiveLoad(recurringChargePeriodDirectiveAPI, recurringChargePeriodDirectivePayload, recurringChargePeriodDirectiveLoadDeferred);

                        });
                        return recurringChargePeriodDirectiveLoadDeferred.promise;

                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return recurringChargePeriodDirectiveAPI.getData();
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);