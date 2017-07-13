﻿'use strict';

app.directive('vrCommonGeneralTechnicalSettingsEditor', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: "/Client/Modules/Common/Directives/GeneralTechnicalSettings/Templates/GeneralTemplateTechnicalSettings.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {
            var gaSettingsAPI;
            var gaSettingsReadyDeferred = UtilsService.createPromiseDeferred();          
            $scope.scopeModel = {};
            $scope.scopeModel.onGaSettingsReady = function (api) {
                gaSettingsAPI = api;
                gaSettingsReadyDeferred.resolve();
            };
           
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var gaSettingsLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(gaSettingsLoadDeferred.promise);

                    gaSettingsReadyDeferred.promise.then(function () {
                        var gapayload = {
                            data: payload != undefined && payload.data != undefined ? payload.data.GAData : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(gaSettingsAPI, gapayload, gaSettingsLoadDeferred);
                    });
                    


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Entities.GeneralTechnicalSettingData, Vanrise.Entities",
                        GAData: gaSettingsAPI.getData()
                    };
                };
                
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);
