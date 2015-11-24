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
        var sellingProductDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        var customerGroupDirectiveAPI;
        var customerGroupDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

          
            $scope.onSaleZoneGroupDirectiveReady = function (api) {
                saleZoneGroupDirectiveAPI = api;
                saleZoneGroupDirectiveReadyPromiseDeferred.resolve();
              
            }
            $scope.onCustomerGroupDirectiveReady = function (api) {
                customerGroupDirectiveAPI = api;
                customerGroupDirectiveReadyPromiseDeferred.resolve();
            }
            $scope.onSellingProductDirectiveReady = function (api) {
                sellingProductDirectiveAPI = api;
                sellingProductDirectiveReadyPromiseDeferred.resolve();
            }
            loadTemplates();
            defineAPI();
        
        }
        function loadTemplates() {
            $scope.templates = [{
                description: "Customers",
                value: 0,
            },
            {
                description: "Selling Products",
                value: 1,
            }];
            $scope.selectedTemplate = $scope.templates[0];
        }

        function defineAPI() {
            var api = {};
            api.getData = function () {
                var obj = {
                    SaleZoneGroupSettings: saleZoneGroupDirectiveAPI.getData(),   
                }
                if ($scope.selectedTemplate != undefined) {
                    switch ($scope.selectedTemplate.value) {
                        case 0: obj.CustomerGroupSettings = customerGroupDirectiveAPI != undefined ? customerGroupDirectiveAPI.getData() : undefined
                        case 1: obj.SellingProductIds = sellingProductDirectiveAPI != undefined ? sellingProductDirectiveAPI.getSelectedIds() : undefined
                    }
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
                        customerGroupSettings = payload.CustomerGroupSettings;
                        $scope.selectedTemplate = $scope.templates[0];
                    }
                    if (payload.SaleZoneGroupSettings != null)
                    {
                        saleZoneGroupSettings = payload.SaleZoneGroupSettings;
                    }
                    if (payload.SellingProductIds != null)
                    {
                        sellingProductIds = payload.SellingProductIds;
                        $scope.selectedTemplate = $scope.templates[1];
                    }
                       
                }
                var promises = [];
                if (customerGroupDirectiveReadyPromiseDeferred == undefined)
                    customerGroupDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                var customerGroupDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(customerGroupDirectiveLoadPromiseDeferred.promise);
                customerGroupDirectiveReadyPromiseDeferred.promise.then(function () {
                    customerGroupDirectiveReadyPromiseDeferred = undefined;
                    var customerpayload = customerGroupSettings != undefined ? customerGroupSettings : undefined;
                    VRUIUtilsService.callDirectiveLoad(customerGroupDirectiveAPI, customerpayload, customerGroupDirectiveLoadPromiseDeferred);
                });

                if (saleZoneGroupDirectiveReadyPromiseDeferred == undefined)
                    saleZoneGroupDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                var saleZoneGroupDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(saleZoneGroupDirectiveLoadPromiseDeferred.promise);
                saleZoneGroupDirectiveReadyPromiseDeferred.promise.then(function () {
                    saleZoneGroupDirectiveReadyPromiseDeferred = undefined;
                    var saleZoneGroupPayload;
                    if (saleZoneGroupSettings != undefined) {
                        saleZoneGroupPayload = { saleZoneGroupSettings: saleZoneGroupSettings };
                    }
                    VRUIUtilsService.callDirectiveLoad(saleZoneGroupDirectiveAPI, saleZoneGroupPayload, saleZoneGroupDirectiveLoadPromiseDeferred);
                });

                if (sellingProductDirectiveReadyPromiseDeferred == undefined)
                    sellingProductDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                var sellingProductDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(sellingProductDirectiveLoadPromiseDeferred.promise);
                sellingProductDirectiveReadyPromiseDeferred.promise.then(function () {
                    sellingProductDirectiveReadyPromiseDeferred = undefined;
                    var sellingproductpayload = sellingProductIds != undefined ? sellingProductIds : undefined;
                    VRUIUtilsService.callDirectiveLoad(sellingProductDirectiveAPI, sellingproductpayload, sellingProductDirectiveLoadPromiseDeferred);
                });
                return UtilsService.waitMultiplePromises(promises);
            }




            if (ctrl.onReady != null)

                ctrl.onReady(api);
        }



        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);