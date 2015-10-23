'use strict';
app.directive('vrWhsBeAllexceptsalezones', ['WhS_BE_SaleZoneAPIService', 'WhS_BE_SellingNumberPlanAPIService', 'UtilsService',
    function (WhS_BE_SaleZoneAPIService, WhS_BE_SellingNumberPlanAPIService, UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                salezonepackageid: "="
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                $scope.selectedSaleZones = [];

                $scope.sellingNumberPlans = [];
                $scope.selectedSellingNumberPlan = undefined;

                $scope.showSellingNumberPlan = ctrl.salezonepackageid == undefined;
                $scope.isPackageDefined = !$scope.showSellingNumberPlan;

                var beSaleZonesCtor = new beSaleZones(ctrl, $scope, WhS_BE_SaleZoneAPIService);
                beSaleZonesCtor.initializeController();

                $scope.onSellingNumberPlanValueChanged = function () {
                    $scope.isPackageDefined = (!$scope.showSellingNumberPlan || $scope.selectedSellingNumberPlan != undefined);
                }

                $scope.searchZones = function (filter) {
                    var packageId;
                    if (ctrl.salezonepackageid == undefined)
                        packageId = $scope.selectedSellingNumberPlan.SellingNumberPlanId;
                    else
                        packageId = ctrl.salezonepackageid;

                    return WhS_BE_SaleZoneAPIService.GetSaleZonesInfo(packageId, filter);
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
                return getBeAllExceptSaleZonesTemplate(attrs);
            }

        };

        function getBeAllExceptSaleZonesTemplate(attrs) {
            return '/Client/Modules/WhS_BusinessEntity/Directives/SaleZone/Templates/SelectiveSaleZonesDirectiveTemplate.html';
        }

        function beSaleZones(ctrl, $scope, WhS_BE_SaleZoneAPIService) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {
                    return WhS_BE_SellingNumberPlanAPIService.GetSellingNumberPlans().then(function (response) {
                        angular.forEach(response, function (item) {
                            $scope.sellingNumberPlans.push(item);
                        });
                    });
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups.AllExceptSaleZonesSettings, TOne.WhS.BusinessEntity.MainExtensions",
                        SellingNumberPlanId: ctrl.salezonepackageid == undefined ? $scope.selectedSellingNumberPlan.SellingNumberPlanId : ctrl.salezonepackageid,
                        ZoneIds: UtilsService.getPropValuesFromArray($scope.selectedSaleZones, "SaleZoneId")
                    };
                }

                api.setData = function (saleZoneGroupSettings) {
                    var sellingNumberPlanId;

                    if (ctrl.salezonepackageid == undefined) {
                        $scope.selectedSellingNumberPlan = UtilsService.getItemByVal($scope.sellingNumberPlans, saleZoneGroupSettings.SellingNumberPlanId, "SellingNumberPlanId");
                        sellingNumberPlanId = saleZoneGroupSettings.SellingNumberPlanId;
                    }
                    else {
                        sellingNumberPlanId = ctrl.salezonepackageid;
                    }

                    if (saleZoneGroupSettings.ZoneIds.length > 0) {
                        var input = { SellingNumberPlanId: sellingNumberPlanId, SaleZoneIds: saleZoneGroupSettings.ZoneIds };

                        WhS_BE_SaleZoneAPIService.GetSaleZonesInfoByIds(input).then(function (response) {
                            angular.forEach(response, function (item) {
                                $scope.selectedSaleZones.push(item);
                            });
                        }).catch(function (error) {
                            //TODO handle the case of exceptions
                        });
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);