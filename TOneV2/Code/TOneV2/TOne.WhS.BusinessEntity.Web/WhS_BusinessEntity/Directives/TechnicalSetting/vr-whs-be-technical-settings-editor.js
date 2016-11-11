'use strict';

app.directive('vrWhsBeTechnicalSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
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

            ctrl.titles = [];

            var offPeakRateTypeSelectorAPI;
            var offPeakRateTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var weekendRateTypeSelectorAPI;
            var weekendRateTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var holidayRateTypeSelectorAPI;
            var holidayRateTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.onOffPeakRateTypeSelectorReady = function (api) {
                offPeakRateTypeSelectorAPI = api;
                offPeakRateTypeSelectorReadyPromiseDeferred.resolve();
            };

            $scope.onWeekendRateTypeSelectorReady = function (api) {
                weekendRateTypeSelectorAPI = api;
                weekendRateTypeSelectorReadyPromiseDeferred.resolve();
            };

            $scope.onHolidayRateTypeSelectorReady = function (api) {
                holidayRateTypeSelectorAPI = api;
                holidayRateTypeSelectorReadyPromiseDeferred.resolve();
            };

            function initializeController() {
                ctrl.disabledAddTitle = true;

                ctrl.addTitle = function () {
                    ctrl.titles.push({ title: ctrl.titlevalue });
                    ctrl.titlevalue = undefined;
                    ctrl.disabledAddTitle = true;
                };

                ctrl.onTitleValueChange = function (value) {
                    ctrl.disabledAddTitle = (value == undefined && ctrl.titlevalue.length - 1 < 1) || UtilsService.getItemIndexByVal(ctrl.titles, value, "title") != -1;
                };

                ctrl.validateAddTitle = function () {
                    if (ctrl.title != undefined && ctrl.title.length == 0)
                        return "Enter at least one keyword.";
                    return null;
                };

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

                        if (payload.data.TaxesDefinition != undefined && payload.data.TaxesDefinition.ItemDefinitions != undefined)
                            angular.forEach(payload.data.TaxesDefinition.ItemDefinitions, function (val) {
                                ctrl.titles.push({ title: val.Title });
                            });

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
                    var itemDefinitions = [];

                    for (var i = 0; i < ctrl.titles.length; i++) {
                        var taxItemDefinition = {};
                        taxItemDefinition.ItemId = UtilsService.guid();
                        taxItemDefinition.Title = ctrl.titles[i].title;
                        itemDefinitions.push(taxItemDefinition);
                    }

                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.BusinessEntityTechnicalSettingsData, TOne.WhS.BusinessEntity.Entities",
                        RateTypeConfiguration: {
                            OffPeakRateTypeId: offPeakRateTypeSelectorAPI.getSelectedIds(),
                            WeekendRateTypeId: weekendRateTypeSelectorAPI.getSelectedIds(),
                            HolidayRateTypeId: holidayRateTypeSelectorAPI.getSelectedIds()
                        },
                        TaxesDefinition: { ItemDefinitions: itemDefinitions }
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);