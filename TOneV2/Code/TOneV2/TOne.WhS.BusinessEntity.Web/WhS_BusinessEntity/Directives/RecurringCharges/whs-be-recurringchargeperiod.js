'use strict';

app.directive('whsBeRecurringchargeperiod', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new recurringChargePeriodSelector(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/RecurringCharges/Templates/RecurringChargePeriodTemplate.html"
        };

        function recurringChargePeriodSelector(ctrl, $scope, $attrs) {

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
                    var promises = [];

                    promises.push(loadRecurringChargePeriodSelector());

                    function loadRecurringChargePeriodSelector() {
                        var recurringChargePeriodSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        recurringChargePeriodSelectorReadyDeferred.promise.then(function () {
                            var recurringChargePeriodSelectorPayload;
                            if (payload != undefined)
                                recurringChargePeriodSelectorPayload = { Settings: payload.Settings };

                            VRUIUtilsService.callDirectiveLoad(recurringChargePeriodSelectorAPI, recurringChargePeriodSelectorPayload, recurringChargePeriodSelectorLoadDeferred);

                            return recurringChargePeriodSelectorLoadDeferred.promise;
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.RecurringChargePeriod, TOne.WhS.BusinessEntity.Entities",
                        Settings: recurringChargePeriodSelectorAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);