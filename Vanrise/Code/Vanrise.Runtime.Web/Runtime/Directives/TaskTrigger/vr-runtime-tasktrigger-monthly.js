'use strict';

app.directive('vrRuntimeTasktriggerMonthly', ['UtilsService', 'TimeSchedulerTypeEnum', 'DayOfMonthTypeEnum',
    function (UtilsService, TimeSchedulerTypeEnum, DayOfMonthTypeEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TaskTriggerMonthlyCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Runtime/Directives/TaskTrigger/Templates/TaskTriggerMonthly.html'
        };

        function TaskTriggerMonthlyCtor($scope, ctrl) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dayOfMonthTypeItems = UtilsService.getArrayEnum(DayOfMonthTypeEnum);
                $scope.scopeModel.specificDayType = DayOfMonthTypeEnum.SpecificDay.value;
                $scope.scopeModel.selectedDays = [];
                $scope.scopeModel.selectedTimes = [];

                $scope.scopeModel.addDay = function () {
                    var dayOfMonthTypeItem = UtilsService.getItemByVal($scope.scopeModel.dayOfMonthTypeItems, $scope.scopeModel.selectedDayOfMonthTypeItem.value, 'value');
                    var newDay = {
                        DayOfMonthType: dayOfMonthTypeItem.value,
                        DayOfMonthTypeDescription: dayOfMonthTypeItem.description,
                        SpecificDay: ($scope.scopeModel.specificDay != undefined) ? parseInt($scope.scopeModel.specificDay) : null
                    };

                    $scope.scopeModel.selectedDays.push(newDay);
                    $scope.scopeModel.selectedDayOfMonthTypeItem = undefined;
                    $scope.scopeModel.specificDay = undefined;
                };

                $scope.scopeModel.addTime = function () {
                    $scope.scopeModel.selectedTimes.push($scope.scopeModel.selectedTime);
                    $scope.scopeModel.selectedTime = undefined;
                };

                $scope.scopeModel.onDayOfMonthTypeSelectionChanged = function () {
                    $scope.scopeModel.specificDay = undefined;
                };

                $scope.scopeModel.validateNumberOfSelectedDays = function () {
                    if ($scope.scopeModel.selectedDays == undefined || $scope.scopeModel.selectedDays.length == 0)
                        return 'At least one day should be added';
                    return null;
                };

                $scope.scopeModel.validateDay = function () {
                    if ($scope.scopeModel.selectedDayOfMonthTypeItem == undefined)
                        return false;

                    switch ($scope.scopeModel.selectedDayOfMonthTypeItem.value) {
                        case DayOfMonthTypeEnum.LastDay.value:
                            for (var i = 0; i < $scope.scopeModel.selectedDays.length; i++) {
                                if ($scope.scopeModel.selectedDays[i].DayOfMonthType == DayOfMonthTypeEnum.LastDay.value)
                                    return false;
                            }
                            break;
                        case DayOfMonthTypeEnum.SpecificDay.value:
                            if (isNaN($scope.scopeModel.specificDay))
                                return false;

                            var specificDay = parseInt($scope.scopeModel.specificDay);
                            if (specificDay < 1 || specificDay > 28)
                                return false;

                            for (var i = 0; i < $scope.scopeModel.selectedDays.length; i++) {
                                if (specificDay == $scope.scopeModel.selectedDays[i].SpecificDay) {
                                    return false;
                                }
                            }
                            break;
                        default:
                            return false;
                    }
                    return true;
                };

                $scope.scopeModel.validateNumberOfSelectedTimes = function () {
                    if ($scope.scopeModel.selectedTimes == undefined || $scope.scopeModel.selectedTimes.length == 0)
                        return 'At least one value should be added';
                    return null;
                };

                $scope.scopeModel.validateTime = function () {
                    if ($scope.scopeModel.selectedTime == undefined)
                        return false;

                    for (var i = 0; i < $scope.scopeModel.selectedTimes.length; i++) {
                        if (UtilsService.compareEqualsTimes($scope.scopeModel.selectedTimes[i], $scope.scopeModel.selectedTime)) {
                            return false;
                        }
                    }

                    return true;
                };
 
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.data != undefined) {
                        var data = payload.data;

                        if (data.ScheduledDays != undefined) {
                            for (var i = 0; i < data.ScheduledDays.length; i++) {
                                var dayOfMonthTypeItem = UtilsService.getItemByVal($scope.scopeModel.dayOfMonthTypeItems, data.ScheduledDays[i].DayOfMonthType, 'value');
                                var dayItem = {
                                    DayOfMonthType: dayOfMonthTypeItem.value,
                                    DayOfMonthTypeDescription: dayOfMonthTypeItem.description,
                                    SpecificDay: data.ScheduledDays[i].SpecificDay
                                };

                                $scope.scopeModel.selectedDays.push(dayItem);
                            }
                        }

                        if (data.ScheduledTimesToRun != undefined) {

                            console.log(data.ScheduledTimesToRun);

                            for (var i = 0; i < data.ScheduledTimesToRun.length; i++) {
                                $scope.scopeModel.selectedTimes.push(data.ScheduledTimesToRun[i]);
                            }
                        }
                    }
                };

                api.getData = function () {
                    var scheduledDays = [];
                    for (var i = 0; i < $scope.scopeModel.selectedDays.length; i++) {
                        scheduledDays.push({
                            DayOfMonthType: $scope.scopeModel.selectedDays[i].DayOfMonthType,
                            SpecificDay: $scope.scopeModel.selectedDays[i].SpecificDay
                        });
                    }

                    return {
                        $type: "Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments.MonthlyTimeTaskTriggerArgument, Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments",
                        TimerTriggerTypeFQTN: TimeSchedulerTypeEnum.Monthly.FQTN,
                        ScheduledDays: scheduledDays,
                        ScheduledTimesToRun: $scope.scopeModel.selectedTimes
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }

        return directiveDefinitionObject;
    }]);