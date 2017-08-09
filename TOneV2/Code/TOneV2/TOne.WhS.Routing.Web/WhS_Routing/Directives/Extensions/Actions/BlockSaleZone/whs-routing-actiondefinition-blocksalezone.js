'use strict';

app.directive('whsRoutingActiondefinitionBlocksalezone', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockSaleZoneActionDefinition = new BlockSaleZoneActionDefinition($scope, ctrl, $attrs);
            blockSaleZoneActionDefinition.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Routing/Directives/Extensions/Actions/BlockSaleZone/Templates/BlockSaleZoneActionDefinition.html'
    };

    function BlockSaleZoneActionDefinition($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedDataRecordTypePromiseDeferred;

        var customerDataRecordTypeFieldsSelectorAPI;
        var customerDataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDataRecordTypeFieldsSelectorAPI;
        var saleZoneDataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onCustomerDataRecordTypeFieldsSelectorReady = function (api) {
                customerDataRecordTypeFieldsSelectorAPI = api;
                customerDataRecordTypeFieldsSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onSaleZoneDataRecordTypeFieldsSelectorReady = function (api) {
                saleZoneDataRecordTypeFieldsSelectorAPI = api;
                saleZoneDataRecordTypeFieldsSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onDataRecordTypeChange = function (selectedDataRecordType) {
                if (selectedDataRecordType != undefined) {
                    if (selectedDataRecordTypePromiseDeferred != undefined) {
                        selectedDataRecordTypePromiseDeferred.resolve();
                    }
                    else {

                        var customerSelectorPayload = {
                            dataRecordTypeId: selectedDataRecordType.DataRecordTypeId
                        };
                        var setCustomerSelectorLoader = function (value) {
                            $scope.scopeModel.customerSelectorLoader = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, customerDataRecordTypeFieldsSelectorAPI, customerSelectorPayload, setCustomerSelectorLoader);


                        var saleZoneSelectorPayload = {
                            dataRecordTypeId: selectedDataRecordType.DataRecordTypeId
                        };
                        var setSaleZoneSelectorLoader = function (value) {
                            $scope.scopeModel.saleZoneSelectorLoader = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDataRecordTypeFieldsSelectorAPI, saleZoneSelectorPayload, setSaleZoneSelectorLoader);
                    }
                }

            };

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

                    var customerDataRecordTypeFieldsSelectorloadDeferred = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultiplePromises([customerDataRecordTypeFieldsSelectorReadyDeferred.promise, selectedDataRecordTypePromiseDeferred.promise]).then(function () {
                        var customerDataRecordTypeFieldsSelectorPayload;
                        if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                            customerDataRecordTypeFieldsSelectorPayload = {
                                selectedIds: payload.Settings.ExtendedSettings.CustomerFieldName,
                                dataRecordTypeId: payload.Settings.ExtendedSettings.DataRecordTypeId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(customerDataRecordTypeFieldsSelectorAPI, customerDataRecordTypeFieldsSelectorPayload, customerDataRecordTypeFieldsSelectorloadDeferred);
                    });
                    promises.push(customerDataRecordTypeFieldsSelectorloadDeferred.promise);


                    var saleZoneDataRecordTypeFieldsSelectorloadDeferred = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultiplePromises([saleZoneDataRecordTypeFieldsSelectorReadyDeferred.promise, selectedDataRecordTypePromiseDeferred.promise]).then(function () {
                        var saleZoneDataRecordTypeFieldsSelectorPayload;
                        if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                            saleZoneDataRecordTypeFieldsSelectorPayload = {
                                selectedIds: payload.Settings.ExtendedSettings.SaleZoneFieldName,
                                dataRecordTypeId: payload.Settings.ExtendedSettings.DataRecordTypeId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(saleZoneDataRecordTypeFieldsSelectorAPI, saleZoneDataRecordTypeFieldsSelectorPayload, saleZoneDataRecordTypeFieldsSelectorloadDeferred);
                    });
                    promises.push(saleZoneDataRecordTypeFieldsSelectorloadDeferred.promise);

                }

                return UtilsService.waitMultiplePromises(promises).then(function () {
                    selectedDataRecordTypePromiseDeferred = undefined;
                });
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.Routing.MainExtensions.BlockSaleZoneDefinitionSettings,TOne.WhS.Routing.MainExtensions',
                    DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                    CustomerFieldName: customerDataRecordTypeFieldsSelectorAPI.getSelectedIds(),
                    SaleZoneFieldName: saleZoneDataRecordTypeFieldsSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);