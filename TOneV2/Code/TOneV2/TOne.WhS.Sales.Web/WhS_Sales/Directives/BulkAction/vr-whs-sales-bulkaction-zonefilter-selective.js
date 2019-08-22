﻿'use strict';

app.directive('vrWhsSalesBulkactionZonefilterSelective', ['WhS_Sales_RatePlanAPIService', 'UtilsService', 'VRUIUtilsService', 'WhS_Sales_SalePriceListOwnerTypeEnum', function (WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService, WhS_Sales_SalePriceListOwnerTypeEnum) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var selectiveCtrl = this;
            var bulkActionZoneFilterSelective = new BulkActionZoneFilterSelective($scope, selectiveCtrl, $attrs);
            bulkActionZoneFilterSelective.initializeController();
        },
        controllerAs: "selectiveCtrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/BulkAction/Templates/BulkActionZoneFilterSelectiveTemplate.html'
    };

    function BulkActionZoneFilterSelective($scope, selectiveCtrl, $attrs) {

        this.initializeController = initializeController;

        var bulkActionContext;

        var selectorAPI;

        var directiveAPI;
        var directiveReadyDeferred;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.extensionConfigs = [];
            $scope.scopeModel.selectedExtensionConfig;

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };

            $scope.scopeModel.onZoneFilterSelected = function (selectedZoneFilter) {
                if (bulkActionContext != undefined && bulkActionContext.requireEvaluation != undefined)
                    bulkActionContext.requireEvaluation();
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
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                selectorAPI.clearDataSource();

                var promises = [];
                var zoneFilterType;

                if (payload != undefined) {
                    zoneFilterType = payload.zoneFilterType;
                    bulkActionContext = payload.bulkActionContext;
                }

                extendBulkActionContext();

                var loadDirectivePromise = loadDirective();
                promises.push(loadDirectivePromise);

                var loadBulkActionZoneFilterTypeExtensionCofigsPromise = loadBulkActionZoneFilterTypeExtensionCofigs();
                promises.push(loadBulkActionZoneFilterTypeExtensionCofigsPromise);

                function loadBulkActionZoneFilterTypeExtensionCofigs() {
                    return WhS_Sales_RatePlanAPIService.GetBulkActionZoneFilterTypeExtensionConfigs().then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                var zoneFilterObject = response[i];
                                if ((bulkActionContext.ownerType != WhS_Sales_SalePriceListOwnerTypeEnum.SellingProduct.value) || zoneFilterObject.ShowForSellingProduct)
                                    $scope.scopeModel.extensionConfigs.push(zoneFilterObject);
                            }
                            if (zoneFilterType != undefined) {
                                $scope.scopeModel.selectedExtensionConfig = UtilsService.getItemByVal($scope.scopeModel.extensionConfigs, zoneFilterType.ConfigId, 'ExtensionConfigurationId');
                            }
                            else if ($scope.scopeModel.extensionConfigs.length > 0) {
                                $scope.scopeModel.selectedExtensionConfig = $scope.scopeModel.extensionConfigs[0];
                            }
                        }
                    });
                }
                function loadDirective() {
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();

                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    directiveReadyDeferred.promise.then(function () {
                        directiveReadyDeferred = undefined;
                        var directivePayload = {
                            zoneFilter: zoneFilterType,
                            bulkActionContext: bulkActionContext
                        };
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                    });

                    return directiveLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var data;
                if ($scope.scopeModel.selectedExtensionConfig != undefined && directiveAPI != undefined) {
                    data = directiveAPI.getData();
                }
                return data;
            };

            api.getSummary = function () {
                var summary = {};

                var filterTitle = ($scope.scopeModel.selectedExtensionConfig != undefined) ? $scope.scopeModel.selectedExtensionConfig.Title : 'None';
                summary.title = 'Filter: ' + filterTitle;

                if (directiveAPI != undefined && directiveAPI.getSummary != undefined)
                    summary.body = directiveAPI.getSummary();

                return summary;
            };

            if (selectiveCtrl.onReady != null) {
                selectiveCtrl.onReady(api);
            }
        }
        function extendBulkActionContext() {
            if (bulkActionContext == undefined)
                return;
            bulkActionContext.selectDefaultBulkActionZoneFilter = function (defaultBulkActionZoneFilterConfigId, canApplyZoneFilter) {
                $scope.scopeModel.isDisabled = (canApplyZoneFilter === false);

                if (defaultBulkActionZoneFilterConfigId == undefined)
                    return;

                // If the default zone filter is different than the selected one, delete the onBulkActionChanged of the selected zone filter
                if ($scope.scopeModel.selectedExtensionConfig != undefined && $scope.scopeModel.selectedExtensionConfig.ExtensionConfigurationId != defaultBulkActionZoneFilterConfigId)
                    delete bulkActionContext.onBulkActionChanged;

                $scope.scopeModel.selectedExtensionConfig = UtilsService.getItemByVal($scope.scopeModel.extensionConfigs, defaultBulkActionZoneFilterConfigId, 'ExtensionConfigurationId');
            };
        }
    }
}]);