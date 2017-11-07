'use strict';

app.directive('vrWhsSalesBulkactionTypeRate', ['WhS_Sales_RatePlanAPIService', 'WhS_Sales_BulkActionUtilsService', 'UtilsService', 'VRUIUtilsService', 'WhS_BE_SalePriceListOwnerTypeEnum', function (WhS_Sales_RatePlanAPIService, WhS_Sales_BulkActionUtilsService, UtilsService, VRUIUtilsService, WhS_BE_SalePriceListOwnerTypeEnum) {
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


        var rateSourceSelectorAPI;
        var rateSourceSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var directiveAPI;
        var directiveReadyDeferred;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.rateCalculationMethods = [];

            $scope.scopeModel.showRateSource = false;

            $scope.scopeModel.onRateCalculationMethodSelectorReady = function (api) {
                rateCalculationMethodSelectorAPI = api;
                rateCalculationMethodSelectorReadyDeferred.resolve();
            };
                
            $scope.scopeModel.onRateSourceSelectorReady = function (api) {
                rateSourceSelectorAPI = api;
                rateSourceSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onRateSourceChanged = function ()
            {
                WhS_Sales_BulkActionUtilsService.onBulkActionChanged(bulkActionContext);
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

            UtilsService.waitMultiplePromises([rateCalculationMethodSelectorReadyDeferred.promise, rateSourceSelectorReadyDeferred.promise]).then(function () {
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

                    if (bulkActionContext.ownerType != undefined && bulkActionContext.ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value)
                        $scope.scopeModel.showRateSource = true;
                }

                var promises = [];

                var loadRateCalculationMethodExtensionConfigsPromise = loadRateCalculationMethodExtensionConfigs();
                promises.push(loadRateCalculationMethodExtensionConfigsPromise);

                var loadRateSourcePromise = loadRateSourceSelector();
                promises.push(loadRateSourcePromise);

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

                function loadRateSourceSelector() {
                    var rateSourceLoadDeferred = UtilsService.createPromiseDeferred();
                    rateSourceSelectorReadyDeferred.promise.then(function () {                       
                        VRUIUtilsService.callDirectiveLoad(rateSourceSelectorAPI, undefined, rateSourceLoadDeferred);
                    });
                    return rateSourceLoadDeferred.promise;
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
                    BED: ($scope.scopeModel.beginEffectiveDate != undefined) ? UtilsService.getDateFromDateTime($scope.scopeModel.beginEffectiveDate) : null,
                    RateSources: rateSourceSelectorAPI.getSelectedIds()
                };
            };

            api.getSummary = function () {
                var rateCalculationMethodTitle = ($scope.scopeModel.selectedRateCalculationMethod != undefined) ? $scope.scopeModel.selectedRateCalculationMethod.Title : 'None';

                var rateCalculationMethodDescription = 'None';
                var rateSourcesText = 'None';
                if (directiveAPI != undefined) {
                    var directiveDescription = directiveAPI.getDescription();
                    if (directiveDescription != undefined)
                        rateCalculationMethodDescription = directiveDescription;
                }
                if (rateSourceSelectorAPI.getSelectedIds() != undefined)
                    rateSourcesText = rateSourceSelectorAPI.getSelectedText().join(',');
                var bedAsString = ($scope.scopeModel.beginEffectiveDate != undefined) ? UtilsService.getShortDate($scope.scopeModel.beginEffectiveDate) : 'None';

                var summary = 'Rate Calculation Method: ' + rateCalculationMethodTitle + ' | ' + rateCalculationMethodDescription;
                if ($scope.scopeModel.showRateSource == true)
                    summary += ' |  Apply on Rates: ' + rateSourcesText;
                return summary;
            };

            if (rateBulkActionTypeCtrl.onReady != null) {
                rateBulkActionTypeCtrl.onReady(api);
            }
        }
    }
}]);