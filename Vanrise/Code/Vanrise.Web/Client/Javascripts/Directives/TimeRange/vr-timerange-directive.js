'use strict';
app.directive('vrTimerange', ['UtilsService', 'VRUIUtilsService', 'PeriodEnum','VRValidationService',
function (UtilsService, VRUIUtilsService, PeriodEnum, VRValidationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            hideperiodsection: "@",
            width: '@',
            type: '@',
            from: '=',
            to: '=',
            period: '=',
            isrequired:'='
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            var ctor = new timeRangeCtor(ctrl, $scope, $attrs);
            ctor.initializeController();

        },
        controllerAs: 'ctrltimerange',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        template: function (element, attrs) {
            return getTemplate(attrs);
        }

    };

    function getTemplate(attrs)
    {
        var periodSection = "";
        var onblurchanged = "";
        if (attrs.hideperiodsection == undefined)
        {
            periodSection = '<vr-columns width="' + attrs.width + '">'
                          + '<vr-period-selector on-ready="onPeriodDirectiveReady" selectedvalues="ctrltimerange.period" onselectionchanged="periodSelectionChanged"></vr-period-selector>'
                          + '</vr-columns>'
            onblurchanged = ' onvaluechanged="onBlurChanged" '
        }

        return   periodSection
               + '<vr-columns width="' + attrs.width + '">'
               + '    <vr-datetimepicker type="' + attrs.type + '" value="ctrltimerange.from" label="From" customvalidate="validateDateTime()" isrequired="true" ' + onblurchanged + '></vr-datetimepicker>'
               + '</vr-columns>'
               + '<vr-columns width="' + attrs.width + '">'
               + '    <vr-datetimepicker type="' + attrs.type + '" value="ctrltimerange.to" label="To" customvalidate="validateDateTime()" isrequired="ctrltimerange.isrequired" ' + onblurchanged + '></vr-datetimepicker>'
               + '</vr-columns>'
    }

    function timeRangeCtor(ctrl, $scope, $attrs) {
        var periodDirectiveAPI;
        var periodReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var date;
        var periodSelectedPromiseDeferred;
        function initializeController() {
            var date;
            $scope.validateDateTime = function () {
                return VRValidationService.validateTimeRange(ctrl.from, ctrl.to);
            }

            $scope.onPeriodDirectiveReady = function (api)
            {
                periodDirectiveAPI = api;
                periodReadyPromiseDeferred.resolve();
                defineAPI();
            }

            $scope.periodSelectionChanged = function () {
             
                if (periodSelectedPromiseDeferred == undefined)
                {
                  
                    if (ctrl.period != undefined && ctrl.period.value != -1) {
                        date = ctrl.period.getInterval();
                        ctrl.from = date.from;
                        ctrl.to = date.to;
                    }
                }
               else
                {
                    periodSelectedPromiseDeferred.resolve();
                }
                
            }

            var customize = {
                value: -1,
                description: "Customize"
            }

            $scope.onBlurChanged = function () {
                if (ctrl.period != undefined && ctrl.period.value != -1)
                {
                    date = ctrl.period.getInterval();
                    var from = UtilsService.getShortDate(ctrl.from);
                    var oldFrom = UtilsService.getShortDate(date.from);
                    var to = UtilsService.getShortDate(ctrl.to);
                    var oldTo = UtilsService.getShortDate(date.to);
                    if (from != oldFrom || to != oldTo)
                        ctrl.period = customize;
                }
               
            }

            if ($attrs.hideperiodsection != undefined) {
                defineAPI();
            }
        }

        function defineAPI() {
            var api = {};
           
            api.load = function (payload) {
                if (payload != undefined) {
                    ctrl.from = payload.fromData;
                    ctrl.to = payload.toDate;
                   
                }

                if ($attrs.hideperiodsection != undefined) {
                    return;
                }

                var loadPeriodPromiseDeferred = UtilsService.createPromiseDeferred();
                periodSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                periodReadyPromiseDeferred.promise.then(function () {
                    var payloadPeriod;
                    if (payload && payload.period != undefined) {
                        payloadPeriod = {
                            selectedIds: payload.period
                        };

                    }

                    VRUIUtilsService.callDirectiveLoad(periodDirectiveAPI, payloadPeriod, loadPeriodPromiseDeferred);
                });

                return UtilsService.waitMultiplePromises([loadPeriodPromiseDeferred.promise,periodSelectedPromiseDeferred.promise]).then(function ()
                {
                    if (payload && payload.period != undefined)
                    {

                        if (payload.fromData == undefined && payload.toDate == undefined)
                        {
                            date = periodDirectiveAPI.getData().getInterval();
                            ctrl.from = date.from;
                            ctrl.to = date.to;
                        }
                        if (payload.fromDate != undefined)
                        {
                            setTimeout(function () { ctrl.from = payload.fromDate; UtilsService.safeApply($scope);});
                        }
                        if (payload.toDate != undefined) {
                            setTimeout(function () { ctrl.to = payload.toDate; UtilsService.safeApply($scope); });

                           
                        }
                        periodSelectedPromiseDeferred = undefined;
                    }
                    
                })
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);