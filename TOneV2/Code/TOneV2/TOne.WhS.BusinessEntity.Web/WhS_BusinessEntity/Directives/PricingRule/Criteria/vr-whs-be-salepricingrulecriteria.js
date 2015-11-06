'use strict';
app.directive('vrWhsBeSalepricingrulecriteria', [ 'UtilsService', '$compile','WhS_BE_SaleZoneAPIService','WhS_BE_CarrierAccountAPIService','VRNotificationService','VRUIUtilsService',
function ( UtilsService, $compile, WhS_BE_SaleZoneAPIService, WhS_BE_CarrierAccountAPIService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new beSalePricingRule(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Criteria/Templates/SalePricingRuleCriteriaTemplate.html"

    };


    function beSalePricingRule(ctrl, $scope, $attrs) {


        var saleZoneGroupDirectiveAPI;
        var saleZoneGroupDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingProductDirectiveAPI;
        var sellingProductDirectiveReadyPromiseDeferred;


        var customerGroupDirectiveAPI;
        var customerGroupDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.onCustomerGroupDirectiveReady = function (api) {
                customerGroupDirectiveAPI = api;
                customerGroupDirectiveReadyPromiseDeferred.resolve();

            }
            
            $scope.onSellingProductDirectiveReady = function (api) {
                sellingProductDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingSellingProductDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sellingProductDirectiveAPI, undefined, setLoader, sellingProductDirectiveReadyPromiseDeferred);
            }
            $scope.onSaleZoneGroupDirectiveReady = function (api) {
                saleZoneGroupDirectiveAPI = api;
                saleZoneGroupDirectiveReadyPromiseDeferred.resolve();
              
            }

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    SaleZoneGroupSettings: saleZoneGroupDirectiveAPI.getData(),
                    CustomerGroupSettings: customerGroupDirectiveAPI!=undefined?customerGroupDirectiveAPI.getData():undefined,
                    SellingProductIds:sellingProductDirectiveAPI!=undefined?sellingProductDirectiveAPI.getSelectedIds():undefined
                }
                return obj;
            }

            api.load = function (payload) {
                var customerGroupSettings;
                var saleZoneGroupSettings;
                var sellingProductIds;
                if (payload != undefined) {
                    if (payload.CustomerGroupSettings != null)
                    {
                        customerGroupSettings= payload.CustomerGroupSettings;
                    }
                    if (payload.SaleZoneGroupSettings != null)
                    {
                        saleZoneGroupSettings = payload.SaleGroupSettings;
                    }
                    if (payload.SellingProductIds != null)
                        sellingProductIds = payload.SellingProductIds;
                }
                    var promises = [];
                
                    if (customerGroupSettings != undefined) {

                        if (customerGroupDirectiveReadyPromiseDeferred == undefined)
                            customerGroupDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                        var customerGroupDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(customerGroupDirectiveLoadPromiseDeferred.promise);

                        customerGroupDirectiveReadyPromiseDeferred.promise.then(function () {
                            customerGroupDirectiveReadyPromiseDeferred = undefined;
                            var customerpayload = customerGroupSettings != undefined ? customerGroupSettings : undefined;
                            VRUIUtilsService.callDirectiveLoad(customerGroupDirectiveAPI, customerpayload, customerGroupDirectiveLoadPromiseDeferred);
                        });
                    }
                    if (saleZoneGroupDirectiveReadyPromiseDeferred == undefined)
                       saleZoneGroupDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    var saleZoneGroupDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                   

                    saleZoneGroupDirectiveReadyPromiseDeferred.promise.then(function () {
                        saleZoneGroupDirectiveReadyPromiseDeferred = undefined;

                        var saleZoneGroupPayload;

                        if (saleZoneGroupSettings != undefined ) {
                            saleZoneGroupPayload = {
                                saleZoneGroupSettings: saleZoneGroupSettings.SaleZoneGroupSettings
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(saleZoneGroupDirectiveAPI, saleZoneGroupPayload, saleZoneGroupDirectiveLoadPromiseDeferred);

                    });
                    promises.push(saleZoneGroupDirectiveLoadPromiseDeferred.promise);

                    if (sellingProductIds != undefined) {
                        if (sellingProductDirectiveReadyPromiseDeferred == undefined)
                            sellingProductDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                        var sellingProductDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();


                        sellingProductDirectiveReadyPromiseDeferred.promise.then(function () {
                            sellingProductDirectiveReadyPromiseDeferred = undefined;



                            var sellingproductpayload = sellingProductIds != undefined ? sellingProductIds : undefined;
                            VRUIUtilsService.callDirectiveLoad(sellingProductDirectiveAPI, sellingproductpayload, sellingProductDirectiveLoadPromiseDeferred);

                        });
                        promises.push(sellingProductDirectiveLoadPromiseDeferred.promise);
                    }
                   


                 return  UtilsService.waitMultiplePromises(promises);
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);