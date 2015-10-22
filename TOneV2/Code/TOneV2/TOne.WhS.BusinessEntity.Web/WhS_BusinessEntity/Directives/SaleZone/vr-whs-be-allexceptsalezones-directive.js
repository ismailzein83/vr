'use strict';
app.directive('vrWhsBeAllexceptsalezones', ['WhS_BE_SaleZoneAPIService', 'WhS_BE_SaleZonePackageAPIService', 'UtilsService',
    function (WhS_BE_SaleZoneAPIService, WhS_BE_SaleZonePackageAPIService, UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                salezonepackageid: "="
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                $scope.selectedSaleZones = [];

                $scope.saleZonePackages = [];
                $scope.selectedSaleZonePackage = undefined;

                $scope.showSaleZonePackage = ctrl.salezonepackageid == undefined;
                $scope.isPackageDefined = !$scope.showSaleZonePackage;

                var beSaleZonesCtor = new beSaleZones(ctrl, $scope, WhS_BE_SaleZoneAPIService);
                beSaleZonesCtor.initializeController();

                $scope.onSaleZonePackageValueChanged = function () {
                    $scope.isPackageDefined = (!$scope.showSaleZonePackage || $scope.selectedSaleZonePackage != undefined);
                }

                $scope.searchZones = function (filter) {
                    var packageId;
                    if (ctrl.salezonepackageid == undefined)
                        packageId = $scope.selectedSaleZonePackage.SaleZonePackageId;
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
                    return WhS_BE_SaleZonePackageAPIService.GetSaleZonePackages().then(function (response) {
                        angular.forEach(response, function (item) {
                            $scope.saleZonePackages.push(item);
                        });
                    });
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups.AllExceptSaleZonesSettings, TOne.WhS.BusinessEntity.MainExtensions",
                        SaleZonePackageId: ctrl.salezonepackageid == undefined ? $scope.selectedSaleZonePackage.SaleZonePackageId : ctrl.salezonepackageid,
                        ZoneIds: UtilsService.getPropValuesFromArray($scope.selectedSaleZones, "SaleZoneId")
                    };
                }

                api.setData = function (saleZoneGroupSettings) {
                    var packageId;

                    if (ctrl.salezonepackageid == undefined) {
                        $scope.selectedSaleZonePackage = UtilsService.getItemByVal($scope.saleZonePackages, saleZoneGroupSettings.SaleZonePackageId, "SaleZonePackageId");
                        packageId = saleZoneGroupSettings.SaleZonePackageId;
                    }
                    else {
                        packageId = ctrl.salezonepackageid;
                    }

                    if (saleZoneGroupSettings.ZoneIds.length > 0) {
                        var input = { PackageId: packageId, SaleZoneIds: saleZoneGroupSettings.ZoneIds };

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