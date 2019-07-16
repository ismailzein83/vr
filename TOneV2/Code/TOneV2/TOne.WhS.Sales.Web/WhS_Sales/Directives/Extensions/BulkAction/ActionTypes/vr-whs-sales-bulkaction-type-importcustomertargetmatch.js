'use strict';

app.directive('vrWhsSalesBulkactionTypeImportcustomertargetmatch', ['WhS_Sales_RatePlanAPIService', 'WhS_Sales_BulkActionUtilsService', 'UtilsService', 'VRUIUtilsService', 'WhS_BE_SalePriceListOwnerTypeEnum', 'WhS_Sales_CustomerTargetMatchAPIService'
    , function (WhS_Sales_RatePlanAPIService, WhS_Sales_BulkActionUtilsService, UtilsService, VRUIUtilsService, WhS_BE_SalePriceListOwnerTypeEnum, WhS_Sales_CustomerTargetMatchAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var targetMatchBulkActionTypeCtrl = this;
                var targetMatchBulkActionType = new TargetMatchBulkActionType($scope, targetMatchBulkActionTypeCtrl, $attrs);
                targetMatchBulkActionType.initializeController();
            },
            controllerAs: "rateBulkActionTypeCtrl",
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Sales/Directives/Extensions/BulkAction/ActionTypes/Templates/CustomerTargetMatchImportActionTypeTemplate.html'
        };

        function TargetMatchBulkActionType($scope, targetMatchBulkActionTypeCtrl, $attrs) {

            this.initializeController = initializeController;

            var bulkActionContext;
            var cacheObjectName;

            var directiveAPI;
            var directiveReadyDeferred;

            var rateCalculationMethodSelectorAPI;
            var rateCalculationMethodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var rateCalculationMethod;
            var ownerId;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.rateCalculationMethods = [];
                $scope.scopeModel.headerRowExists = true;
                cacheObjectName = UtilsService.guid();

                $scope.scopeModel.onFileChanged = function (obj) {
                    cacheObjectName = UtilsService.guid();
                    if (obj != undefined) {
                        $scope.scopeModel.file = { fileId: obj.fileId, fileName: obj.fileName };
                        WhS_Sales_BulkActionUtilsService.onBulkActionChanged(bulkActionContext);
                    }
                };
                $scope.scopeModel.onRateCalculationMethodSelectorReady = function (api) {
                    rateCalculationMethodSelectorAPI = api;
                    rateCalculationMethodSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.download = function () {
                    WhS_Sales_CustomerTargetMatchAPIService.DownloadCustomerTargetMatchTemplate(ownerId).then(function (response) {
                        UtilsService.downloadFile(response.data, response.headers);
                    });
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

                UtilsService.waitMultiplePromises([]).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.isLoading = true;
                    var promises = [];

                    //rateCalculationMethodSelectorAPI.clearDataSource();
                    if (payload != undefined) {
                        bulkActionContext = payload.bulkActionContext;
                        if (bulkActionContext != undefined) {
                            ownerId = bulkActionContext.ownerId;
                        }
                        if (payload.bulkAction != undefined) {
                            rateCalculationMethod = payload.bulkAction.RateCalculationMethod;
                        }
                    }
                    var loadRateCalculationMethodExtensionConfigsPromise = loadRateCalculationMethodExtensionConfigs();
                    promises.push(loadRateCalculationMethodExtensionConfigsPromise);

                    if (rateCalculationMethod != undefined) {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }
                    return UtilsService.waitMultiplePromises(promises).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                };

                api.getData = function () {
                    var data = {
                        $type: 'TOne.WhS.Sales.MainExtensions.ImportCustomerTargetMatch, TOne.WhS.Sales.MainExtensions',
                        HeaderRowExists: $scope.scopeModel.headerRowExists,
                        CacheObjectName: cacheObjectName,
                        RateCalculationMethod: (directiveAPI != undefined) ? directiveAPI.getData() : null,
                        CostCalculationMethods: bulkActionContext != undefined ? bulkActionContext.costCalculationMethods : null,
                        PolicyConfigId: bulkActionContext.policyConfigId,
                        RoutingDatabaseId: bulkActionContext.routingDatabaseId,
                        NumberOfOptions: bulkActionContext.numberOfOptions,
                        CurrencyId: bulkActionContext.currencyId
                    };
                    if ($scope.scopeModel.file != undefined) {
                        data.FileId = $scope.scopeModel.file.fileId;

                    }
                    if (bulkActionContext != undefined) {
                        data.OwnerId = bulkActionContext.ownerId;
                    }
                    console.log(data);
                    return data;
                };

                api.getSummary = function () {
                    var rateCalculationMethodTitle = ($scope.scopeModel.selectedRateCalculationMethod != undefined) ? $scope.scopeModel.selectedRateCalculationMethod.Title : 'None';
                    var rateCalculationMethodDescription = 'None';
                    if (directiveAPI != undefined) {
                        var directiveDescription = directiveAPI.getDescription();
                        if (directiveDescription != undefined)
                            rateCalculationMethodDescription = directiveDescription;
                    }

                    var uploadedFileName = ($scope.scopeModel.file != undefined) ? $scope.scopeModel.file.fileName : 'None';
                    var headerRowExistsAsString = ($scope.scopeModel.headerRowExists === true) ? 'Header Exists' : 'Header Does Not Exist';
                    return 'Uploaded File: ' + uploadedFileName + ' | ' + headerRowExistsAsString + '|' + 'Rate Calculation Method: ' + rateCalculationMethodTitle + ' | ' + rateCalculationMethodDescription;;
                };

                if (targetMatchBulkActionTypeCtrl.onReady != null) {
                    targetMatchBulkActionTypeCtrl.onReady(api);
                }
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
        }

    }]);