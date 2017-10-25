'use strict';

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
            var companyContactSettingsAPI;
            var companyContactSettingsReadyDeferred = UtilsService.createPromiseDeferred();
            var companyDefinitionSettingsAPI;
            var companyDefinitionSettingsReadyDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};
            $scope.scopeModel.onGaSettingsReady = function (api) {
                gaSettingsAPI = api;
                gaSettingsReadyDeferred.resolve();
            };
            $scope.scopeModel.onCompanyContactSettingsReady = function (api) {
                companyContactSettingsAPI = api;
                companyContactSettingsReadyDeferred.resolve();
            };
            $scope.scopeModel.onCompanyDefinitionSettingsReady = function (api) {
                companyDefinitionSettingsAPI = api;
                companyDefinitionSettingsReadyDeferred.resolve();
            };

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var companySettingDefinition = payload != undefined && payload.data != undefined ? payload.data.CompanySettingDefinition : undefined;

                    var gaSettingsLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(gaSettingsLoadDeferred.promise);

                    gaSettingsReadyDeferred.promise.then(function () {
                        var gapayload = {
                            data: payload != undefined && payload.data != undefined ? payload.data.GAData : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(gaSettingsAPI, gapayload, gaSettingsLoadDeferred);
                    });


                    var companyContactSettingsLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(companyContactSettingsLoadDeferred.promise);

                    companyContactSettingsReadyDeferred.promise.then(function () {
                        var companyContactpayload = {
                            data: payload != undefined && payload.data != undefined ? payload.data.CompanySettingDefinition : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(companyContactSettingsAPI, companyContactpayload, companyContactSettingsLoadDeferred);
                    });

                    var companyDefinitionSettingsLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(companyDefinitionSettingsLoadDeferred.promise);

                    companyDefinitionSettingsReadyDeferred.promise.then(function () {
                        var companyDefinitionpayload = {
                            extendedSettings: companySettingDefinition != undefined ? companySettingDefinition.ExtendedSettings : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(companyDefinitionSettingsAPI, companyDefinitionpayload, companyDefinitionSettingsLoadDeferred);
                    });

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Entities.GeneralTechnicalSettingData, Vanrise.Entities",
                        GAData: gaSettingsAPI.getData(),
                        CompanySettingDefinition: {
                            ContactTypes: companyContactSettingsAPI.getData().ContactTypes,
                            ExtendedSettings: companyDefinitionSettingsAPI.getData(),
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);
