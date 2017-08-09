'use strict';

app.directive('whsRoutingActiondefinitionBlockcustomeraccount', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockCustomerAccountActionDefinition = new BlockCustomerAccountActionDefinition($scope, ctrl, $attrs);
            blockCustomerAccountActionDefinition.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Routing/Directives/Extensions/Actions/BlockCustomeAccount/Templates/BlockCustomerAccountActionDefinition.html'
    };

    function BlockCustomerAccountActionDefinition($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedDataRecordTypePromiseDeferred;

        var customerDataRecordTypeFieldsSelectorAPI;
        var customerDataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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
                        selectedDataRecordTypePromiseDeferred = undefined;
                        var dataRecordTypeFieldsSelectorPayload;
                        if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                            dataRecordTypeFieldsSelectorPayload = {
                                selectedIds: payload.Settings.ExtendedSettings.CustomerFieldName,
                                dataRecordTypeId: payload.Settings.ExtendedSettings.DataRecordTypeId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(customerDataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsSelectorPayload, customerDataRecordTypeFieldsSelectorloadDeferred);
                    });
                    promises.push(customerDataRecordTypeFieldsSelectorloadDeferred.promise);
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.Routing.MainExtensions.BlockCustomerAccountDefinitionSettings,TOne.WhS.Routing.MainExtensions',
                    DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                    CustomerFieldName: customerDataRecordTypeFieldsSelectorAPI.getSelectedIds(),
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);