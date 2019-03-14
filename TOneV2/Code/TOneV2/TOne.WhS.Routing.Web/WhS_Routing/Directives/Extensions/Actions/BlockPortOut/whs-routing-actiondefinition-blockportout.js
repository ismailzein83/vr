'use strict';

app.directive('whsRoutingActiondefinitionBlockportout', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockPortOutActionDefinition = new BlockPortOutActionDefinition($scope, ctrl, $attrs);
            blockPortOutActionDefinition.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Routing/Directives/Extensions/Actions/BlockPortOut/Templates/BlockPortOutActionDefinition.html"
    };

    function BlockPortOutActionDefinition($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedDataRecordTypePromiseDeferred;

        var switchDataRecordTypeFieldsSelectorAPI;
        var switchDataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var portOutDataRecordTypeFieldsSelectorAPI;
        var portOutDataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onPortOutDataRecordTypeFieldsSelectorReady = function (api) {
                portOutDataRecordTypeFieldsSelectorAPI = api;
                portOutDataRecordTypeFieldsSelectorReadyDeferred.resolve();
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


                        var portOutSelectorPayload = {
                            dataRecordTypeId: selectedDataRecordType.DataRecordTypeId
                        };
                        var setPortOutSelectorLoader = function (value) {
                            $scope.scopeModel.portOutSelectorLoader = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, portOutDataRecordTypeFieldsSelectorAPI, portOutSelectorPayload, setPortOutSelectorLoader);
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
                    var portOutDataRecordTypeFieldsSelectorloadDeferred = UtilsService.createPromiseDeferred();

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


                    UtilsService.waitMultiplePromises([portOutDataRecordTypeFieldsSelectorReadyDeferred.promise, selectedDataRecordTypePromiseDeferred.promise]).then(function () {
                        var portOutDataRecordTypeFieldsSelectorPayload;
                        if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                            portOutDataRecordTypeFieldsSelectorPayload = {
                                selectedIds: payload.Settings.ExtendedSettings.PortOutFieldName,
                                dataRecordTypeId: payload.Settings.ExtendedSettings.DataRecordTypeId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(portOutDataRecordTypeFieldsSelectorAPI, portOutDataRecordTypeFieldsSelectorPayload, portOutDataRecordTypeFieldsSelectorloadDeferred);
                    });
                    promises.push(portOutDataRecordTypeFieldsSelectorloadDeferred.promise);

                }

                return UtilsService.waitMultiplePromises(promises).then(function () {
                    selectedDataRecordTypePromiseDeferred = undefined;
                });
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.Routing.MainExtensions.VRActions.BlockPortOutDefinitionSettings, TOne.WhS.Routing.MainExtensions',
                    DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                    SwitchFieldName: switchDataRecordTypeFieldsSelectorAPI.getSelectedIds(),
                    PortOutFieldName: portOutDataRecordTypeFieldsSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);