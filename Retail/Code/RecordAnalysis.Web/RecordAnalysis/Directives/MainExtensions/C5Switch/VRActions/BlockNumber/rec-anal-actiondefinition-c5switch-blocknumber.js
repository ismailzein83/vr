'use strict';

app.directive('recAnalActiondefinitionC5switchBlocknumber', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockNumberActionDefinition = new BlockNumberActionDefinition($scope, ctrl, $attrs);
            blockNumberActionDefinition.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/RecordAnalysis/Directives/MainExtensions/C5Switch/VRActions/BlockNumber/Templates/BlockNumberActionDefinition.html"
    };

    function BlockNumberActionDefinition($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedDataRecordTypePromiseDeferred;

        var numberDataRecordTypeFieldsSelectorAPI;
        var numberDataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onNumberDataRecordTypeFieldsSelectorReady = function (api) {
                numberDataRecordTypeFieldsSelectorAPI = api;
                numberDataRecordTypeFieldsSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onDataRecordTypeChange = function (selectedDataRecordType) {
                if (selectedDataRecordType != undefined) {

                    if (selectedDataRecordTypePromiseDeferred != undefined) {
                        selectedDataRecordTypePromiseDeferred.resolve();
                    }
                    else {
                        var numberSelectorPayload = {
                            dataRecordTypeId: selectedDataRecordType.DataRecordTypeId
                        };
                        var setNumberSelectorLoader = function (value) {
                            $scope.scopeModel.numberSelectorLoader = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, numberDataRecordTypeFieldsSelectorAPI, numberSelectorPayload, setNumberSelectorLoader);
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
                    var numberDataRecordTypeFieldsSelectorloadDeferred = UtilsService.createPromiseDeferred();

                    UtilsService.waitMultiplePromises([numberDataRecordTypeFieldsSelectorReadyDeferred.promise, selectedDataRecordTypePromiseDeferred.promise]).then(function () {
                        var numberDataRecordTypeFieldsSelectorPayload;
                        if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                            numberDataRecordTypeFieldsSelectorPayload = {
                                selectedIds: payload.Settings.ExtendedSettings.NumberFieldName,
                                dataRecordTypeId: payload.Settings.ExtendedSettings.DataRecordTypeId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(numberDataRecordTypeFieldsSelectorAPI, numberDataRecordTypeFieldsSelectorPayload, numberDataRecordTypeFieldsSelectorloadDeferred);
                    });
                    promises.push(numberDataRecordTypeFieldsSelectorloadDeferred.promise);

                }

                return UtilsService.waitMultiplePromises(promises).then(function () {
                    selectedDataRecordTypePromiseDeferred = undefined;
                });
            };

            api.getData = function () {
                return {
                    $type: 'RecordAnalysis.MainExtensions.C5Switch.VRActions.BlockNumber.BlockNumberDefinitionSettings, RecordAnalysis.MainExtensions',
                    DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                    NumberFieldName: numberDataRecordTypeFieldsSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);