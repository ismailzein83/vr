﻿(function (app) {

    'use strict';

    EricssonSWSyncSettingsEditor.$inject = ["UtilsService"];

    function EricssonSWSyncSettingsEditor(UtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new EricssonSWSyncSettingsEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/Templates/EricssonRouteSyncSetting.html"
        };

        function EricssonSWSyncSettingsEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.faultCodes = [];
                $scope.scopeModel.faultCodeValue = undefined;
                $scope.scopeModel.disableAddButton = true;
                $scope.scopeModel.numberOfRetries = 1;

                $scope.scopeModel.onFaultCodeValueChange = function (value) {

                    if (value == undefined) {
                        $scope.scopeModel.disableAddButton = true;
                        return;
                    }

                    if ($scope.scopeModel.faultCodes == undefined || $scope.scopeModel.faultCodes.length == 0) {
                        $scope.scopeModel.disableAddButton = false;
                        return;
                    }

                    for (var i = 0; i < $scope.scopeModel.faultCodes.length; i++) {
                        if ($scope.scopeModel.faultCodes[i].faultCode == $scope.scopeModel.faultCodeValue) {
                            $scope.scopeModel.disableAddButton = true;
                            return;
                        }
                    }
                    $scope.scopeModel.disableAddButton = false;
                };

                $scope.scopeModel.addFaultCodeValue = function () {

                    $scope.scopeModel.faultCodes.push({
                        faultCode: $scope.scopeModel.faultCodeValue
                    });
                    $scope.scopeModel.faultCodeValue = undefined;
                    $scope.scopeModel.disableAddButton = true;
                };

                $scope.scopeModel.validateFaultCodes = function () {
                    if ($scope.scopeModel.faultCodes == undefined || $scope.scopeModel.faultCodes.length == 0)
                        return 'Please, add fault codes';
                    return null;
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        $scope.scopeModel.numberOfRetries = payload.settings.NumberOfRetries;
                        if (payload.settings.FaultCodes != undefined) {
                            $scope.scopeModel.faultCodes = payload.settings.FaultCodes;

                        }
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "TOne.WhS.RouteSync.Ericsson.EricssonSwitchRouteSynchronizerSettings, TOne.WhS.RouteSync.Ericsson",
                        NumberOfRetries: $scope.scopeModel.numberOfRetries,
                        FaultCodes: $scope.scopeModel.faultCodes,
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncEricssonSettingseditor', EricssonSWSyncSettingsEditor);
})(app);