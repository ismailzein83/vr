﻿'use strict';
app.directive('vrTimerange', ['UtilsService', 'VRUIUtilsService', 'PeriodEnum','VRValidationService',
function (UtilsService, VRUIUtilsService, PeriodEnum, VRValidationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            hideperiodsection: "@"
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
        templateUrl: "/Client/Javascripts/Directives/TimeRange/Templates/TimeRangeTemplate.html"


    };


    function timeRangeCtor(ctrl, $scope, $attrs) {
        var periodDirectiveAPI;
        var periodReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var date;
        var selectedPeriod;
        function initializeController() {


            var date;
            $scope.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            }

            $scope.onPeriodDirectiveReady = function (api)
            {
                periodDirectiveAPI = api;
                periodReadyPromiseDeferred.resolve();
                defineAPI();
            }
            $scope.periodSelectionChanged = function () {
                if ($scope.selectedPeriod != undefined && $scope.selectedPeriod.value != -1) {
                    selectedPeriod = $scope.selectedPeriod;
                    date = $scope.selectedPeriod.getInterval();
                    $scope.fromDate = date.from;
                    $scope.toDate = date.to;
                }

            }

            //var customize = {
            //    value: -1,
            //    description: "Customize"
            //}
            //$scope.onBlurChanged = function () {
            //    var from = UtilsService.getShortDate($scope.fromDate);
            //    var oldFrom = UtilsService.getShortDate(date.from);
            //    var to = UtilsService.getShortDate($scope.toDate);
            //    var oldTo = UtilsService.getShortDate(date.to);
            //    if (from != oldFrom || to != oldTo)
            //        $scope.selectedPeriod = customize;

            //}
       
        }

        function defineAPI() {
            var api = {};
            api.getData = function () {
                var obj = {
                    fromDate: $scope.fromDate,
                    toDate: $scope.toDate,
                    period: selectedPeriod

                }
                return obj;
            }
            api.load = function (payload) {
               
                if (payload != undefined) {

                    $scope.fromDate = payload.fromData;
                    $scope.toDate = payload.toDate;
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
                    date = periodDirectiveAPI.getData().getInterval();
                    selectedPeriod = periodDirectiveAPI.getData();// $scope.selectedPeriod.getInterval();
                    $scope.fromDate = date.from;
                    $scope.toDate = date.to;
                })
               

            }
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);