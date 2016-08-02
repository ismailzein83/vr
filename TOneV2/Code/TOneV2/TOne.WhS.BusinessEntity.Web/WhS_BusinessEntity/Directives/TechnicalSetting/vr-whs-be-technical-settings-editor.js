'use strict';

app.directive('vrWhsBeTechnicalSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/TechnicalSetting/Templates/BETechnicalSettingsTemplate.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {

            var offPeakRateTypeSelectorAPI;
            var offPeakRateTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var weekendRateTypeSelectorAPI;
            var weekendRateTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var holidayRateTypeSelectorAPI;
            var holidayRateTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.onOffPeakRateTypeSelectorReady = function (api) {
                offPeakRateTypeSelectorAPI = api;
                offPeakRateTypeSelectorReadyPromiseDeferred.resolve();
            }

            $scope.onWeekendRateTypeSelectorReady = function (api) {
                weekendRateTypeSelectorAPI = api;
                weekendRateTypeSelectorReadyPromiseDeferred.resolve();
            }

            $scope.onHolidayRateTypeSelectorReady = function (api) {
                holidayRateTypeSelectorAPI = api;
                holidayRateTypeSelectorReadyPromiseDeferred.resolve();
            }

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var offPeakPayload;
                    var weekendPayload;
                    var holidayPayload;

                    if (payload != undefined && payload.data != undefined) {
                        if (payload.data.RateTypeConfiguration != undefined) {
                            offPeakPayload = { selectedIds: payload.data.RateTypeConfiguration.OffPeakRateTypeId };
                            weekendPayload = { selectedIds: payload.data.RateTypeConfiguration.WeekendRateTypeId };
                            holidayPayload = { selectedIds: payload.data.RateTypeConfiguration.HolidayRateTypeId };
                        }
                    }

                    var offPeakRateTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    offPeakRateTypeSelectorReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(offPeakRateTypeSelectorAPI, offPeakPayload, offPeakRateTypeSelectorLoadPromiseDeferred);
                        });

                    promises.push(offPeakRateTypeSelectorLoadPromiseDeferred.promise);

                    var weekendRateTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    weekendRateTypeSelectorReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(weekendRateTypeSelectorAPI, weekendPayload, weekendRateTypeSelectorLoadPromiseDeferred);
                        });

                    promises.push(weekendRateTypeSelectorLoadPromiseDeferred.promise);

                    var holidayRateTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    holidayRateTypeSelectorReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(holidayRateTypeSelectorAPI, holidayPayload, holidayRateTypeSelectorLoadPromiseDeferred);
                        });

                    promises.push(holidayRateTypeSelectorLoadPromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.BusinessEntityTechnicalSettingsData, TOne.WhS.BusinessEntity.Entities",
                        RateTypeConfiguration: {
                            OffPeakRateTypeId: offPeakRateTypeSelectorAPI.getSelectedIds(),
                            WeekendRateTypeId: weekendRateTypeSelectorAPI.getSelectedIds(),
                            HolidayRateTypeId: holidayRateTypeSelectorAPI.getSelectedIds()
                        }
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);