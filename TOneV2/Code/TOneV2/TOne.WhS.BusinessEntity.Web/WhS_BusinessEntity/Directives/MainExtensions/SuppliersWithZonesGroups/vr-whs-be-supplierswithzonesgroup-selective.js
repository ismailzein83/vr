'use strict';
app.directive('vrWhsBeSupplierswithzonesSelective', ['UtilsService', '$compile', 'WhS_BE_PricingRuleAPIService','VRUIUtilsService',
function (UtilsService, $compile, WhS_BE_PricingRuleAPIService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new selectiveCtor(ctrl, $scope, $attrs);
            ctor.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/SuppliersWithZonesGroups/Templates/SelectiveSuppliersWithZonesGroup.html"

    };


    function selectiveCtor(ctrl, $scope, $attrs) {
        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.suppliers = [];
            ctrl.selectedSuppliers = [];
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            }

            $scope.onSelectionChanged = function () {
                if (carrierAccountDirectiveAPI != undefined) {
                    for (var i = 0; i < ctrl.selectedSuppliers.length; i++)
                    {
                        addSupplierZoneAPIFunction(ctrl.selectedSuppliers[i]); 
                    }
                }
            };
            function addSupplierZoneAPIFunction(obj) {
                obj.selectedSuplierZones = [];
                obj.onSupplierZonesDirectiveReady = function (api) {
                    obj.supplierZonesDirectiveAPI = api;
                    obj.supplierZonesDirectiveAPI.load();
                }
            }
            $scope.removeSupplier = function ($event, supplier) {
                $event.preventDefault();
                $event.stopPropagation();
                var index = UtilsService.getItemIndexByVal(ctrl.selectedSuppliers, supplier.CarrierAccountId, 'CarrierAccountId');
                ctrl.selectedSuppliers.splice(index, 1);
            };
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var suppliersWithZones = [];
                for (var i = 0; i < ctrl.selectedSuppliers.length; i++)
                {
                    suppliersWithZones.push({
                        $type: "TOne.WhS.BusinessEntity.Entities.SupplierWithZones,TOne.WhS.BusinessEntity.Entities",
                        SupplierId: ctrl.selectedSuppliers[i].CarrierAccountId,
                        SupplierZoneIds: UtilsService.getPropValuesFromArray(ctrl.selectedSuppliers[i].selectedSuplierZones, "SupplierZoneId"),
                    });
                }
               
                return suppliersWithZones;
            }
 
            api.load = function (payload) {
                var supplierIds = [];
                if (payload != undefined) {
                    for (var i = 0; i < payload.length; i++) {
                        var obj = payload[i];
                        supplierIds.push(obj.SupplierId);
                    }
                }
                

                var promises = [];
                var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();
                carrierAccountReadyPromiseDeferred.promise.then(function () {
                    var carrierAccountPayload = {
                        filter: {},
                        selectedIds: supplierIds
                    };
                    VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, carrierAccountPayload, loadCarrierAccountPromiseDeferred);

                   
                    if (payload != undefined) {
                        for (var i = 0; i < payload.length; i++) {
                            for (var j = 0; j < ctrl.selectedSuppliers.length; j++) {
                                if (ctrl.selectedSuppliers[j].CarrierAccountId == payload[i].SupplierId)
                                    var loadSellingNumberPlanPromise = ctrl.selectedSuppliers[j].supplierZonesDirectiveAPI.load(payload[i].SupplierZoneIds);
                                promises.push(loadSellingNumberPlanPromise);
                            }
                        }
                    }
                   

                });
                return UtilsService.waitMultiplePromises(promises);
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);