'use strict';

app.directive('vrWhsSalesBulkactionZonefilterSelective', ['WhS_Sales_RatePlanAPIService', 'UtilsService', 'VRUIUtilsService', function (WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService) {
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

        var dropdownSectionAPI;
        var dropdownSectionReadyDeferred = UtilsService.createPromiseDeferred();

        var selectorAPI;
        var selectorReadyDeferred = UtilsService.createPromiseDeferred();

        var directiveAPI;
        var directiveReadyDeferred;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.extensionConfigs = [];
            $scope.scopeModel.selectedExtensionConfig;

            $scope.scopeModel.onDropdownSelectorReady = function (api) {
                dropdownSectionAPI = api;
                dropdownSectionReadyDeferred.resolve();
            };

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                selectorReadyDeferred.resolve();
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

            $scope.scopeModel.getDirectiveSummary = function () {
                var bulkActionZoneFilterTitle = ($scope.scopeModel.selectedExtensionConfig != undefined) ? $scope.scopeModel.selectedExtensionConfig.Title : 'None';
                var summary = 'Filter: ' + bulkActionZoneFilterTitle;

                if (directiveAPI != undefined && directiveAPI.getSummary != undefined) {
                    var directiveSummary = directiveAPI.getSummary();
                    if (directiveSummary != undefined)
                        summary += ' | ' + directiveSummary;
                }

                return summary;
            };

            UtilsService.waitMultiplePromises([dropdownSectionReadyDeferred.promise, selectorReadyDeferred.promise]).then(function () {
                defineAPI();
            });
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

                var loadDirectivePromise = loadDirective();
                promises.push(loadDirectivePromise);

                var loadBulkActionZoneFilterTypeExtensionCofigsPromise = loadBulkActionZoneFilterTypeExtensionCofigs();
                promises.push(loadBulkActionZoneFilterTypeExtensionCofigsPromise);

                function loadBulkActionZoneFilterTypeExtensionCofigs() {
                    return WhS_Sales_RatePlanAPIService.GetBulkActionZoneFilterTypeExtensionConfigs().then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModel.extensionConfigs.push(response[i]);
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

            api.collapseSection = function () {
                if (dropdownSectionAPI != undefined)
                    dropdownSectionAPI.collapse();
            };

            if (selectiveCtrl.onReady != null) {
                selectiveCtrl.onReady(api);
            }
        }
    }
}]);