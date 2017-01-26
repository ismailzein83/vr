'use strict';

app.directive('retailBeAccountsynchronizerhandlerAssignproductandpackages', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var assignProductAndPackagesAccountInsertHandler = new AssignProductAndPackagesAccountInsertHandler($scope, ctrl, $attrs);
                assignProductAndPackagesAccountInsertHandler.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountSynchronizerInsertHandlers/Templates/AssignProductAndPackagesAccountInsertHandlerTemplate.html'
        };

        function AssignProductAndPackagesAccountInsertHandler($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var productSelectorAPI;
            var productSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var packageSelectorAPI;
            var packageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onProductDirectiveReady = function (api) {
                    productSelectorAPI = api;
                    productSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onPackageSelectorReady = function (api) {
                    packageSelectorAPI = api;
                    packageSelectorReadyDeferred.resolve();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountBEDefinitionId;
                    var handlerSettings;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        handlerSettings = payload.Settings;
                    }

                    var productSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    productSelectorReadyPromiseDeferred.promise.then(function () {
                        var productSelectorPayload = {
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.AccountDefinitionProductFilter, Retail.BusinessEntity.Business",
                                    AccountBEDefinitionId: accountBEDefinitionId
                                }]
                            }
                        };
                        if (handlerSettings != undefined) {
                            productSelectorPayload.selectedIds = handlerSettings.ProductId;
                        }
                        VRUIUtilsService.callDirectiveLoad(productSelectorAPI, productSelectorPayload, productSelectorLoadPromiseDeferred);
                    });
                    promises.push(productSelectorLoadPromiseDeferred.promise);

                    promises.push(loadPackageSelector());

                    function loadPackageSelector() {
                        var packageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        packageSelectorReadyDeferred.promise.then(function () {

                            var packageSelectorPayload;
                            if (handlerSettings != undefined) {
                                packageSelectorPayload = {
                                    filter: {

                                        Filters: [{
                                            $type: "Retail.BusinessEntity.Business.ProductPackageFilter, Retail.BusinessEntity.Business",
                                            ProductId: handlerSettings.ProductId
                                        }]
                                    }
                                };
                            }
                            if (handlerSettings != undefined) {
                                packageSelectorPayload.selectedIds = handlerSettings.Packages;
                            }
                            VRUIUtilsService.callDirectiveLoad(packageSelectorAPI, packageSelectorPayload, packageSelectorLoadDeferred);
                        });

                        return packageSelectorLoadDeferred.promise;
                    };

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountSynchronizerInsertHandlers.AssignProductAndPackagesAccountInsertHandler, Retail.BusinessEntity.MainExtensions',
                        ProductId: productSelectorAPI.getSelectedIds(),
                        Packages: packageSelectorAPI.getSelectedIds(),
                        AssignementDate: $scope.scopeModel.assignementDate,
                        AssignementDaysOffsetFromToday: $scope.scopeModel.assignementDaysOffsetFromToday
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }
    }]);
