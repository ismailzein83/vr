﻿(function (appControllers) {

    'use strict';

    TQIEditor.$inject = ['$scope', 'WhS_Sales_MarginTypesEnum', 'WhS_Sales_RatePlanAPIService', 'WhS_Sales_PeriodTypesEnum', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService'];

    function TQIEditor($scope, WhS_Sales_MarginTypesEnum, WhS_Sales_RatePlanAPIService, WhS_Sales_PeriodTypesEnum, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService) {
        var tqiSelectiveDirectiveAPI;
        var tqiSelectiveDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var servicesDirectiveAPI;
        var servicesDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var rpRouteDetail;
        var ownerName;
        var zoneItem;
        var routingDatabaseId;
        var currencyId;
        var ratePlanSettings;

        var tqiGridAPI;
        var tqiGridAPIReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var periodSelectorAPI;
        var periodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                zoneItem = parameters.context.zoneItem;
                rpRouteDetail = parameters.context.zoneItem.RPRouteDetail;
                ownerName = parameters.context.ownerName;
                routingDatabaseId = parameters.context.routingDatabaseId;
                currencyId = parameters.context.currencyId;
                ratePlanSettings = parameters.context.ratePlanSettings;
            }
        }
        function defineScope() {
            
            $scope.marginTypes = UtilsService.getArrayEnum(WhS_Sales_MarginTypesEnum);
            $scope.marginTypesSelectedValue = UtilsService.getItemByVal($scope.marginTypes, WhS_Sales_MarginTypesEnum.Percentage.value, 'value');

            $scope.showMarginPercentage = false;

            $scope.onTQISelectiveReady = function (api) {
                tqiSelectiveDirectiveAPI = api;
                tqiSelectiveDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.onTQIGridReady = function (api) {
                tqiGridAPI = api;
                tqiGridAPIReadyPromiseDeferred.resolve();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.disableSaveBtn = function () {
                return $scope.calculatedRate == undefined;
            }

            $scope.evaluate = function () {
                return WhS_Sales_RatePlanAPIService.GetTQIEvaluatedRate(buildTQIEvaluatedRateObjFromScope()).then(function (response) {
                    if (response != undefined) {
                        $scope.evaluatedRate = getRoundedNumber(response.EvaluatedRate);
                    }
                });
            };

            $scope.calculateRate = function () {
                calculateRate();
            };

            $scope.onServiceReady = function (api) {
                servicesDirectiveAPI = api;
                servicesDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.searchClicked = function () {
                return loadTQIGrid();
            };

            $scope.save = function () {
                $scope.onTQIEvaluated($scope.calculatedRate);
                $scope.modalContext.closeModal();
            };

            $scope.onTQIMethodSelectionChanged = function () {
                clearData();
            };

            $scope.onPeriodSelectorReady = function (api) {
                periodSelectorAPI = api;
                periodSelectorReadyDeferred.resolve();
            };
        }
        function load() {
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadTQISelectiveDirective, loadServicesDirective, loadPeriodSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {

            });
        }
        function setTitle() {
            $scope.title = 'TQI';
        }
        function loadStaticData() {
            $scope.saleEntityName = ownerName;

            if (zoneItem != undefined) {
                $scope.zoneName = zoneItem.ZoneName;
                $scope.rate = getRoundedNumber(zoneItem.CurrentRate)
                $scope.rateBED = zoneItem.CurrentRateBED;
                $scope.newRate = zoneItem.NewRate;
            }
        }
        function loadTQISelectiveDirective() {
            var loadTQISelectiveDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            tqiSelectiveDirectiveReadyPromiseDeferred.promise.then(function () {

                var payload = {
                    rpRouteDetail: rpRouteDetail,
                    context: getContext()
                };

                VRUIUtilsService.callDirectiveLoad(tqiSelectiveDirectiveAPI, payload, loadTQISelectiveDirectivePromiseDeferred);
            });

            return loadTQISelectiveDirectivePromiseDeferred.promise;
        }
        function loadServicesDirective() {
            var loadServicesDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            servicesDirectiveReadyPromiseDeferred.promise.then(function () {

                var payload = {
                    selectedIds: zoneItem.EffectiveServiceIds
                };

                VRUIUtilsService.callDirectiveLoad(servicesDirectiveAPI, payload, loadServicesDirectivePromiseDeferred);
            });

            return loadServicesDirectivePromiseDeferred.promise;
        }
        function loadTQIGrid() {
            var loadTQIGridPromiseDeferred = UtilsService.createPromiseDeferred();

            tqiGridAPIReadyPromiseDeferred.promise.then(function () {

                var tqiGridPayload = {
                    rpRouteDetail: rpRouteDetail,
                    currencyId: currencyId,
                    routingDatabaseId: routingDatabaseId,
                    routingProductId: zoneItem.EffectiveRoutingProductId,
                    saleZoneId: zoneItem.ZoneId
                };

                var period = periodSelectorAPI.getData();
                if (period != undefined) {
                    tqiGridPayload.periodValue = period.PeriodValue;
                    tqiGridPayload.periodType = period.PeriodType;
                }

                VRUIUtilsService.callDirectiveLoad(tqiGridAPI, tqiGridPayload, loadTQIGridPromiseDeferred);
            });

            return loadTQIGridPromiseDeferred.promise;
        }
        function loadPeriodSelector() {
            var periodSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            periodSelectorReadyDeferred.promise.then(function () {
                var periodSelectorPayload = {
                    period: {}
                };
                if (ratePlanSettings != undefined) {
                    periodSelectorPayload.period.periodValue = ratePlanSettings.TQIPeriodValue;
                    periodSelectorPayload.period.periodType = ratePlanSettings.TQIPeriodType;
                }
                VRUIUtilsService.callDirectiveLoad(periodSelectorAPI, periodSelectorPayload, periodSelectorLoadDeferred);
            });

            return periodSelectorLoadDeferred.promise;
        }

        function buildTQIEvaluatedRateObjFromScope() {
            return {
                TQIMethod: tqiSelectiveDirectiveAPI.getData(),
                RPRouteDetail: rpRouteDetail
            };
        }

        function calculateRate() {
            if ($scope.marginTypesSelectedValue != undefined && $scope.evaluatedRate != undefined && !isEmpty($scope.margin)) {
                if ($scope.marginTypesSelectedValue.value == WhS_Sales_MarginTypesEnum.Fixed.value) {
                    $scope.marginPercentage = (parseFloat($scope.margin) * 100 / parseFloat($scope.evaluatedRate)).toFixed(2) + "%";
                    $scope.showMarginPercentage = true;
                }
                else {
                    clearMarginPercentage();
                }

                if ($scope.marginTypesSelectedValue.value == WhS_Sales_MarginTypesEnum.Fixed.value) {
                    $scope.calculatedRate = UtilsService.addFloats($scope.evaluatedRate, $scope.margin);
                }
                else if ($scope.marginTypesSelectedValue.value == WhS_Sales_MarginTypesEnum.Percentage.value) {
                    $scope.calculatedRate = UtilsService.addFloats($scope.evaluatedRate, (Number($scope.margin) * $scope.evaluatedRate) / 100);
                }
            }
            else {
                clearMarginPercentage();
            }

            function clearMarginPercentage() {
                $scope.marginPercentage = undefined;
                $scope.showMarginPercentage = false;
            }
        }
        function clearData() {
            $scope.evaluatedRate = undefined;
            $scope.margin = undefined;
            $scope.calculatedRate = undefined;
            $scope.marginPercentage = undefined;
            $scope.marginTypesSelectedValue = undefined;
        }
        function getContext() {
            var context = {
                getDuration: function () {
                    var duration = {};

                    var period = periodSelectorAPI.getData();
                    if (period != undefined) {
                        duration.periodValue = period.PeriodValue;
                        duration.periodType = period.PeriodType;
                    }

                    return duration;
                }
            };

            return context;
        }
        function isEmpty(value) {
            return (value == undefined || value == null || value == '');
        }
        function getRoundedNumber(number) {
            if (!isEmpty(number)) {
                var castedNumber = Number(number);
                if (!isNaN(castedNumber))
                    return UtilsService.round(castedNumber, 4);
            }
        }
    }

    appControllers.controller('WhS_Sales_TQIEditor', TQIEditor);

})(appControllers);