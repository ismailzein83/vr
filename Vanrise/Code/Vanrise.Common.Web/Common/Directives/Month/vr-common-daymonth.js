(function (app) {

    'use strict';

    DayMonthDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRCommon_MonthEnum'];

    function DayMonthDirective(UtilsService, VRUIUtilsService, VRCommon_MonthEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                label: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DayMonth($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,

            templateUrl: "/Client/Modules/Common/Directives/Month/Templates/DayMonthTemplate.html"
        };

        function DayMonth($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var monthSelectorAPI;
            var monthSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var nbOfDaysSelectorAPI;
            var nbOfDaysSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var selectedMonthDeferred;

            $scope.scopeModel = {};
            $scope.scopeModel.days = [];

            function initializeController() {
                
                $scope.scopeModel.onMonthSelectorReady = function (api) {
                    monthSelectorAPI = api;
                    monthSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onNbOfDaysSelectorReady = function (api) {
                    nbOfDaysSelectorAPI = api;
                    nbOfDaysSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onMonthSelectorChanged = function (value) {
                    $scope.scopeModel.selectedDay = "";
                    if (value != undefined) {
                        if (selectedMonthDeferred != undefined)
                            selectedMonthDeferred.resolve();
                        else {
                            $scope.scopeModel.days.length = 0;
                            $scope.scopeModel.selectedDay = "";
                            for (var i = 1; i <= value.nbOfDays; i++)
                                $scope.scopeModel.days.push({ day: i, dayDescription: "" + i + "" });
                        }
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined && payload.selectedValues != undefined){
                        selectedMonthDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadNbOfDaysSelector());
                    }

                    promises.push(loadMonthSelector());
                    
                    function loadMonthSelector() {
                        var monthSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        monthSelectorReadyDeferred.promise.then(function () {
                            var monthSelectorPayload;
                            if (payload != undefined && payload.selectedValues != undefined) {
                                monthSelectorPayload = {
                                    selectedIds: payload.selectedValues.Month,
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(monthSelectorAPI, monthSelectorPayload, monthSelectorLoadDeferred);
                        });
                        return monthSelectorLoadDeferred.promise;
                    }

                    function loadNbOfDaysSelector() {
                        var nbOfDaysSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([selectedMonthDeferred.promise, nbOfDaysSelectorReadyDeferred.promise]).then(function () {
                            selectedMonthDeferred = undefined;
                            $scope.scopeModel.days.length = 0;
                            var selectedMonth = UtilsService.getEnum(VRCommon_MonthEnum, "value", payload.selectedValues.Month);
                            if (selectedMonth != undefined)
                            {
                                for (var i = 1; i <= selectedMonth.nbOfDays; i++) {
                                    $scope.scopeModel.days.push({ day: i, dayDescription: "" + i + "" });
                                }
                            }
                            if (payload.selectedValues.Day != undefined)
                            {
                                $scope.scopeModel.selectedDay = UtilsService.getItemByVal($scope.scopeModel.days, payload.selectedValues.Day, "day");
                            }
                            nbOfDaysSelectorLoadDeferred.resolve();
                        });
                        return nbOfDaysSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        Month: monthSelectorAPI.getSelectedIds(),
                        Day: $scope.scopeModel.selectedDay != undefined ? $scope.scopeModel.selectedDay.day : undefined
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }
    app.directive('vrCommonDaymonth', DayMonthDirective);
})(app);