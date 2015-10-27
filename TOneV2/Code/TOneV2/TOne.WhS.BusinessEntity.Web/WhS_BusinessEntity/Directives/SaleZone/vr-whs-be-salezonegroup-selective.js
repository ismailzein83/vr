'use strict';
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

            var beSaleZonesCtor = new beSaleZones(ctrl, $scope, WhS_BE_SaleZoneAPIService);
            beSaleZonesCtor.initializeController();
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
            return getBeSelectiveSaleZonesTemplate(attrs);
        }

    };

    function getBeSelectiveSaleZonesTemplate(attrs) {
        return '/Client/Modules/WhS_BusinessEntity/Directives/SaleZone/Templates/SelectiveSaleZonesDirectiveTemplate.html';
    }

    function beSaleZones(ctrl, $scope, WhS_BE_SaleZoneAPIService) {
        
        function initializeController() {
            defineAPI();
        }

        function defineAPI()
        {
            var api = {};

            api.load = function (saleZoneGroupSettings) {
                var promises = [];

                var loadSellingNumberPlanPromise = WhS_BE_SellingNumberPlanAPIService.GetSellingNumberPlans().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.sellingNumberPlans.push(item);
                    });

                    if (saleZoneGroupSettings != undefined)
                        $scope.selectedSellingNumberPlan = UtilsService.getItemByVal($scope.sellingNumberPlans, saleZoneGroupSettings.SellingNumberPlanId, "SellingNumberPlanId");
                });

                promises.push(loadSellingNumberPlanPromise);

                if (saleZoneGroupSettings != undefined && saleZoneGroupSettings.ZoneIds.length > 0) {
                    var loadSaleZoneSelectorPromise = setSaleZoneSelector(saleZoneGroupSettings);
                    promises.push(loadSaleZoneSelectorPromise);
                }

                return UtilsService.waitMultiplePromises(promises);
                
                function setSaleZoneSelector(saleZoneGroupSettings) {
                    var sellingNumberPlanId;

                    if (ctrl.sellingnumberplanid == undefined)
                        sellingNumberPlanId = saleZoneGroupSettings.SellingNumberPlanId;
                    else
                        sellingNumberPlanId = ctrl.sellingnumberplanid;

                    var input = { SellingNumberPlanId: sellingNumberPlanId, SaleZoneIds: saleZoneGroupSettings.ZoneIds };

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