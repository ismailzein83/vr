"use strict";

app.directive("whsBeRecurringchargeperiodsettingsOnceayear", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new OnceAYearRecurringCharge($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/RecurringCharges/MainExtensions/Templates/OnceAYearRecurringChargePeriodTemplate.html"

        };

        function OnceAYearRecurringCharge($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dayMonthDirectiveAPI;
            var dayMonthDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDayMonthDirectiveReady = function (api) {
                    dayMonthDirectiveAPI = api;
                    dayMonthDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    promises.push(loadDayMonthDirective());

                    function loadDayMonthDirective() {
                        var dayMonthDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        dayMonthDirectiveReadyDeferred.promise.then(function () {
                            var dayMonthDirectivePayload;
                            if (payload != undefined && payload.extendedSettings != undefined) {
                                dayMonthDirectivePayload = {
                                    selectedValues: payload.extendedSettings.Date,
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(dayMonthDirectiveAPI, dayMonthDirectivePayload, dayMonthDirectiveLoadDeferred);
                        });
                        return dayMonthDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.RecurringCharges.OnceAYearRecuringCharge ,TOne.WhS.BusinessEntity.MainExtensions",
                        Date: dayMonthDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);