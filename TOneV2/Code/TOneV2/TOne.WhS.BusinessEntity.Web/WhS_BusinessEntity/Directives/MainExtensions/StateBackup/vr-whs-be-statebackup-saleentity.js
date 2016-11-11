"use strict";

app.directive("vrWhsBeStatebackupSaleentity", ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_BE_SalePriceListOwnerTypeEnum',
    function (UtilsService, VRUIUtilsService, VRNotificationService, WhS_BE_SalePriceListOwnerTypeEnum) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var stateBackupSaleEntityConstructor = new StateBackupSaleEntityConstructor($scope, ctrl);
                stateBackupSaleEntityConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/Whs_BusinessEntity/Directives/MainExtensions/StateBackup/Templates/SaleEntityStateBackupEditor.html"
        };

        function StateBackupSaleEntityConstructor($scope, ctrl) {

            var ownerTypeSelectorAPI;
            var ownerTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var sellingProductSelectorAPI;
            var sellingProductSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var carrierAccountSelectorAPI;
            var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.selectedSellingProduct = [];

                $scope.selectedCustomer = [];

                $scope.onOwnerTypeSelectorReady = function (api) {
                    ownerTypeSelectorAPI = api;
                    ownerTypeSelectorReadyPromiseDeferred.resolve();
                };

                $scope.onSellingProductSelectorReady = function (api) {
                    sellingProductSelectorAPI = api;
                    sellingProductSelectorReadyDeferred.resolve();
                };


                $scope.onCarrierAccountSelectorReady = function (api) {
                    carrierAccountSelectorAPI = api;
                    carrierAccountSelectorReadyDeferred.resolve();
                };


                $scope.onOwnerTypeChanged = function (item) {

                    var selectedId = ownerTypeSelectorAPI.getSelectedIds();

                    if (selectedId == undefined)
                        return;

                    if (selectedId == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) {
                        $scope.showSellingProductSelector = true;
                        $scope.showCarrierAccountSelector = false;
                        $scope.selectedCustomer.length = 0;
                    }
                    else if (selectedId == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
                        $scope.showSellingProductSelector = false;
                        $scope.showCarrierAccountSelector = true;
                        $scope.selectedSellingProduct.length = 0;
                    }
                    carrierAccountSelectorAPI.clearSelection
                };

                UtilsService.waitMultiplePromises([ownerTypeSelectorReadyPromiseDeferred.promise, carrierAccountSelectorReadyDeferred.promise, sellingProductSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {

                var api = {};

                api.getData = function () {
                    var ownerType = ownerTypeSelectorAPI.getSelectedIds();

                    if (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value)
                        return {
                            $type: "TOne.WhS.BusinessEntity.Entities.SaleEntityStateBackupFilter, TOne.WhS.BusinessEntity.Entities",
                            OwnerType: ownerType,
                            OwnerIds: sellingProductSelectorAPI.getSelectedIds()
                        }

                    else
                        return {
                            $type: "TOne.WhS.BusinessEntity.Entities.SaleEntityStateBackupFilter, TOne.WhS.BusinessEntity.Entities",
                            OwnerType: ownerType,
                            OwnerIds: carrierAccountSelectorAPI.getSelectedIds()
                        }

                };

                api.load = function (payload) {
                    return UtilsService.waitMultipleAsyncOperations([loadSaleEntitySection])
                         .catch(function (error) {
                             VRNotificationService.notifyExceptionWithClose(error, $scope);
                         });
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadSaleEntitySection() {

                var promises = [];

                var ownerTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(ownerTypeSelectorLoadDeferred.promise);

                ownerTypeSelectorReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(ownerTypeSelectorAPI, undefined, ownerTypeSelectorLoadDeferred);
                });

                var sellingProductSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(sellingProductSelectorLoadDeferred.promise);

                sellingProductSelectorReadyDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(sellingProductSelectorAPI, undefined, sellingProductSelectorLoadDeferred);
                });

                var carrierAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(carrierAccountSelectorLoadDeferred.promise);

                carrierAccountSelectorReadyDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, undefined, carrierAccountSelectorLoadDeferred);
                });

                return UtilsService.waitMultiplePromises(promises);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);
