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
        var saleZoneGroupDirectiveReadyPromiseDeferred;

        var customerGroupDirectiveAPI;
        var customerGroupDirectiveReadyPromiseDeferred;

        var directiveAppendixData ;

        function initializeController() {
            $scope.onCustomerGroupDirectiveReady = function (api) {
                customerGroupDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingCustomerGroupDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, customerGroupDirectiveAPI, undefined, setLoader, customerGroupDirectiveReadyPromiseDeferred);

            }
            $scope.onSaleZoneGroupDirectiveReady = function (api) {
                saleZoneGroupDirectiveAPI = api;
               
                var setLoader = function (value) { $scope.isLoadingSaleZoneGroupDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneGroupDirectiveAPI, undefined, setLoader, saleZoneGroupDirectiveReadyPromiseDeferred);
            }

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var saleZoneGroupSettings;
                if ($scope.selectedSaleZoneGroupTemplate != undefined) {
                    if (saleZoneGroupDirectiveAPI != undefined) {
                        saleZoneGroupSettings = saleZoneGroupDirectiveAPI.getData();
                        saleZoneGroupSettings.ConfigId = $scope.selectedSaleZoneGroupTemplate.TemplateConfigID;
                    }
                }


                var customerGroupSettings;
                if ($scope.selectedCustomerGroupTemplate != undefined) {
                    if (customerGroupDirectiveAPI != undefined) {
                        customerGroupSettings = customerGroupDirectiveAPI.getData();
                        customerGroupSettings.ConfigId = $scope.selectedCustomerGroupTemplate.TemplateConfigID;
                    }
                }

                var obj = {
                    SaleZoneGroupSettings: saleZoneGroupSettings,
                    CustomerGroupSettings: customerGroupSettings
                }
                return obj;
            }

            api.load = function (payload) {
                $scope.saleZoneGroupTemplates = [];
                $scope.customerGroupTemplates = [];
                var customerConfigId;
                var saleZoneConfigId;
                var customerGroupSettings;
                var saleZoneGroupSettings = { filter: {} };
                
                if (payload != undefined) {
                    if (payload.CustomerGroupSettings != null)
                    {
                        customerConfigId= payload.CustomerGroupSettings.ConfigId;
                        customerGroupSettings= payload.CustomerGroupSettings;
                    }
                       
                    if (payload.SaleZoneGroupSettings != null)
                    {
                       
                        saleZoneConfigId = payload.SaleZoneGroupSettings.ConfigId;
                        saleZoneGroupSettings.filter.SaleZoneFilterSettings = payload.SaleZoneGroupSettings;
                    }
                   
                }
                var promises = [];
                var loadCustomerGroupTemplatesPromise = WhS_BE_CarrierAccountAPIService.GetCustomerGroupTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.customerGroupTemplates.push(item);
                    });
                    if(customerConfigId!=undefined)
                        $scope.selectedCustomerGroupTemplate = UtilsService.getItemByVal($scope.customerGroupTemplates,customerConfigId, "TemplateConfigID")
                });
                promises.push(loadCustomerGroupTemplatesPromise);

                if (customerGroupSettings != undefined) {
                    customerGroupDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    var customerGroupDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(customerGroupDirectiveLoadPromiseDeferred.promise);

                    customerGroupDirectiveReadyPromiseDeferred.promise.then(function () {
                        customerGroupDirectiveReadyPromiseDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(customerGroupDirectiveAPI, customerGroupSettings, customerGroupDirectiveLoadPromiseDeferred);
                    });
                }

                var loadSaleZoneGroupTemplatesPromise = WhS_BE_SaleZoneAPIService.GetSaleZoneGroupTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.saleZoneGroupTemplates.push(item);
                    });

                    if(saleZoneConfigId!=undefined)
                        $scope.selectedSaleZoneGroupTemplate = UtilsService.getItemByVal($scope.saleZoneGroupTemplates, saleZoneConfigId, "TemplateConfigID")
                });
                promises.push(loadSaleZoneGroupTemplatesPromise);

                if (saleZoneGroupSettings.filter.SaleZoneFilterSettings != undefined) {
                    saleZoneGroupDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    var saleZoneGroupDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(saleZoneGroupDirectiveLoadPromiseDeferred.promise);

                    saleZoneGroupDirectiveReadyPromiseDeferred.promise.then(function () {
                        saleZoneGroupDirectiveReadyPromiseDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(saleZoneGroupDirectiveAPI, saleZoneGroupSettings, saleZoneGroupDirectiveLoadPromiseDeferred);
                    });
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