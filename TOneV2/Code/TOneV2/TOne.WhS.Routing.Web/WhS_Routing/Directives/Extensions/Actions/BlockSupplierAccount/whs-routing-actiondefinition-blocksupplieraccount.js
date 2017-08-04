'use strict';

app.directive('whsRoutingActiondefinitionBlocksupplieraccount', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockSupplierAccountActionDefinition = new BlockSupplierAccountActionDefinition($scope, ctrl, $attrs);
            blockSupplierAccountActionDefinition.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Routing/Directives/Extensions/Actions/BlockSupplierAccount/Templates/BlockSupplierAccountActionDefinition.html'
    };

    function BlockSupplierAccountActionDefinition($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedDataRecordTypePromiseDeferred;

        var supplierDataRecordTypeFieldsSelectorAPI;
        var supplierDataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onDataRecordTypeChange = function (selectedDataRecordType) {
                if (selectedDataRecordType != undefined) {
                    if (selectedDataRecordTypePromiseDeferred != undefined) {
                        selectedDataRecordTypePromiseDeferred.resolve();
                    }
                    else {
                        var payload = {
                            dataRecordTypeId: selectedDataRecordType.DataRecordTypeId
                        };
                        var setSupplierSelectorLoader = function (value) {
                            $scope.scopeModel.supplierSelectorLoader = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierDataRecordTypeFieldsSelectorAPI, payload, setSupplierSelectorLoader);
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
                    UtilsService.waitMultiplePromises([supplierDataRecordTypeFieldsSelectorReadyDeferred.promise, selectedDataRecordTypePromiseDeferred.promise]).then(function () {
                        selectedDataRecordTypePromiseDeferred = undefined;
                        var dataRecordTypeFieldsSelectorPayload;
                        if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                            dataRecordTypeFieldsSelectorPayload = {
                                selectedIds: payload.Settings.ExtendedSettings.SupplierFieldName,
                                dataRecordTypeId: payload.Settings.ExtendedSettings.DataRecordTypeId
                            }
                        }
                        VRUIUtilsService.callDirectiveLoad(supplierDataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsSelectorPayload, supplierDataRecordTypeFieldsSelectorloadDeferred);
                    });
                    promises.push(supplierDataRecordTypeFieldsSelectorloadDeferred.promise);
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.Routing.MainExtensions.BlockSupplierAccountDefinitionSettings,TOne.WhS.Routing.MainExtensions',
                    DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                    SupplierFieldName: supplierDataRecordTypeFieldsSelectorAPI.getSelectedIds(),
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);