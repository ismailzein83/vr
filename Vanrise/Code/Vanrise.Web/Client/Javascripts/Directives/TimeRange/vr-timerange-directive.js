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
            period:'='
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
            onblurchanged = ' onblurdatetime="onBlurChanged" '
        }

        return   '<vr-row removeline>'
               +  periodSection
               + '<vr-columns width="' + attrs.width + '">'
               + '    <vr-datetimepicker type="' + attrs.type + '" value="ctrltimerange.from" label="From" customvalidate="validateDateTime()" isrequired ' + onblurchanged + '></vr-datetimepicker>'
               + '</vr-columns>'
               + '<vr-columns width="' + attrs.width + '">'
               + '    <vr-datetimepicker type="' + attrs.type + '" value="ctrltimerange.to" label="To" customvalidate="validateDateTime()" isrequired ' + onblurchanged + '></vr-datetimepicker>'
               + '</vr-columns>'
               +'</vr-row>'
    }

    function timeRangeCtor(ctrl, $scope, $attrs) {
        var periodDirectiveAPI;
        var periodReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var date;

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
                if (ctrl.period != undefined && ctrl.period.value != -1) {
                    date = ctrl.period.getInterval();
                    ctrl.from = date.from;
                    ctrl.to = date.to;
                }

            }

            var customize = {
                value: -1,
                description: "Customize"
            }
            $scope.onBlurChanged = function () {
                var from = UtilsService.getShortDate(ctrl.from);
                var oldFrom = UtilsService.getShortDate(date.from);
                var to = UtilsService.getShortDate(ctrl.to);
                var oldTo = UtilsService.getShortDate(date.to);
                if (from != oldFrom || to != oldTo)
                    ctrl.period = customize;

            }
       
        }

        function defineAPI() {
            var api = {};
           
            api.load = function (payload) {
                if (payload != undefined) {

                    ctrl.from = payload.fromData;
                    ctrl.to = payload.toDate;
                }

                var loadPeriodPromiseDeferred = UtilsService.createPromiseDeferred();


                periodReadyPromiseDeferred.promise.then(function () {
                    var payloadPeriod;
                    if (payload && payload.period != undefined) {
                        payloadPeriod = {
                            selectedIds: payload.period
                        };

                    }

                    VRUIUtilsService.callDirectiveLoad(periodDirectiveAPI, payloadPeriod, loadPeriodPromiseDeferred);

                });

                return loadPeriodPromiseDeferred.promise.then(function()
                {
                    if (payload && payload.period != undefined)
                    {
                        date = periodDirectiveAPI.getData().getInterval();
                        ctrl.from = date.from;
                        ctrl.to = date.to;
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