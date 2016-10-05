(function (appControllers) {

    'use strict';

    ZoneInfoController.$inject = ['$scope', 'UtilsService', 'VRNavigationService'];

    function ZoneInfoController($scope, UtilsService, VRNavigationService)
    {
        var ownerType;
        var ownerId;
        var zoneId;
        var zoneName;
        var zoneBED;
        var zoneEED;
        var currencyId;

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
            }
        }
        function defineScope()
        {
            $scope.title = 'Zone Info';
            if (zoneName != undefined)
                $scope.title += ': ' + zoneName;

            $scope.scopeModel = {};
            
            if (zoneBED != undefined)
                $scope.scopeModel.zoneBED = UtilsService.getShortDate(new Date(zoneBED));

            $scope.scopeModel.zoneEED = (zoneEED != undefined) ? UtilsService.getShortDate(new Date(zoneEED)) : 'NULL';

            $scope.scopeModel.directiveTabs = [{
                title: 'Rates',
                directive: 'vr-whs-be-salerate-grid',
                loadDirective: function (directiveAPI) {
                    var rateGridPayload = {
                        OwnerType: ownerType,
                        OwnerId: ownerId,
                        ZoneIds: [zoneId],
                        EffectiveOn: UtilsService.getDateFromDateTime(new Date()),
                        CurrencyId: currencyId
                    };
                    return directiveAPI.loadGrid(rateGridPayload);
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

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadGrid]).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadGrid() {
            gridReadyDeferred.promise.then(function () {
                if (calculatedRates != undefined && calculatedRates.InvalidCalculatedRates != null) {
                    for (var i = 0; i < calculatedRates.InvalidCalculatedRates.length; i++) {
                        var dataItem = calculatedRates.InvalidCalculatedRates[i];
                        dataItem.isExcluded = false;
                        $scope.scopeModel.invalidRates.push(dataItem);
                    }
                }
            });
        }
    }

    appControllers.controller('WhS_Sales_ZoneInfoController', ZoneInfoController);

})(appControllers);