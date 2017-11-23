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
            var companyContactSettingsAPI;
            var companyContactSettingsReadyDeferred = UtilsService.createPromiseDeferred();
            var companyDefinitionSettingsAPI;
            var companyDefinitionSettingsReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
               
                $scope.scopeModel.onCompanyContactSettingsReady = function (api) {
                    companyContactSettingsAPI = api;
                    companyContactSettingsReadyDeferred.resolve();
                };

                $scope.scopeModel.onCompanyDefinitionSettingsReady = function (api) {
                    companyDefinitionSettingsAPI = api;
                    companyDefinitionSettingsReadyDeferred.resolve();
                };
                
                UtilsService.waitMultiplePromises([gaSettingsReadyDeferred.promise, companyContactSettingsReadyDeferred.promise, companyDefinitionSettingsReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var companySettingDefinition = payload != undefined && payload.data != undefined ? payload.data.CompanySettingDefinition : undefined;

                    function loadCompanyContactSettings() {
                        var companyContactPayload = {
                            data: companySettingDefinition
                        };
                        return companyContactSettingsAPI.load(companyContactPayload);
                    }

                    function loadCompanyDefinitionSettings() {
                        var extendedSettingsObj;
                        if (companySettingDefinition != undefined)
                            extendedSettingsObj = companySettingDefinition.ExtendedSettings;
                        var companyDefinitionPayload = {
                            extendedSettings: extendedSettingsObj
                        };
                        return companyDefinitionSettingsAPI.load(companyDefinitionPayload);
                    }

                    promises.push(loadCompanyContactSettings());
                    promises.push(loadCompanyDefinitionSettings());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Entities.GeneralTechnicalSettingData, Vanrise.Entities",
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
