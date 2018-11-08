'use strict';

app.directive('cpWhsSupplierzonesDirective', ['UtilsService', 'VRUIUtilsService','CP_WhS_SupplierZonesAPIService',
    function (UtilsService, VRUIUtilsService, CP_WhS_SupplierZonesAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];
                var ctor = new WhSSupplierZonesDirectiveCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/CP_WhS/Elements/SupplierZones/Directives/Templates/WhSSupplierZonesDirectiveTemplate.html'
        };

        function WhSSupplierZonesDirectiveCtor(ctrl, $scope, $attrs) {

            var suppliersSelectorAPI;
            var suppliersSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierZonesSelectorAPI;
            var supplierZonesSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                ctrl.datasource = [];

                $scope.scopeModel = {};
                $scope.scopeModel.isSingleItem = false;

                $scope.scopeModel.onSuppliersSelectorReady = function (api) {
                    suppliersSelectorAPI = api;
                    suppliersSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSupplierZonesSelectorReady = function (api) {
                    supplierZonesSelectorAPI = api;
                    supplierZonesSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSupplierSelectionChanged = function (value) {
                    if (value != undefined) {
                        supplierZonesSelectorReadyPromiseDeferred.promise.then(function () {
                            var setLoader = function (value) {
                            };
                            var payload = {
                                supplierId: value.AccountId
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierZonesSelectorAPI, payload, setLoader, undefined);
                        });
                    }
                    ctrl.selectedvalues.length = 0;
                };
                UtilsService.waitMultiplePromises([suppliersSelectorReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};
                var selectedIds; 
                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined)
                        selectedIds = payload.selectedIds;
                    if (selectedIds != undefined) {
                        var selectedSupplierZoneIds = [];

                        if ($attrs.ismultipleselection != undefined)
                            selectedSupplierZoneIds = selectedIds;
                        else
                            selectedSupplierZoneIds.push(selectedIds);

                        var loadSuppliersSectionPromiseDeferred = UtilsService.createPromiseDeferred();
                        suppliersSelectorReadyPromiseDeferred.promise.then(function () {

                            var editPromises = [];

                            var loadSupplierPromiseDeferred = UtilsService.createPromiseDeferred();
                            editPromises.push(loadSupplierPromiseDeferred.promise);

                            var setSelectedSupplierZonesPromiseDeferred = UtilsService.createPromiseDeferred();
                            editPromises.push(setSelectedSupplierZonesPromiseDeferred.promise);

                            var loadSupplierZonesPromise = CP_WhS_SupplierZonesAPIService.GetSupplierIdBySupplierZoneIds(selectedSupplierZoneIds).then(function (response) {

                                var selectedSupplierId = response;

                                var supplierSelectorPayload = {
                                    selectedIds: selectedSupplierId
                                };

                                VRUIUtilsService.callDirectiveLoad(suppliersSelectorAPI, supplierSelectorPayload, loadSupplierPromiseDeferred);

                                loadSupplierPromiseDeferred.promise.then(function () {
                                    CP_WhS_SupplierZonesAPIService.GetSupplierZoneInfoByIds(UtilsService.serializetoJson(selectedIds)).then(function (response) {
                                        angular.forEach(response, function (item) {
                                            ctrl.datasource.push(item);
                                        });
                                        VRUIUtilsService.setSelectedValues(selectedIds, 'SupplierZoneId', $attrs, ctrl);
                                        setSelectedSupplierZonesPromiseDeferred.resolve();
                                    });
                                });

                            });

                            editPromises.push(loadSupplierZonesPromise);

                            UtilsService.waitMultiplePromises(editPromises).then(function () {
                                loadSuppliersSectionPromiseDeferred.resolve();
                            }).catch(function (error) {
                                loadSuppliersSectionPromiseDeferred.reject(error);
                            });

                        });

                        return loadSuppliersSectionPromiseDeferred.promise;
                    }
                    else {
                        promises.push(loadSuppliersSelector());
                    }
                    function loadSuppliersSelector() {
                        var loadSupplierSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        suppliersSelectorReadyPromiseDeferred.promise.then(function () {
                            var payload = {
                                businessEntityDefinitionId: "574ef14e-64d3-4f56-8d0a-8ec05f043b7b"
                            };
                            VRUIUtilsService.callDirectiveLoad(suppliersSelectorAPI, payload, loadSupplierSelectorPromiseDeferred);
                        });
                        loadSupplierSelectorPromiseDeferred.promise.then(function () {
                            if (suppliersSelectorAPI.isSingleItem()) {
                                suppliersSelectorAPI.selectIfSingleItem();
                                $scope.scopeModel.isSingleItem = true;
                                $scope.scopeModel.selectedSupplier = suppliersSelectorAPI.getSingleItem();
                            }
                        });
                        return loadSupplierSelectorPromiseDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getSelectedIds = function () {
                    return supplierZonesSelectorAPI.getSelectedIds();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);