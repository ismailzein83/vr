(function (appControllers) {

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

        var tqiGridAPI;
        var tqiGridAPIReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
            }
        }

        function defineScope() {
            $scope.marginTypes = [];

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

            $scope.onMarginTypesSelectorReady = function (api) {
                var marginTypes = UtilsService.getArrayEnum(WhS_Sales_MarginTypesEnum);

                for (var i = 0; i < marginTypes.length; i++)
                    $scope.marginTypes.push(marginTypes[i]);
            };


            $scope.evaluate = function () {
                return WhS_Sales_RatePlanAPIService.GetTQIEvaluatedRate(buildTQIEvaluatedRateObjFromScope()).then(function (response) {
                    if (response != undefined) {
                        $scope.evaluatedRate = response.EvaluatedRate;
                    }
                });
            };


            $scope.calculateRate = function () {
                calculateRate();
            };

            $scope.onPeriodTypeSelectorReady = function (api) {
                $scope.periodTypes = UtilsService.getArrayEnum(WhS_Sales_PeriodTypesEnum)
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
        }


        function load() {
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadTQISelectiveDirective, loadServicesDirective]).catch(function (error) {
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

                if (zoneItem.CurrentRate != undefined) {
                    var currentRateAsNumber = Number(zoneItem.CurrentRate);
                    if (!isNaN(currentRateAsNumber))
                        $scope.rate = UtilsService.round(currentRateAsNumber, 4);
                }

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

                var payload = {
                    rpRouteDetail: rpRouteDetail,
                    periodType: $scope.periodTypeSelectedValue != undefined ? $scope.periodTypeSelectedValue.value : undefined,
                    periodValue: $scope.periodValue,
                    currencyId: currencyId,
                    routingDatabaseId: routingDatabaseId,
                    routingProductId: zoneItem.EffectiveRoutingProductId,
                    saleZoneId: zoneItem.ZoneId
                };

                VRUIUtilsService.callDirectiveLoad(tqiGridAPI, payload, loadTQIGridPromiseDeferred);
            });

            return loadTQIGridPromiseDeferred.promise;
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
                    return {
                        periodValue: $scope.periodValue,
                        periodType: $scope.periodTypeSelectedValue.value
                    };
                }
            };

            return context;
        }

        function isEmpty(value) {
            return (value == undefined || value == null || value == '');
        }
    }

    appControllers.controller('WhS_Sales_TQIEditor', TQIEditor);

})(appControllers);