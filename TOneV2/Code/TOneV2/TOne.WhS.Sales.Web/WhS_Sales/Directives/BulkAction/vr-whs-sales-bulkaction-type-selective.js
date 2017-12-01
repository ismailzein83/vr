'use strict';

app.directive('vrWhsSalesBulkactionTypeSelective', ['WhS_Sales_RatePlanAPIService', 'UtilsService', 'VRUIUtilsService', 'WhS_Sales_BulkActionUtilsService', function (WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService, WhS_Sales_BulkActionUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var selectiveCtrl = this;
            var bulkActionTypeSelective = new BulkActionTypeSelective($scope, selectiveCtrl, $attrs);
            bulkActionTypeSelective.initializeController();
        },
        controllerAs: "selectiveCtrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/BulkAction/Templates/BulkActionTypeSelectiveTemplate.html'
    };

    function BulkActionTypeSelective($scope, selectiveCtrl, $attrs) {

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

            $scope.scopeModel.onBulkActionSelected = function (selectedBulkAction) {
                if (bulkActionContext == undefined)
                    return;
                WhS_Sales_BulkActionUtilsService.onBulkActionChanged(bulkActionContext);
                if (bulkActionContext.selectDefaultBulkActionZoneFilter != undefined)
                    bulkActionContext.selectDefaultBulkActionZoneFilter(selectedBulkAction.DefaultBulkActionZoneFilterConfigId, selectedBulkAction.CanApplyZoneFilter);

                if (bulkActionContext.onBulkActionChanged != undefined)
                {
                    bulkActionContext.onBulkActionChanged();
                }
                   
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
                var bulkAction;

                if (payload != undefined) {
                    bulkAction = payload.bulkAction;
                    bulkActionContext = payload.bulkActionContext;
                }

                if (bulkAction != undefined) {
                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);
                }

                var loadBulkActionTypeExtensionCofigsPromise = loadBulkActionTypeExtensionCofigs();
                promises.push(loadBulkActionTypeExtensionCofigsPromise);

                function loadBulkActionTypeExtensionCofigs() {
                    var ownerType = (bulkActionContext != undefined) ? bulkActionContext.ownerType : null;
                    return WhS_Sales_RatePlanAPIService.GetBulkActionTypeExtensionConfigs(ownerType).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModel.extensionConfigs.push(response[i]);
                            }
                            if (bulkAction != undefined) {
                                $scope.scopeModel.selectedExtensionConfig = UtilsService.getItemByVal($scope.scopeModel.extensionConfigs, bulkAction.ConfigId, 'ExtensionConfigurationId');
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
                            bulkAction: bulkAction,
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

            api.getValidationDirectiveName = function () {
                return ($scope.scopeModel.selectedExtensionConfig != null) ? $scope.scopeModel.selectedExtensionConfig.ValidationResultDirective : undefined;
            };

            api.getSummary = function () {
                var summary = {};

                var bulkActionTitle = ($scope.scopeModel.selectedExtensionConfig != undefined) ? $scope.scopeModel.selectedExtensionConfig.Title : 'None';
                summary.title = 'Bulk Action: ' + bulkActionTitle;

                if (directiveAPI != undefined && directiveAPI.getSummary != undefined)
                    summary.body = directiveAPI.getSummary();

                return summary;
            };

            if (selectiveCtrl.onReady != null) {
                selectiveCtrl.onReady(api);
            }
        }
    }
}]);