"use strict";

app.directive("whsBeRecurringchargeperiodsettingsMonthly", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new MonthlyRecurringCharge($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/RecurringCharges/MainExtensions/Templates/MonthlyRecurringChargePeriodTemplate.html"

        };

        function MonthlyRecurringCharge($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var nbOfDaysSelectorAPI;
            var nbOfDaysSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.days = [];

                $scope.scopeModel.onNbOfDaysSelectorReady = function (api) {
                    nbOfDaysSelectorAPI = api;
                    nbOfDaysSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    promises.push(loadNbOfDaysSelector());

                    function loadNbOfDaysSelector() {
                        var nbOfDaysSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        nbOfDaysSelectorReadyDeferred.promise.then(function () {
                            $scope.scopeModel.days.length = 0;
                            for (var i = 1; i <= 31; i++) {
                                $scope.scopeModel.days.push({ day: i, dayDescription: "" + i + "" });
                            }
                            if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.Day != undefined) {
                                $scope.scopeModel.selectedDay = UtilsService.getItemByVal($scope.scopeModel.days, payload.extendedSettings.Day, "day");
                            }
                            nbOfDaysSelectorLoadDeferred.resolve();
                        });
                        return nbOfDaysSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.RecurringCharges.MonthlyRecuringCharge ,TOne.WhS.BusinessEntity.MainExtensions",
                        Day: $scope.scopeModel.selectedDay!=undefined ? $scope.scopeModel.selectedDay.day : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);