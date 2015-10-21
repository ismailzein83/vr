'use strict';
app.directive('vrWhsBeSupplierswithzonesSelective', ['UtilsService', '$compile', 'WhS_BE_PricingRuleAPIService',
function (UtilsService, $compile, WhS_BE_PricingRuleAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var beSuppliersWithZonesObject = new beSuppliersWithZones(ctrl, $scope, $attrs);
            beSuppliersWithZonesObject.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/SuppliersWithZonesGroups/Templates/SelectiveSuppliersWithZonesGroup.html"

    };


    function beSuppliersWithZones(ctrl, $scope, $attrs) {
        var carrierAccountDirectiveAPI;
        var supplierZonesDirectiveAPI;
        function initializeController() {
            $scope.suppliers = [];
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                declareDirectiveAsReady()
            }
            $scope.onSelectionChanged = function () {
                if (carrierAccountDirectiveAPI != undefined) {
                    var obj = carrierAccountDirectiveAPI.getData();
                    $scope.suppliers.length = 0;
                    for (var i = 0; i < obj.length; i++) {
                        addSupplierZoneAPIFunction(obj[i]);
                    }
                    
                }
            };
            function addSupplierZoneAPIFunction(obj) {
                obj.onSupplierZonesDirectiveReady = function (api) {
                    obj.supplierZonesDirectiveAPI = api;
                    obj.supplierZonesDirectiveAPI.load();
                }
                $scope.suppliers.push(obj);
            }
            $scope.removeSupplier = function (supplier) {
                var index = UtilsService.getItemIndexByVal($scope.suppliers, supplier.CarrierAccountId, 'CarrierAccountId');
                $scope.suppliers.splice(index, 1);
            };
            
        }
        function declareDirectiveAsReady() {
            if (carrierAccountDirectiveAPI == undefined)
                return;

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var suppliersWithZones = [];
                for (var i = 0; i < $scope.suppliers.length; i++)
                {
                    suppliersWithZones.push({
                        SupplierId: $scope.suppliers[i].CarrierAccountId,
                        SupplierZoneIds: UtilsService.getPropValuesFromArray($scope.suppliers[i].supplierZonesDirectiveAPI.getData(), "SupplierZoneId"),
                    });
                }
                var obj = {
                    $type: "TOne.WhS.BusinessEntity.MainExtensions.SuppliersWithZonesGroups.SelectiveSuppliersWithZonesGroup,TOne.WhS.BusinessEntity.MainExtensions",
                    SuppliersWithZones: suppliersWithZones,
                }
                return obj;
            }
            
            api.setData = function (suppliersWithZones) {
                var supplierIds = [];
                for (var i = 0; i < suppliersWithZones.length; i++) {
                    var obj = suppliersWithZones[i];
                    supplierIds.push(obj.SupplierId);
                    carrierAccountDirectiveAPI.setData(supplierIds);
                    addAPIFunction(obj);
                }
                function addAPIFunction(obj) {
                    obj.onSupplierZonesDirectiveReady = function (api) {
                        obj.supplierZonesDirectiveAPI = api;
                        obj.supplierZonesDirectiveAPI.load();
                        obj.supplierZonesDirectiveAPI.setData(obj.SupplierZoneIds);
                    }
                }
            }
            api.load = function () {
                return carrierAccountDirectiveAPI.load();
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);