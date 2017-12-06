'use strict';

app.directive('vrWhsSalesBulkactionTypeRoutingproduct', ['WhS_Sales_BulkActionUtilsService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_BE_SalePriceListOwnerTypeEnum', function (WhS_Sales_BulkActionUtilsService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_BE_SalePriceListOwnerTypeEnum) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var routingProductBulkActionType = new RoutingProductBulkActionType($scope, ctrl, $attrs);
            routingProductBulkActionType.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function RoutingProductBulkActionType($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var bulkActionContext;

        var routingProductSelectorAPI;
        var _selectedRoutingProduct;

        var rateSourceSelectorAPI;
        var rateSourceSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var resetData = false;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.followRateDate = true;
            $scope.scopeModel.showRateSource = false;

            $scope.scopeModel.onSelectorReady = function (api) {
                routingProductSelectorAPI = api;
                defineAPI();
            };
            $scope.scopeModel.onRateSourceSelectorReady = function (api) {
                rateSourceSelectorAPI = api;
                rateSourceSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onRoutingProductSelected = function (selectedRoutingProduct) {
                if (selectedRoutingProduct.IsDefinedForAllZones == false)
                    VRNotificationService.showWarning("Routing product " + selectedRoutingProduct.Name + " is defined only for specific zones");
                _selectedRoutingProduct = selectedRoutingProduct;
                    WhS_Sales_BulkActionUtilsService.onBulkActionChanged(bulkActionContext);
            };          
        }

        function defineAPI() {

            var api = {};

            api.load = function (payload) {

                var routingProductId;

                if (payload != undefined) {
                    bulkActionContext = payload.bulkActionContext;
                    if (payload.bulkAction != undefined) {
                        routingProductId = payload.bulkAction.RoutingProductId;
                        $scope.scopeModel.followRateDate = payload.bulkAction.ApplyNewNormalRateBED;
                    }
                    if (bulkActionContext.ownerType != undefined && bulkActionContext.ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value)
                        $scope.scopeModel.showRateSource = true;
                }

                var promises = [];

                var loadRoutingProductSelectorPromise = loadRoutingProductSelector();
                promises.push(loadRoutingProductSelectorPromise);

                function loadRoutingProductSelector() {
                    var routingProductSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var routingProductSelectorPayload = {
                        filter: {},
                        selectedIds: routingProductId
                    };
                    if (bulkActionContext != undefined) {
                        routingProductSelectorPayload.filter.SellingNumberPlanId = bulkActionContext.ownerSellingNumberPlanId;
                    }
                    VRUIUtilsService.callDirectiveLoad(routingProductSelectorAPI, routingProductSelectorPayload, routingProductSelectorLoadDeferred);
                    return routingProductSelectorLoadDeferred.promise;
                }
                var loadRateSourcePromise = loadRateSourceSelector();
                promises.push(loadRateSourcePromise);

                function loadRateSourceSelector() {
                    var rateSourceLoadDeferred = UtilsService.createPromiseDeferred();
                    rateSourceSelectorReadyDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(rateSourceSelectorAPI, undefined, rateSourceLoadDeferred);
                    });
                    return rateSourceLoadDeferred.promise;
                }
                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.Sales.MainExtensions.RoutingProductBulkActionType, TOne.WhS.Sales.MainExtensions',
                    RoutingProductId:(_selectedRoutingProduct != undefined) ? _selectedRoutingProduct.RoutingProductId : undefined,
                    ApplyNewNormalRateBED: $scope.scopeModel.followRateDate,
                    RateSources: rateSourceSelectorAPI.getSelectedIds()
                };
            };

            api.getSummary = function () {
                var routingProductName = ($scope.scopeModel.selectedRoutingProduct != undefined) ? $scope.scopeModel.selectedRoutingProduct.Name : 'None';
                var followsRateDateAsString = ($scope.scopeModel.followRateDate === true) ? 'Follows Rate Date' : 'Does Not Follow Rate Date';
                var rateSourcesText = 'None';
                if (rateSourceSelectorAPI.getSelectedIds() != undefined)
                    rateSourcesText = rateSourceSelectorAPI.getSelectedText().join(',');

                var summary = 'Routing Product: ' + routingProductName + ' | ' + followsRateDateAsString;
                if ($scope.scopeModel.showRateSource == true)
                    summary += ' | Apply on Rates: ' + rateSourcesText;
                return summary;
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }

    function getTemplate(attrs) {
        return '<vr-columns colnum="{{ctrl.normalColNum}}">\
					<vr-whs-be-routingproduct-selector on-ready="scopeModel.onSelectorReady"\
						selectedvalues="scopeModel.selectedRoutingProduct"\
						onselectitem="scopeModel.onRoutingProductSelected"\
                        onbeforeselectionchanged="scopeModel.onRoutingProductBeforeSelectionChanged"\
						isrequired="ctrl.isrequired"\
						hideremoveicon="ctrl.isrequired">\
					</vr-whs-be-routingproduct-selector>\
				</vr-columns>\
				<vr-columns colnum="{{ctrl.normalColNum}}"  ng-show="scopeModel.showRateSource">\
                    <vr-whs-sales-ratesource-selector customelabel="Apply on Rates" ismultipleselection on-ready="scopeModel.onRateSourceSelectorReady"></vr-whs-sales-ratesource-selector>\
				</vr-columns>\
		        <vr-columns colnum="{{ctrl.normalColNum}}">\
					<vr-switch label="Follow Rate Date" value="scopeModel.followRateDate" isrequired="ctrl.isrequired"></vr-switch>\
                </vr-columns>';
    }
}]);