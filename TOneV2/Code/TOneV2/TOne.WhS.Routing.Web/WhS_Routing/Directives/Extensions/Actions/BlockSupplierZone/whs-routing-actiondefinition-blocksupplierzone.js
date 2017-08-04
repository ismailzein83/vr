'use strict';

app.directive('whsRoutingActiondefinitionBlocksupplierzone', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockSupplierZoneActionDefinition = new BlockSupplierZoneActionDefinition($scope, ctrl, $attrs);
            blockSupplierZoneActionDefinition.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Routing/Directives/Extensions/Actions/BlockSupplierZone/Templates/BlockSupplierZoneActionDefinition.html'
    };

    function BlockSupplierZoneActionDefinition($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedDataRecordTypePromiseDeferred;

        var supplierDataRecordTypeFieldsSelectorAPI;
        var supplierDataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var supplierZoneDataRecordTypeFieldsSelectorAPI;
        var supplierZoneDataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onSupplierDataRecordTypeFieldsSelectorReady = function (api) {
                supplierDataRecordTypeFieldsSelectorAPI = api;
                supplierDataRecordTypeFieldsSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onSupplierZoneDataRecordTypeFieldsSelectorReady = function (api) {
                supplierZoneDataRecordTypeFieldsSelectorAPI = api;
                supplierZoneDataRecordTypeFieldsSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onDataRecordTypeChange = function (selectedDataRecordType) {
                if (selectedDataRecordType != undefined) {

                    if (selectedDataRecordTypePromiseDeferred != undefined) {
                        selectedDataRecordTypePromiseDeferred.resolve();
                    }
                    else {
                        var supplierSelectorPayload = {
                            dataRecordTypeId: selectedDataRecordType.DataRecordTypeId
                        };
                        var setSupplierSelectorLoader = function (value) {
                            $scope.scopeModel.supplierSelectorLoader = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierDataRecordTypeFieldsSelectorAPI, supplierSelectorPayload, setSupplierSelectorLoader);


                        var supplierZoneSelectorPayload = {
                            dataRecordTypeId: selectedDataRecordType.DataRecordTypeId
                        };
                        var setSupplierZoneSelectorLoader = function (value) {
                            $scope.scopeModel.supplierZoneSelectorLoader = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierZoneDataRecordTypeFieldsSelectorAPI, supplierZoneSelectorPayload, setSupplierZoneSelectorLoader);
                    }
                }

            }

            UtilsService.waitMultiplePromises([dataRecordTypeSelectorReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }

        function defineAPI() {

            var api = {};

            api.load = function (payload) {

                var promises = [];

                if (payload != undefined && payload.Settings != undefined) {
                    selectedDataRecordTypePromiseDeferred = UtilsService.createPromiseDeferred();
                }

                function loadDataRecordTypeSelector() {
                    var dataRecordTypeSelectorPayload;
                    if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                        dataRecordTypeSelectorPayload = {
                            selectedIds: payload.Settings.ExtendedSettings.DataRecordTypeId
                        };
                    }
                    return dataRecordTypeSelectorAPI.load(dataRecordTypeSelectorPayload);
                }

                promises.push(loadDataRecordTypeSelector());

                if (selectedDataRecordTypePromiseDeferred != undefined) {
                    var supplierDataRecordTypeFieldsSelectorloadDeferred = UtilsService.createPromiseDeferred();
                    var supplierZoneDataRecordTypeFieldsSelectorloadDeferred = UtilsService.createPromiseDeferred();

                    UtilsService.waitMultiplePromises([supplierDataRecordTypeFieldsSelectorReadyDeferred.promise, selectedDataRecordTypePromiseDeferred.promise]).then(function () {
                        var supplierDataRecordTypeFieldsSelectorPayload;
                        if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                            supplierDataRecordTypeFieldsSelectorPayload = {
                                selectedIds: payload.Settings.ExtendedSettings.SupplierFieldName,
                                dataRecordTypeId: payload.Settings.ExtendedSettings.DataRecordTypeId
                            }
                        }
                        VRUIUtilsService.callDirectiveLoad(supplierDataRecordTypeFieldsSelectorAPI, supplierDataRecordTypeFieldsSelectorPayload, supplierDataRecordTypeFieldsSelectorloadDeferred);
                    });
                    promises.push(supplierDataRecordTypeFieldsSelectorloadDeferred.promise);


                    UtilsService.waitMultiplePromises([supplierZoneDataRecordTypeFieldsSelectorReadyDeferred.promise, selectedDataRecordTypePromiseDeferred.promise]).then(function () {
                        var supplierZoneDataRecordTypeFieldsSelectorPayload;
                        if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                            supplierZoneDataRecordTypeFieldsSelectorPayload = {
                                selectedIds: payload.Settings.ExtendedSettings.SupplierZoneFieldName,
                                dataRecordTypeId: payload.Settings.ExtendedSettings.DataRecordTypeId
                            }
                        }
                        VRUIUtilsService.callDirectiveLoad(supplierZoneDataRecordTypeFieldsSelectorAPI, supplierZoneDataRecordTypeFieldsSelectorPayload, supplierZoneDataRecordTypeFieldsSelectorloadDeferred);
                    });
                    promises.push(supplierZoneDataRecordTypeFieldsSelectorloadDeferred.promise);

                }

                return UtilsService.waitMultiplePromises(promises).then(function () {
                    selectedDataRecordTypePromiseDeferred = undefined;
                });
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.Routing.MainExtensions.BlockSupplierZoneDefinitionSettings,TOne.WhS.Routing.MainExtensions',
                    DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                    SupplierFieldName: supplierDataRecordTypeFieldsSelectorAPI.getSelectedIds(),
                    SupplierZoneFieldName: supplierZoneDataRecordTypeFieldsSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);