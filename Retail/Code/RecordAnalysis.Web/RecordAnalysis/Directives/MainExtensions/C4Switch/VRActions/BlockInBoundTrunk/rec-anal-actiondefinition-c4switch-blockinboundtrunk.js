'use strict';

app.directive('recAnalActiondefinitionC4switchBlockinboundtrunk', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockInTrunkActionDefinition = new BlockInTrunkActionDefinition($scope, ctrl, $attrs);
            blockInTrunkActionDefinition.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/RecordAnalysis/Directives/MainExtensions/C4Switch/VRActions/BlockInBoundTrunk/Templates/BlockInBoundTrunkActionDefinition.html"
    };

    function BlockInTrunkActionDefinition($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedDataRecordTypePromiseDeferred;

        var switchDataRecordTypeFieldsSelectorAPI;
        var switchDataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var inTrunkDataRecordTypeFieldsSelectorAPI;
        var inTrunkDataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onSwitchDataRecordTypeFieldsSelectorReady = function (api) {
                switchDataRecordTypeFieldsSelectorAPI = api;
                switchDataRecordTypeFieldsSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onInTrunkDataRecordTypeFieldsSelectorReady = function (api) {
                inTrunkDataRecordTypeFieldsSelectorAPI = api;
                inTrunkDataRecordTypeFieldsSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onDataRecordTypeChange = function (selectedDataRecordType) {
                if (selectedDataRecordType != undefined) {

                    if (selectedDataRecordTypePromiseDeferred != undefined) {
                        selectedDataRecordTypePromiseDeferred.resolve();
                    }
                    else {
                        var switchSelectorPayload = {
                            dataRecordTypeId: selectedDataRecordType.DataRecordTypeId
                        };
                        var setSwitchSelectorLoader = function (value) {
                            $scope.scopeModel.switchSelectorLoader = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, switchDataRecordTypeFieldsSelectorAPI, switchSelectorPayload, setSwitchSelectorLoader);


                        var inTrunkSelectorPayload = {
                            dataRecordTypeId: selectedDataRecordType.DataRecordTypeId
                        };
                        var setInTrunkSelectorLoader = function (value) {
                            $scope.scopeModel.inTrunkSelectorLoader = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, inTrunkDataRecordTypeFieldsSelectorAPI, inTrunkSelectorPayload, setInTrunkSelectorLoader);
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
                    var switchDataRecordTypeFieldsSelectorloadDeferred = UtilsService.createPromiseDeferred();
                    var inTrunkDataRecordTypeFieldsSelectorloadDeferred = UtilsService.createPromiseDeferred();

                    UtilsService.waitMultiplePromises([switchDataRecordTypeFieldsSelectorReadyDeferred.promise, selectedDataRecordTypePromiseDeferred.promise]).then(function () {
                        var switchDataRecordTypeFieldsSelectorPayload;
                        if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                            switchDataRecordTypeFieldsSelectorPayload = {
                                selectedIds: payload.Settings.ExtendedSettings.SwitchFieldName,
                                dataRecordTypeId: payload.Settings.ExtendedSettings.DataRecordTypeId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(switchDataRecordTypeFieldsSelectorAPI, switchDataRecordTypeFieldsSelectorPayload, switchDataRecordTypeFieldsSelectorloadDeferred);
                    });
                    promises.push(switchDataRecordTypeFieldsSelectorloadDeferred.promise);


                    UtilsService.waitMultiplePromises([inTrunkDataRecordTypeFieldsSelectorReadyDeferred.promise, selectedDataRecordTypePromiseDeferred.promise]).then(function () {
                        var inTrunkDataRecordTypeFieldsSelectorPayload;
                        if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                            inTrunkDataRecordTypeFieldsSelectorPayload = {
                                selectedIds: payload.Settings.ExtendedSettings.InTrunkFieldName,
                                dataRecordTypeId: payload.Settings.ExtendedSettings.DataRecordTypeId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(inTrunkDataRecordTypeFieldsSelectorAPI, inTrunkDataRecordTypeFieldsSelectorPayload, inTrunkDataRecordTypeFieldsSelectorloadDeferred);
                    });
                    promises.push(inTrunkDataRecordTypeFieldsSelectorloadDeferred.promise);

                }

                return UtilsService.waitMultiplePromises(promises).then(function () {
                    selectedDataRecordTypePromiseDeferred = undefined;
                });
            };

            api.getData = function () {
                return {
                    $type: 'RecordAnalysis.MainExtensions.C4Switch.VRActions.BlockInBoundTrunk.BlockInBoundTrunkDefinitionSettings, RecordAnalysis.MainExtensions',
                    DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                    SwitchFieldName: switchDataRecordTypeFieldsSelectorAPI.getSelectedIds(),
                    InTrunkFieldName: inTrunkDataRecordTypeFieldsSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);