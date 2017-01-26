'use strict';

app.directive('vrCommonGeneralSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/GeneralSettings/Templates/GeneralTemplateSettings.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {
            var uiSettingsAPI;
            var uiSettingsReadyDeferred = UtilsService.createPromiseDeferred();
            $scope.scopeModel = {};
            $scope.scopeModel.onUISettingsReady = function (api) {
                uiSettingsAPI = api;
                uiSettingsReadyDeferred.resolve();
            };
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var uiSettingsLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(uiSettingsLoadDeferred.promise);

                    uiSettingsReadyDeferred.promise.then(function () {
                        var uipayload = {
                            data: payload != undefined && payload.data != undefined ? payload.data.UIData : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(uiSettingsAPI, uipayload, uiSettingsLoadDeferred);
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Entities.GeneralSettingData, Vanrise.Entities",
                        UIData: uiSettingsAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);
