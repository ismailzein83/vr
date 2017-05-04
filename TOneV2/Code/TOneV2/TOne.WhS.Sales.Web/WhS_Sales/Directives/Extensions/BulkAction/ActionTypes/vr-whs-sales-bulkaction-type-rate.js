﻿'use strict';

app.directive('vrWhsSalesBulkactionTypeRate', ['WhS_Sales_RatePlanAPIService', 'WhS_Sales_BulkActionUtilsService', 'UtilsService', 'VRUIUtilsService', function (WhS_Sales_RatePlanAPIService, WhS_Sales_BulkActionUtilsService, UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var rateBulkActionTypeCtrl = this;
            var rateBulkActionType = new RateBulkActionType($scope, rateBulkActionTypeCtrl, $attrs);
            rateBulkActionType.initializeController();
        },
        controllerAs: "rateBulkActionTypeCtrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Extensions/BulkAction/ActionTypes/Templates/RateBulkActionTypeTemplate.html'
    };

    function RateBulkActionType($scope, rateBulkActionTypeCtrl, $attrs) {

        this.initializeController = initializeController;

        var bulkActionContext;

        var rateCalculationMethodSelectorAPI;
        var rateCalculationMethodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var directiveAPI;
        var directiveReadyDeferred;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.rateCalculationMethods = [];

            $scope.scopeModel.onRateCalculationMethodSelectorReady = function (api) {
                rateCalculationMethodSelectorAPI = api;
                rateCalculationMethodSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var directivePayload = {
                    bulkActionContext: bulkActionContext
                };
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
            };

            $scope.scopeModel.onBulkActionChanged = function () {
                WhS_Sales_BulkActionUtilsService.onBulkActionChanged(bulkActionContext);
            };

            UtilsService.waitMultiplePromises([rateCalculationMethodSelectorReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {

                $scope.scopeModel.isLoading = true;

                rateCalculationMethodSelectorAPI.clearDataSource();

                var rateCalculationMethod;

                if (payload != undefined) {

                    bulkActionContext = payload.bulkActionContext;

                    if (payload.bulkAction != undefined) {
                        rateCalculationMethod = payload.bulkAction.RateCalculationMethod;
                        $scope.scopeModel.beginEffectiveDate = payload.bulkAction.BED;
                    }
                }

                var promises = [];

                var loadRateCalculationMethodExtensionConfigsPromise = loadRateCalculationMethodExtensionConfigs();
                promises.push(loadRateCalculationMethodExtensionConfigsPromise);

                if (rateCalculationMethod != undefined) {
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();
                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);
                }

                function loadRateCalculationMethodExtensionConfigs() {
                    return WhS_Sales_RatePlanAPIService.GetRateCalculationMethodTemplates().then(function (response) {
                        if (response != undefined) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModel.rateCalculationMethods.push(response[i]);
                            }
                            if (rateCalculationMethod != undefined)
                                $scope.scopeModel.selectedRateCalculationMethod = UtilsService.getItemByVal($scope.scopeModel.rateCalculationMethods, rateCalculationMethod.ConfigId, 'ExtensionConfigurationId');
                        }
                    });
                }
                function loadDirective() {
                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                    directiveReadyDeferred.promise.then(function () {
                        directiveReadyDeferred = undefined;
                        var directivePayload = {
                            rateCalculationMethod: rateCalculationMethod,
                            bulkActionContext: bulkActionContext
                        };
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                    });
                    return directiveLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.Sales.MainExtensions.RateBulkAction, TOne.WhS.Sales.MainExtensions',
                    RateCalculationMethod: (directiveAPI != undefined) ? directiveAPI.getData() : null,
                    BED: ($scope.scopeModel.beginEffectiveDate != undefined) ? UtilsService.getDateFromDateTime($scope.scopeModel.beginEffectiveDate) : null
                };
            };

            api.getSummary = function () {
                var rateCalculationMethodTitle = ($scope.scopeModel.selectedRateCalculationMethod != undefined) ? $scope.scopeModel.selectedRateCalculationMethod.Title : 'None';

                var rateCalculationMethodDescription = 'None';
                if (directiveAPI != undefined) {
                    var directiveDescription = directiveAPI.getDescription();
                    if (directiveDescription != undefined)
                        rateCalculationMethodDescription = directiveDescription;
                }

                var bedAsString = ($scope.scopeModel.beginEffectiveDate != undefined) ? UtilsService.getShortDate($scope.scopeModel.beginEffectiveDate) : 'None';
                return 'Rate Calculation Method: ' + rateCalculationMethodTitle + ' | Value: ' + rateCalculationMethodDescription + ' | BED: ' + bedAsString;
            };

            if (rateBulkActionTypeCtrl.onReady != null) {
                rateBulkActionTypeCtrl.onReady(api);
            }
        }
    }
}]);