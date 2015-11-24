'use strict';
app.directive('vrWhsBePricingrulesettingsRatetypeDaysofweek', ['$compile','DaysOfWeekEnum','UtilsService','VRValidationService',
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
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/PricingRules/RateTypeSettings/Templates/PricingRuleDaysOfWeekRateTypeTemplate.html"

    };


    function daysOfWeekRateTypeCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            defineDaysOfWeek();
            ctrl.times = [];
            ctrl.addFilter = function () {
                var filter = {
                    id:ctrl.times.length+1,
                    FromTime: ctrl.fromTime,
                    ToTime: ctrl.toTime
                }
                if (!UtilsService.contains(ctrl.times, filter))
                    ctrl.times.push(filter);
            }
            ctrl.validateTime = function () {
              return  VRValidationService.validateTimeRange(ctrl.fromTime, ctrl.toTime);
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
                    $type: "TOne.WhS.BusinessEntity.MainExtensions.PricingRules.RateTypeSettings.DaysOfWeekRateTypeSettings, TOne.WhS.BusinessEntity.MainExtensions",
                    Days: UtilsService.getPropValuesFromArray(ctrl.selectedDaysOfWeek, "value"),
                    TimeRanges:getTimeRanges()
                }
                return obj;
            }
            function getTimeRanges(){
                var obj=[];
                for(var i=0;i<ctrl.times.length;i++){
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
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);