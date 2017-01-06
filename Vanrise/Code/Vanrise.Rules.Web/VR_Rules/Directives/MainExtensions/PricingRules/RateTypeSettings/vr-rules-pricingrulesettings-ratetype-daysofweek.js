'use strict';
app.directive('vrRulesPricingrulesettingsRatetypeDaysofweek', ['$compile', 'DaysOfWeekEnum', 'UtilsService', 'VRValidationService',
function ($compile, DaysOfWeekEnum, UtilsService, VRValidationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.selectedDaysOfWeek = [];
            var ctor = new daysOfWeekRateTypeCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_Rules/Directives/MainExtensions/PricingRules/RateTypeSettings/Templates/PricingRuleDaysOfWeekRateTypeTemplate.html"

    };


    function daysOfWeekRateTypeCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            defineDaysOfWeek();
            ctrl.times = [];
            ctrl.addFilter = function () {
                var filter = {
                    id: ctrl.times.length + 1,
                    FromTime: ctrl.fromTime,
                    ToTime: ctrl.toTime
                };
                if (!isFilterExisting()) {
                    ctrl.times.push(filter);
                    ctrl.fromTime = undefined;
                    ctrl.toTime = undefined;
                }
            };

            ctrl.disableAddButton = function () {
                if (ctrl.fromTime != undefined && ctrl.toTime != undefined && ctrl.validateTime() == undefined)
                    return false;
                return true;
            };

            ctrl.validateTime = function () {
                return VRValidationService.validateTimeRange(ctrl.fromTime, ctrl.toTime);
            };

            ctrl.validateAddedTime = function () {
                if (ctrl.selectedDaysOfWeek != undefined && ctrl.selectedDaysOfWeek.length > 0 && (ctrl.times == undefined || ctrl.times.length == 0)) {
                    return 'At least one time interval should be added.'
                }
                return null;
            };
            
            function isFilterExisting() {
                if (ctrl.times.length == 0)
                    return false;

                for (var x = 0; x < ctrl.times.length; x++) {
                    var currentTime = ctrl.times[x];
                    if (currentTime.FromTime == ctrl.fromTime && currentTime.ToTime == ctrl.toTime)
                        return true;
                }

                return false;
            }

            defineAPI();
        }



        function defineDaysOfWeek() {
            ctrl.daysOfWeek = [];
            for (var p in DaysOfWeekEnum)
                ctrl.daysOfWeek.push(DaysOfWeekEnum[p]);

        }
        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "Vanrise.Rules.Pricing.MainExtensions.RateType.DaysOfWeekRateTypeSettings, Vanrise.Rules.Pricing.MainExtensions",
                    Days: UtilsService.getPropValuesFromArray(ctrl.selectedDaysOfWeek, "value"),
                    TimeRanges: getTimeRanges()
                };
                return obj;
            };
            function getTimeRanges() {
                var obj = [];
                for (var i = 0; i < ctrl.times.length; i++) {
                    obj.push({
                        FromTime: ctrl.times[i].FromTime,
                        ToTime: ctrl.times[i].ToTime
                    });

                }
                return obj;
            }
            api.load = function (payload) {
                if (payload != undefined) {
                    for (var i = 0; i < payload.Days.length; i++) {
                        ctrl.selectedDaysOfWeek.push(UtilsService.getItemByVal(ctrl.daysOfWeek, payload.Days[i], "value"));
                    }
                    if (payload.TimeRanges != null) {
                        for (var i = 0; i < payload.TimeRanges.length; i++) {
                            ctrl.times.push({
                                id: ctrl.times.length + 1,
                                FromTime: payload.TimeRanges[i].FromTime,
                                ToTime: payload.TimeRanges[i].ToTime,
                            });

                        }
                    }

                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);