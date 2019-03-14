'use strict';

app.directive('whsRoutingActiondefinitionBlockportin', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockPortInActionDefinition = new BlockPortInActionDefinition($scope, ctrl, $attrs);
            blockPortInActionDefinition.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Routing/Directives/Extensions/Actions/BlockPortIn/Templates/BlockPortInActionDefinition.html"
    };

    function BlockPortInActionDefinition($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedDataRecordTypePromiseDeferred;

        var switchDataRecordTypeFieldsSelectorAPI;
        var switchDataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var portInDataRecordTypeFieldsSelectorAPI;
        var portInDataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onPortInDataRecordTypeFieldsSelectorReady = function (api) {
                portInDataRecordTypeFieldsSelectorAPI = api;
                portInDataRecordTypeFieldsSelectorReadyDeferred.resolve();
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


                        var portInSelectorPayload = {
                            dataRecordTypeId: selectedDataRecordType.DataRecordTypeId
                        };
                        var setPortInSelectorLoader = function (value) {
                            $scope.scopeModel.portInSelectorLoader = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, portInDataRecordTypeFieldsSelectorAPI, portInSelectorPayload, setPortInSelectorLoader);
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
                    var portInDataRecordTypeFieldsSelectorloadDeferred = UtilsService.createPromiseDeferred();

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


                    UtilsService.waitMultiplePromises([portInDataRecordTypeFieldsSelectorReadyDeferred.promise, selectedDataRecordTypePromiseDeferred.promise]).then(function () {
                        var portInDataRecordTypeFieldsSelectorPayload;
                        if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                            portInDataRecordTypeFieldsSelectorPayload = {
                                selectedIds: payload.Settings.ExtendedSettings.PortInFieldName,
                                dataRecordTypeId: payload.Settings.ExtendedSettings.DataRecordTypeId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(portInDataRecordTypeFieldsSelectorAPI, portInDataRecordTypeFieldsSelectorPayload, portInDataRecordTypeFieldsSelectorloadDeferred);
                    });
                    promises.push(portInDataRecordTypeFieldsSelectorloadDeferred.promise);

                }

                return UtilsService.waitMultiplePromises(promises).then(function () {
                    selectedDataRecordTypePromiseDeferred = undefined;
                });
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.Routing.MainExtensions.VRActions.BlockPortInDefinitionSettings, TOne.WhS.Routing.MainExtensions',
                    DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                    SwitchFieldName: switchDataRecordTypeFieldsSelectorAPI.getSelectedIds(),
                    PortInFieldName: portInDataRecordTypeFieldsSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);