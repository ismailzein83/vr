﻿'use strict';

app.directive('retailBeRecurringchargeperiod', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/RecurringChargePeriod/Templates/RecurringChargePeriodTemplate.html"
        };

        function RecurringChargePeriod(ctrl, $scope, $attrs) {

            var recurringChargePeriodSelectorAPI;
            var recurringChargePeriodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRecurringChargePeriodSelectorReady = function (api) {
                    recurringChargePeriodSelectorAPI = api;
                    recurringChargePeriodSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedValues;
                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                    }

                    var promises = [];

                    promises.push(loadRecurringChargePeriodSelector());

                    function loadRecurringChargePeriodSelector() {
                        var recurringChargePeriodSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        recurringChargePeriodSelectorReadyDeferred.promise.then(function () {
                            var recurringChargePeriodSelectorPayload;

                            if (selectedValues != undefined && selectedValues.RecurringChargePeriod != undefined)
                                recurringChargePeriodSelectorPayload = { recurringChargePeriod: selectedValues.RecurringChargePeriod.Settings };

                            VRUIUtilsService.callDirectiveLoad(recurringChargePeriodSelectorAPI, recurringChargePeriodSelectorPayload, recurringChargePeriodSelectorLoadDeferred);

                        });
                        return recurringChargePeriodSelectorLoadDeferred.promise;

                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (entity) {
                    if (entity != undefined) {
                        entity.RecurringChargePeriod = {
                            $type: "Retail.BusinessEntity.Entities.FinancialRecurringChargePeriod, Retail.BusinessEntity.Entities",
                            Settings: recurringChargePeriodSelectorAPI.getData()
                        };
                    }
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);