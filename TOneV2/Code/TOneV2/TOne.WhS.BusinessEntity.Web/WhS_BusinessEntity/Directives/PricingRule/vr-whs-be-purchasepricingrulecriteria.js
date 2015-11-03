﻿'use strict';
app.directive('vrWhsBePurchasepricingrulecriteria', ['UtilsService', '$compile', 'WhS_BE_PricingRuleAPIService','WhS_BE_CarrierAccountAPIService','VRUIUtilsService','VRNotificationService',
function (UtilsService, $compile, WhS_BE_PricingRuleAPIService, WhS_BE_CarrierAccountAPIService, VRUIUtilsService, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new bePurchasePricingRuleCriteria(ctrl, $scope, $attrs);
            ctor.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Templates/PurchasePricingRuleCriteriaTemplate.html"

    };


    function bePurchasePricingRuleCriteria(ctrl, $scope, $attrs) {
        var directiveReadyAPI;
        var directiveReadyPromiseDeferred;
        function initializeController() {
            $scope.onDirectiveReady = function (api) {
                directiveReadyAPI = api;
                var setLoader = function (value) { $scope.isLoadingDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveReadyAPI, undefined, setLoader, directiveReadyPromiseDeferred);
            }
            $scope.onSelectionChanged = function () {
                if (carrierAccountDirectiveAPI!=undefined)
                    $scope.suppliers = carrierAccountDirectiveAPI.getData();
            };
            defineAPI();
        }
        function defineAPI() {
            var api = {};
            api.getData = function () {
                var obj = {};
                if (ctrl.selectedSuppliersWithZonesStettingsTemplate != undefined)
                {
                    obj = {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.SuppliersWithZonesGroups.SelectiveSuppliersWithZonesGroup,TOne.WhS.BusinessEntity.MainExtensions",
                        SuppliersWithZones: suppliersWithZonesGroupsDirectiveAPI.getData(),
                        ConfigId: ctrl.selectedSuppliersWithZonesStettingsTemplate.TemplateConfigID,
                    }
                    var suppliersWithZonesGroupSettings = {
                        SuppliersWithZonesGroupSettings: obj
                    }
                    return suppliersWithZonesGroupSettings;
                }
               else
                    return obj;
            }

            api.load = function (payload) {
                $scope.suppliersWithZonesStettingsTemplates = [];
                var suppliersWithZones ;
                var configId;
                if (payload != undefined)
                {
                    suppliersWithZones = payload.SuppliersWithZonesGroupSettings.SuppliersWithZones;
                    configId=payload.SuppliersWithZonesGroupSettings.ConfigId;
                }
                var promises = [];
                var loadTemplatesPromise = WhS_BE_CarrierAccountAPIService.GetSuppliersWithZonesGroupsTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.suppliersWithZonesStettingsTemplates.push(item);
                    });
                    
                    if (configId != undefined)
                        $scope.selectedSuppliersWithZonesStettingsTemplate = UtilsService.getItemByVal($scope.suppliersWithZonesStettingsTemplates, configId, "TemplateConfigID");
                });
                promises.push(loadTemplatesPromise);
                if (suppliersWithZones != undefined) {
                    directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(directiveLoadPromiseDeferred.promise);

                    directiveReadyPromiseDeferred.promise.then(function () {
                        directiveReadyPromiseDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(directiveReadyAPI, suppliersWithZones, directiveLoadPromiseDeferred);
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