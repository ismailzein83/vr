'use strict';

app.directive('retailBeRecurringperiodWeekly', ['VRNotificationService', 'UtilsService', 'VRUIUtilsService',
    function (vrNotificationService, utilsService, vruiUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var recurringPeriod = new RecurringPeriodWeekly($scope, ctrl, $attrs);
                recurringPeriod.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/RecurringPeriod/Templates/WeeklyPeriodTemplate .html'
        };

        function RecurringPeriodWeekly($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var daysOfWeekApi;
            var daysOfWeekSelectorReadyDeferred = utilsService.createPromiseDeferred();
            var dayOfWeekEntity;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onDaysOfWeekSelectorReady = function (api) {
                    daysOfWeekApi = api;
                    daysOfWeekSelectorReadyDeferred.resolve();
                }
            }
            function defineApi() {

                var api = {};
                api.load = function (payload) {
                    dayOfWeekEntity = 0;
                    if (payload != undefined) {
                        dayOfWeekEntity = payload.DayOfWeek;
                    }
                };
                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.RecurringPeriods.WeeklyRecurringPeriodSettings, Retail.BusinessEntity.MainExtensions",
                        DayOfWeek: daysOfWeekApi.getSelectedIds()
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
                function loadDayOfWeekSelector() {
                    var dayOfWeekSelectorLoadDeferred = utilsService.createPromiseDeferred();
                    daysOfWeekSelectorReadyDeferred.promise.then(function () {
                        var dayOfWeekSelectorPayload = null;
                        if (dayOfWeekEntity != undefined) {
                            dayOfWeekSelectorPayload = {
                                selectedIds: dayOfWeekEntity
                            };
                        }
                        vruiUtilsService.callDirectiveLoad(daysOfWeekApi, dayOfWeekSelectorPayload, dayOfWeekSelectorLoadDeferred);
                    });
                    return dayOfWeekSelectorLoadDeferred.promise;
                }
                loadDayOfWeekSelector();
            }
            defineApi();
        }
    }]);
