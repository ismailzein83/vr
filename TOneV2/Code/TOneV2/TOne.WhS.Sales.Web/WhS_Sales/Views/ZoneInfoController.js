(function (appControllers) {

    'use strict';

    ZoneInfoController.$inject = ['$scope', 'WhS_BE_SalePriceListOwnerTypeEnum', 'UtilsService', 'VRNavigationService'];

    function ZoneInfoController($scope, WhS_BE_SalePriceListOwnerTypeEnum, UtilsService, VRNavigationService) {
        var ownerType;
        var ownerId;
        var zoneId;
        var zoneName;
        var zoneBED;
        var zoneEED;
        var currencyId;
        var countryId;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                ownerType = parameters.ownerType;
                ownerId = parameters.ownerId;
                zoneId = parameters.zoneId;
                zoneName = parameters.zoneName;
                zoneBED = parameters.zoneBED;
                zoneEED = parameters.zoneEED;
                currencyId = parameters.currencyId;
                countryId = parameters.countryId;
            }
        }
        function defineScope() {
            $scope.title = 'Zone Info';
            if (zoneName != undefined)
                $scope.title += ': ' + zoneName;

            $scope.scopeModel = {};

            if (zoneBED != undefined)
                $scope.scopeModel.zoneBED = UtilsService.getShortDate(new Date(zoneBED));

            $scope.scopeModel.zoneEED = (zoneEED != undefined) ? UtilsService.getShortDate(new Date(zoneEED)) : 'NULL';

            var saleRateHistoryGridDirectiveName = (ownerType === WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) ?
                'vr-whs-be-salerate-history-sellingproduct-grid' : 'vr-whs-be-salerate-history-customer-grid';

            $scope.scopeModel.directiveTabs = [{
                title: 'Rate History',
                directive: saleRateHistoryGridDirectiveName,
                loadDirective: function (directiveAPI) {
                    var payload = {
                        ZoneName: zoneName,
                        CountryId: countryId
                    };

                    var ownerIdPropertyName = (ownerType === WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) ? 'SellingProductId' : 'CustomerId';
                    payload[ownerIdPropertyName] = ownerId;

                    return directiveAPI.load(payload);
                }
            }, {
                title: 'Codes',
                directive: 'vr-whs-be-salecode-grid',
                loadDirective: function (directiveAPI) {
                    var queryHandler = {
                        $type: 'TOne.WhS.BusinessEntity.Business.SaleCodeQueryByZoneHandler, TOne.WhS.BusinessEntity.Business'
                    };
                    queryHandler.Query = {
                        $type: 'TOne.WhS.BusinessEntity.Entities.SaleCodeQueryByZone, TOne.WhS.BusinessEntity.Entities',
                        ZoneId: zoneId,
                        EffectiveOn: UtilsService.getDateFromDateTime(new Date())
                    };
                    var codeGridPayload = {
                        queryHandler: queryHandler,
                        hidesalezonecolumn: true
                    };
                    return directiveAPI.loadGrid(codeGridPayload);
                }
            }];

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {

        }
    }

    appControllers.controller('WhS_Sales_ZoneInfoController', ZoneInfoController);

})(appControllers);