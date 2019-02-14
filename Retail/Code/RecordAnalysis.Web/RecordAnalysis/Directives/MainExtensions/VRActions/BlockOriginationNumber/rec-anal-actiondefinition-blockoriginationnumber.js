'use strict';

app.directive('recAnalActiondefinitionBlockoriginationnumber', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockOriginationNumberActionDefinition = new BlockOriginationNumberActionDefinition($scope, ctrl, $attrs);
            blockOriginationNumberActionDefinition.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/RecordAnalysis/Directives/MainExtensions/VRActions/BlockOriginationNumber/Templates/BlockOriginationNumberActionDefinition.html"
    };

    function BlockOriginationNumberActionDefinition($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedDataRecordTypePromiseDeferred;

        var originationNumberDataRecordTypeFieldsSelectorAPI;
        var originationNumberDataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onOriginationNumberDataRecordTypeFieldsSelectorReady = function (api) {
                originationNumberDataRecordTypeFieldsSelectorAPI = api;
                originationNumberDataRecordTypeFieldsSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onDataRecordTypeChange = function (selectedDataRecordType) {
                if (selectedDataRecordType != undefined) {

                    if (selectedDataRecordTypePromiseDeferred != undefined) {
                        selectedDataRecordTypePromiseDeferred.resolve();
                    }
                    else {
                        var originationNumberSelectorPayload = {
                            dataRecordTypeId: selectedDataRecordType.DataRecordTypeId
                        };
                        var setOriginationNumberSelectorLoader = function (value) {
                            $scope.scopeModel.originationNumberSelectorLoader = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, originationNumberDataRecordTypeFieldsSelectorAPI, originationNumberSelectorPayload, setOriginationNumberSelectorLoader);
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
                    var originationNumberDataRecordTypeFieldsSelectorloadDeferred = UtilsService.createPromiseDeferred();

                    UtilsService.waitMultiplePromises([originationNumberDataRecordTypeFieldsSelectorReadyDeferred.promise, selectedDataRecordTypePromiseDeferred.promise]).then(function () {
                        var originationNumberDataRecordTypeFieldsSelectorPayload;
                        if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                            originationNumberDataRecordTypeFieldsSelectorPayload = {
                                selectedIds: payload.Settings.ExtendedSettings.OriginationNumberFieldName,
                                dataRecordTypeId: payload.Settings.ExtendedSettings.DataRecordTypeId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(originationNumberDataRecordTypeFieldsSelectorAPI, originationNumberDataRecordTypeFieldsSelectorPayload, originationNumberDataRecordTypeFieldsSelectorloadDeferred);
                    });
                    promises.push(originationNumberDataRecordTypeFieldsSelectorloadDeferred.promise);
                }

                return UtilsService.waitMultiplePromises(promises).then(function () {
                    selectedDataRecordTypePromiseDeferred = undefined;
                });
            };

            api.getData = function () {
                return {
                    $type: 'RecordAnalysis.MainExtensions.VRActions.BlockOriginationNumber.BlockOriginationNumberDefinitionSettings, RecordAnalysis.MainExtensions',
                    DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                    OriginationNumberFieldName: originationNumberDataRecordTypeFieldsSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);