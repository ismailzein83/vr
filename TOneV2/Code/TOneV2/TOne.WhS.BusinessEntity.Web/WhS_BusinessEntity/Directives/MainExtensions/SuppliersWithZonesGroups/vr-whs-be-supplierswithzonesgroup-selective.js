'use strict';
app.directive('vrWhsBeSupplierswithzonesSelective', ['UtilsService', '$compile','VRUIUtilsService',
function (UtilsService, $compile, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '='
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
            ctrl.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            };
            ctrl.isValid = function () {

                if (ctrl.datasource.length > 0)
                    return null;
                return "You Should Select at least one Supplier ";
            };
            ctrl.datasource = [];
            ctrl.onSelectItem = function (dataItem) {
                addSupplierZoneAPIFunction(dataItem);
            };
            ctrl.onDeselectItem = function (dataItem) {
                var datasourceIndex = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.CarrierAccountId, 'CarrierAccountId');
                ctrl.datasource.splice(datasourceIndex, 1);
            };
            function addSupplierZoneAPIFunction(obj) {
                var dataItem = {
                    CarrierAccountId: obj.CarrierAccountId,
                    name: obj.Name,
                    selectedSuplierZones: []
                };
                dataItem.onDirectiveReady = function (api) {
                    dataItem.directiveAPI = api;
                    var supplierId = obj.CarrierAccountId;
                    if (supplierId != undefined) {
                        var setLoader = function (value) { $scope.isLoadingSupplierZonesSelector = value };

                        var payload = {
                            supplierId: supplierId
                        };

                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, payload, setLoader);
                    }
                };
              
                ctrl.datasource.push(dataItem);
            }
            ctrl.removeFilter = function (dataItem) {
                var index = UtilsService.getItemIndexByVal(ctrl.selectedSuppliers, dataItem.CarrierAccountId, 'CarrierAccountId');
                ctrl.selectedSuppliers.splice(index, 1);
                var datasourceIndex = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.CarrierAccountId, 'CarrierAccountId');
                ctrl.datasource.splice(datasourceIndex, 1);
            };
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var suppliersWithZones = [];
                for (var i = 0; i < ctrl.datasource.length; i++) {
                    suppliersWithZones.push({
                        $type: "TOne.WhS.BusinessEntity.Entities.SupplierWithZones,TOne.WhS.BusinessEntity.Entities",
                        SupplierId: ctrl.datasource[i].CarrierAccountId,
                        SupplierZoneIds: UtilsService.getPropValuesFromArray(ctrl.datasource[i].selectedSuplierZones, "SupplierZoneId"),
                    });
                }

                return suppliersWithZones;
            };
 
            api.load = function (payload) {

                var supplierIds = [];
                if (payload != undefined) {
                    for (var i = 0; i < payload.length; i++) {
                        var obj = payload[i];
                        supplierIds.push(obj.SupplierId);
                    }
                }

                var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();


                carrierAccountReadyPromiseDeferred.promise.then(function () {
                    var carrierAccountPayload = {
                        filter: {},
                        selectedIds: supplierIds
                    };
                    VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, carrierAccountPayload, loadCarrierAccountPromiseDeferred);

                });
                var loadFinalPromiseDeferred = UtilsService.createPromiseDeferred();

                loadCarrierAccountPromiseDeferred.promise.then(function () {
                    loadSupplierZoneDirectives().then(function () {
                        if (loadFinalPromiseDeferred != undefined)
                            loadFinalPromiseDeferred.resolve();
                    }).catch(function (error) {
                        if (loadFinalPromiseDeferred != undefined)
                            loadFinalPromiseDeferred.reject(error);
                    });
                });


                return loadFinalPromiseDeferred.promise;

                function loadSupplierZoneDirectives() {
                    var promises = [];
                    var filterItems;
                    if (payload != undefined) {
                        filterItems = [];
                        for (var i = 0; i < payload.length; i++) {
                            var filterItem = {
                                payload: payload[i],
                                readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            promises.push(filterItem.loadPromiseDeferred.promise);
                            addFilterItemToGrid(filterItem);
                        }
                    }

                    function addFilterItemToGrid(filterItem) {
                        for (var i = 0; i < ctrl.selectedSuppliers.length; i++) {
                            if (filterItem.payload.SupplierId == ctrl.selectedSuppliers[i].CarrierAccountId) {
                                addAPIToDataItem(filterItem, ctrl.selectedSuppliers[i])
                            }
                        }
                    }
                    function addAPIToDataItem(filterItem, selectedSupplier) {

                        var dataItem = {
                            CarrierAccountId: selectedSupplier.CarrierAccountId,
                            name: selectedSupplier.Name
                        };
                        var dataItemPayload = { selectedIds: filterItem.payload.SupplierZoneIds, supplierId: selectedSupplier.CarrierAccountId };

                        dataItem.selectedSuplierZones = [];
                        dataItem.onDirectiveReady = function (api) {
                            dataItem.directiveAPI = api;
                            filterItem.readyPromiseDeferred.resolve();
                        };
                        filterItem.readyPromiseDeferred.promise
                            .then(function () {
                                VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, dataItemPayload, filterItem.loadPromiseDeferred);
                            });

                        ctrl.datasource.push(dataItem);
                    }
                    return UtilsService.waitMultiplePromises(promises);
                }

            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);