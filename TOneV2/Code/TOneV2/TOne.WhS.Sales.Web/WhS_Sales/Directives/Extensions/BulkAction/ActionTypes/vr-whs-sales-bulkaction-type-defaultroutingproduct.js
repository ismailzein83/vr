'use strict';

app.directive('vrWhsSalesBulkactionTypeDefaultroutingproduct', ['WhS_Sales_RatePlanAPIService', 'WhS_Sales_BulkActionUtilsService', 'WhS_BE_SalePriceListOwnerTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRDateTimeService',
    function (WhS_Sales_RatePlanAPIService, WhS_Sales_BulkActionUtilsService, WhS_BE_SalePriceListOwnerTypeEnum, UtilsService, VRUIUtilsService, VRNotificationService, VRDateTimeService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var defaultRoutingProductBulkActionType = new DefaultRoutingProductBulkActionType($scope, ctrl, $attrs);
                defaultRoutingProductBulkActionType.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function DefaultRoutingProductBulkActionType($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var bulkActionContext;

            var routingProductSelectorAPI;
            var routingProductSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRoutingProductSelectorReady = function (api) {
                    routingProductSelectorAPI = api;
                    routingProductSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onRoutingProductSelected = function (selectedRoutingProduct) {
                    WhS_Sales_BulkActionUtilsService.onBulkActionChanged(bulkActionContext);
                };

                UtilsService.waitMultiplePromises([routingProductSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var routingProductId;

                    if (payload != undefined) {
                        bulkActionContext = payload.bulkActionContext;

                        if (payload.bulkAction != undefined) {
                            routingProductId = payload.bulkAction.DefaultRoutingProductId;
                        }
                    }

                    var promises = [];

                    var ownerSellingNumberPlanId;
                    var ownerType;
                    var ownerId;

                    if (bulkActionContext != undefined) {
                        ownerSellingNumberPlanId = bulkActionContext.ownerSellingNumberPlanId;
                        ownerType = bulkActionContext.ownerType;
                        ownerId = bulkActionContext.ownerId;
                    }

                    var defaultData;

                    var getDefaultDataDeferred = UtilsService.createPromiseDeferred();
                    promises.push(getDefaultDataDeferred.promise);

                    var loadRoutingProductSelectorDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadRoutingProductSelectorDeferred.promise);

                    getDefaultDataDeferred.promise.then(function () {
                        loadRoutingProductSelector().then(function () {
                            loadRoutingProductSelectorDeferred.resolve();
                        }).catch(function (error) {
                            loadRoutingProductSelectorDeferred.reject(error);
                        });
                    });

                    if (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
                        getDefaultData().then(function () {
                            getDefaultDataDeferred.resolve();
                        }).catch(function (error) {
                            getDefaultDataDeferred.reject(error);
                        });
                    }
                    else {
                        getDefaultDataDeferred.resolve();
                    }

                    function getDefaultData() {
                        var effectiveOn = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());
                        return WhS_Sales_RatePlanAPIService.GetDefaultItem(ownerType, ownerId, effectiveOn).then(function (response) {
                            defaultData = response;
                        });
                    }
                    function loadRoutingProductSelector() {
                        var routingProductSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        var routingProductSelectorPayload = {
                            filter: {},
                            selectedIds: routingProductId
                        };
                        if (bulkActionContext != undefined) {
                            routingProductSelectorPayload.filter.SellingNumberPlanId = bulkActionContext.ownerSellingNumberPlanId;
                            routingProductSelectorPayload.filter.AssignableToOwnerType = bulkActionContext.ownerType;
                            routingProductSelectorPayload.filter.AssignableToOwnerId = bulkActionContext.ownerId;
                        }
                        if (defaultData != undefined && defaultData.IsCurrentRoutingProductEditable === true) {
                            routingProductSelectorPayload.defaultItems = [{
                                RoutingProductId: -1,
                                Name: '(Reset to Default)'
                            }];
                        }
                        VRUIUtilsService.callDirectiveLoad(routingProductSelectorAPI, routingProductSelectorPayload, routingProductSelectorLoadDeferred);
                        return routingProductSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var selectedRoutingProductId = routingProductSelectorAPI.getSelectedIds();
                    return {
                        $type: 'TOne.WhS.Sales.MainExtensions.DefaultRoutingProductBulkActionType, TOne.WhS.Sales.MainExtensions',
                        DefaultRoutingProductId: (selectedRoutingProductId != -1) ? selectedRoutingProductId : null
                    };
                };

                api.getSummary = function () {
                    var routingProductName = ($scope.scopeModel.selectedRoutingProduct != undefined) ? $scope.scopeModel.selectedRoutingProduct.Name : 'None';
                    return 'Default Routing Product: ' + routingProductName;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }

        function getTemplate(attrs) {
            return '<vr-columns colnum="{{ctrl.normalColNum}}">\
					<vr-whs-be-routingproduct-selector on-ready="scopeModel.onRoutingProductSelectorReady"\
						selectedvalues="scopeModel.selectedRoutingProduct"\
						onselectitem="scopeModel.onRoutingProductSelected"\
						isrequired="ctrl.isrequired"\
						hideremoveicon="ctrl.isrequired">\
					</vr-whs-be-routingproduct-selector>\
				</vr-columns>';
        }
    }]);