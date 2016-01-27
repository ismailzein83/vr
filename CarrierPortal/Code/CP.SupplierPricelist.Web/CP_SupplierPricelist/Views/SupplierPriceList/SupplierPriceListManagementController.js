(
    function (appControllers) {
        "use strict";

        function supplierPriceListManagementController($scope, utilsService, vrNotificationService, vruiUtilsService, priceListTypeEnum, supplierPriceListAPIService) {
            var gridAPI;
            function importPriceList() {
                var priceListObject = {
                    FileId: $scope.file.fileId,
                    PriceListType: $scope.selectedpriceListType.pricelistID,
                    EffectiveOnDate: $scope.effectiveOn,
                    UserId: 1,
                    Satus: 0
                };
                supplierPriceListAPIService.importPriceList(priceListObject);
            }

            function defineScope() {
                $scope.priceListTypes = [];
                $scope.importPriceList = function () {
                    importPriceList();
                }
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    api.loadGrid();
                }
            }

            defineScope();

            function loadPriceListType() {
                angular.forEach(priceListTypeEnum, function (value, key) {
                    $scope.priceListTypes.push({ pricelistID: value.ID, title: value.Value });
                }
                );
            }

            function load() {
                loadPriceListType();
            }

            load();
        }

        supplierPriceListManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'CP_SupplierPriceList_PriceListTypeEnum', 'CP_SupplierPricelist_SupplierPriceListAPIService'];
        appControllers.controller('CP_SupplierPriceList_SupplierPriceListManagementController', supplierPriceListManagementController);
    }
)(appControllers);