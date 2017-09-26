(function (appControllers) {

    'use strict';

    ZoneInfoController.$inject = ['$scope', 'WhS_BE_SalePriceListOwnerTypeEnum', 'UtilsService', 'VRNavigationService', 'VRDateTimeService'];

    function ZoneInfoController($scope, WhS_BE_SalePriceListOwnerTypeEnum, UtilsService, VRNavigationService, VRDateTimeService) {
        var ownerType;
        var ownerId;
        var zoneId;
        var zoneName;
        var zoneBED;
        var zoneEED;
        var currencyId;
        var countryId;
        var primarySaleEntity;

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
                primarySaleEntity = parameters.primarySaleEntity;
            }
        }
        function defineScope() {
            $scope.title = 'History';
            $scope.scopeModel = {};

            if (zoneBED != undefined)
                $scope.scopeModel.zoneBED = UtilsService.getShortDate(new Date(zoneBED));

            $scope.scopeModel.zoneName = zoneName;
            $scope.scopeModel.zoneEED = (zoneEED != undefined) ? UtilsService.getShortDate(new Date(zoneEED)) : undefined;

            $scope.scopeModel.directiveTabs = [{
                title: 'Rate History',
                directive: 'vr-whs-be-sale-rate-history-grid',
                loadDirective: function (directiveAPI) {
                    var saleRateHistoryGridPayload = {
                        query: {
                            OwnerType: ownerType,
                            OwnerId: ownerId,
                            ZoneName: zoneName,
                            CountryId: countryId,
                            CurrencyId: currencyId
                        }
                    };
                    saleRateHistoryGridPayload.primarySaleEntity = primarySaleEntity;
                    return directiveAPI.load(saleRateHistoryGridPayload);
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
                        EffectiveOn: UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime())
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