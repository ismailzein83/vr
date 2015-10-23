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
        var directiveAppendixData;
        function initializeController() {
            $scope.suppliers = [];
            $scope.selectedSuppliers = [];
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                declareDirectiveAsReady()
            }
            $scope.onSelectionChanged = function () {
                if (carrierAccountDirectiveAPI != undefined) {
                    for (var i = 0; i < $scope.selectedSuppliers.length; i++)
                    {
                        addSupplierZoneAPIFunction($scope.selectedSuppliers[i]);
                        
                    }
                }
            };
            function addSupplierZoneAPIFunction(obj) {
                obj.onSupplierZonesDirectiveReady = function (api) {
                    obj.supplierZonesDirectiveAPI = api;
                    obj.supplierZonesDirectiveAPI.load();
                    if (directiveAppendixData != undefined)
                        trySetSupplierZoneDirectiveValues();
                }
            }
            $scope.removeSupplier = function ($event, supplier) {
                $event.preventDefault();
                $event.stopPropagation();
                var index = UtilsService.getItemIndexByVal($scope.selectedSuppliers, supplier.CarrierAccountId, 'CarrierAccountId');
                $scope.selectedSuppliers.splice(index, 1);
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
                for (var i = 0; i < $scope.selectedSuppliers.length; i++)
                {
                    suppliersWithZones.push({
                        $type: "TOne.WhS.BusinessEntity.Entities.SupplierWithZones,TOne.WhS.BusinessEntity.Entities",
                        SupplierId: $scope.selectedSuppliers[i].CarrierAccountId,
                        SupplierZoneIds: UtilsService.getPropValuesFromArray($scope.selectedSuppliers[i].supplierZonesDirectiveAPI.getData(), "SupplierZoneId"),
                    });
                }
               
                return suppliersWithZones;
            }
            
            api.setData = function (suppliersWithZones) {
                var supplierIds = [];
                for (var i = 0; i < suppliersWithZones.length; i++) {
                    var obj = suppliersWithZones[i];
                    supplierIds.push(obj.SupplierId);
                }
                carrierAccountDirectiveAPI.setData(supplierIds);
                directiveAppendixData = suppliersWithZones;
                
                trySetSupplierZoneDirectiveValues();
            }
            
            api.load = function () {
                return carrierAccountDirectiveAPI.load();
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        function trySetSupplierZoneDirectiveValues() {
            for (var i = 0; i < $scope.selectedSuppliers.length; i++) {
                if ($scope.selectedSuppliers[i].supplierZonesDirectiveAPI == undefined)
                     return;
            }
            for (var i = 0; i < directiveAppendixData.length; i++) {
                for (var j = 0; j < $scope.selectedSuppliers.length; j++) {
                    if ($scope.selectedSuppliers[j].CarrierAccountId == directiveAppendixData[i].SupplierId)
                        $scope.selectedSuppliers[j].supplierZonesDirectiveAPI.setData(directiveAppendixData[i].SupplierZoneIds);
                }
            }
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);