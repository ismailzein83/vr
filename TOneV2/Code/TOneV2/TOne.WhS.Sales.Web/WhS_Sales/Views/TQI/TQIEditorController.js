(function (appControllers) {

    'use strict';

    TQIEditor.$inject = ['$scope', 'WhS_Sales_MarginTypesEnum', 'WhS_Sales_RatePlanAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService'];

    function TQIEditor($scope, WhS_Sales_MarginTypesEnum, WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService) {
        var tqiSelectiveDirectiveAPI;
        var tqiSelectiveDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var servicesDirectiveAPI;
        var servicesDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var rpRouteDetail;
        var ownerName;
        var zoneItem;

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
            }
        }

        function defineScope() {
            $scope.marginTypes = [];

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

            $scope.showMarginPercentage = function () {
                if ($scope.marginTypesSelectedValue != undefined && $scope.margin != undefined && $scope.evaluatedRate != undefined && $scope.marginTypesSelectedValue.value == WhS_Sales_MarginTypesEnum.Fixed.value) {
                    $scope.marginPercentage = (parseFloat($scope.margin) * 100 / parseFloat($scope.evaluatedRate)).toFixed(2) + "%";
                    return true;
                }
                return false;
            };

            $scope.evaluate = function () {
                return WhS_Sales_RatePlanAPIService.GetTQIEvaluatedRate(buildTQIEvaluatedRateObjFromScope()).then(function (response) {
                    if (response != undefined) {
                        $scope.evaluatedRate = response.EvaluatedRate;
                    }
                });
            };

            $scope.calculateRate = function () {
                if ($scope.marginTypesSelectedValue && $scope.evaluatedRate != undefined && $scope.margin != undefined) {
                    if ($scope.marginTypesSelectedValue.value == WhS_Sales_MarginTypesEnum.Fixed.value)
                        $scope.calculatedRate = UtilsService.addFloats($scope.evaluatedRate, $scope.margin);
                    else if ($scope.marginTypesSelectedValue.value == WhS_Sales_MarginTypesEnum.Percentage.value)
                        $scope.calculatedRate = UtilsService.addFloats($scope.evaluatedRate, (Number($scope.margin) * $scope.evaluatedRate) / 100);
                }
            };

            $scope.onServiceReady = function (api) {
                servicesDirectiveAPI = api;
                servicesDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.save = function () {
                $scope.onTQIEvaluated($scope.calculatedRate);
                $scope.modalContext.closeModal();
            };
        }


        function load() {
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadTQISelectiveDirective, loadServicesDirective, loadTQIGrid]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {

            });
        }

        function setTitle() {
            $scope.title = 'TQI Methods';
        }

        function loadStaticData() {
            $scope.saleEntityName = ownerName;
            $scope.zoneName = zoneItem.ZoneName;
            $scope.rate = zoneItem.CurrentRate;
            $scope.rateBED = zoneItem.CurrentRateBED;
            
            if (zoneItem.NewRates != undefined) {
                for (var i = 0; i < zoneItem.NewRates.length ; i++)
                    if (zoneItem.NewRates[i].RateTypeId == null)
                        $scope.newRate = zoneItem.NewRates[i].Rate
            }
        }

        function loadTQISelectiveDirective() {
            var loadTQISelectiveDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            tqiSelectiveDirectiveReadyPromiseDeferred.promise.then(function () {

                var payload = {
                    rpRouteDetail: rpRouteDetail
                };

                VRUIUtilsService.callDirectiveLoad(tqiSelectiveDirectiveAPI, payload, loadTQISelectiveDirectivePromiseDeferred);
            });

            return loadTQISelectiveDirectivePromiseDeferred.promise;
        }

        function loadServicesDirective() {
            var loadServicesDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            servicesDirectiveReadyPromiseDeferred.promise.then(function () {

                var payload = {
                    selectedIds: zoneItem.CurrentServiceIds
                };

                VRUIUtilsService.callDirectiveLoad(servicesDirectiveAPI, payload, loadServicesDirectivePromiseDeferred);
            });

            return loadServicesDirectivePromiseDeferred.promise;
        }

        function loadTQIGrid() {
            var loadTQIGridPromiseDeferred = UtilsService.createPromiseDeferred();

            tqiGridAPIReadyPromiseDeferred.promise.then(function () {

                var payload = {
                    rpRouteDetail: rpRouteDetail
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

    }

    appControllers.controller('WhS_Sales_TQIEditor', TQIEditor);

})(appControllers);