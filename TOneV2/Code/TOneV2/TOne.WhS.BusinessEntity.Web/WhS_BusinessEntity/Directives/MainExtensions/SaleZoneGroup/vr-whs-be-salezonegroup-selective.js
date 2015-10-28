﻿'use strict';
app.directive('vrWhsBeSalezonegroupSelective', ['WhS_BE_SaleZoneAPIService', 'WhS_BE_SellingNumberPlanAPIService', 'UtilsService',
    function (WhS_BE_SaleZoneAPIService, WhS_BE_SellingNumberPlanAPIService, UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            sellingnumberplanid: "="
        },
        controller: function ($scope, $element, $attrs) {

         
          
            var ctrl = this;
            
            $scope.selectedSaleZones = [];

            $scope.sellingNumberPlans = [];
            $scope.selectedSellingNumberPlan = undefined;

            $scope.showSellingNumberPlan = ctrl.sellingnumberplanid == undefined;
            $scope.isPackageDefined = !$scope.showSellingNumberPlan;

            var ctor = new selectiveCtor(ctrl, $scope, WhS_BE_SaleZoneAPIService);
            ctor.initializeController();

            $scope.onSellingNumberPlanValueChanged = function () {
                $scope.isPackageDefined = (!$scope.showSellingNumberPlan || $scope.selectedSellingNumberPlan != undefined);
            }
            $scope.searchZones = function (filter) {
                var sellingNumberPlanId;
                if (ctrl.sellingnumberplanid == undefined && $scope.selectedSellingNumberPlan != undefined)
                {
                    sellingNumberPlanId = $scope.selectedSellingNumberPlan.SellingNumberPlanId;
                }    
                else {
                    sellingNumberPlanId = ctrl.sellingnumberplanid;
                }
                   

                return WhS_BE_SaleZoneAPIService.GetSaleZonesInfo(sellingNumberPlanId, filter);
            }
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                   
                }
            }
        },
        templateUrl: function (element, attrs) {
            return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/SaleZoneGroup/Templates/SelectiveSaleZonesDirectiveTemplate.html';
        }

    };

    function selectiveCtor(ctrl, $scope, WhS_BE_SaleZoneAPIService) {
        
        function initializeController() {
            defineAPI();
        }

        function defineAPI()
        {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                var loadSellingNumberPlanPromise = WhS_BE_SellingNumberPlanAPIService.GetSellingNumberPlans().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.sellingNumberPlans.push(item);
                    });

                    if (payload != undefined)
                        $scope.selectedSellingNumberPlan = UtilsService.getItemByVal($scope.sellingNumberPlans, payload.SellingNumberPlanId, "SellingNumberPlanId");
                });

                promises.push(loadSellingNumberPlanPromise);

                if (payload != undefined && payload.ZoneIds.length > 0) {
                    var loadSaleZoneSelectorPromise = setSaleZoneSelector(payload);
                    promises.push(loadSaleZoneSelectorPromise);
                }

                return UtilsService.waitMultiplePromises(promises);
                
                function setSaleZoneSelector(payload) {
                    var sellingNumberPlanId;

                    if (ctrl.sellingnumberplanid == undefined)
                        sellingNumberPlanId = payload.SellingNumberPlanId;
                    else
                        sellingNumberPlanId = ctrl.sellingnumberplanid;

                    var input = { SellingNumberPlanId: sellingNumberPlanId, SaleZoneIds: payload.ZoneIds };

                    return WhS_BE_SaleZoneAPIService.GetSaleZonesInfoByIds(input).then(function (response) {
                        angular.forEach(response, function (item) {
                            $scope.selectedSaleZones.push(item);
                        });
                    });
                }
            }

            api.getData = function () {
                return {
                    $type: "TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups.SelectiveSaleZoneGroup, TOne.WhS.BusinessEntity.MainExtensions",
                    SellingNumberPlanId: ctrl.sellingnumberplanid == undefined ? $scope.selectedSellingNumberPlan.SellingNumberPlanId : ctrl.sellingnumberplanid,
                    ZoneIds: UtilsService.getPropValuesFromArray($scope.selectedSaleZones, "SaleZoneId")
                };
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);