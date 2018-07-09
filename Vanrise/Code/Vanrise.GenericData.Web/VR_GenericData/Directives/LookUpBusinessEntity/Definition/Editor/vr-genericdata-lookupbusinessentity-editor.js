"use strict";

app.directive("vrGenericdataLookupbusinessentityEditor", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new LKUPBEBusinessEntityDefinitionEditor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/LookUpBusinessEntity/Definition/Editor/Templates/LookUpBusinessEntityDefinitionEditor.html"

        };

        function LKUPBEBusinessEntityDefinitionEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var extendedSettingsAPI;
            var extendedSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var securityAPI;
            var securityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var context;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.selectorSingularTitle;

                $scope.scopeModel.selectorPluralTitle;

                $scope.scopeModel.onLookUpBEEditorExtendedSettingsReady = function (api) {
                    extendedSettingsAPI = api;
                    extendedSettingsReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onLookUpBESecurityReady = function (api) {
                    securityAPI = api;
                    securityReadyPromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([extendedSettingsReadyPromiseDeferred.promise, securityReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Business.LKUPBusinessEntityDefinitionSettings, Vanrise.GenericData.Business",
                        SelectorSingularTitle: $scope.scopeModel.selectorSingularTitle,
                        SelectorPluralTitle: $scope.scopeModel.selectorPluralTitle,
                        ExtendedSettings: extendedSettingsAPI.getData(),
                        Security: securityAPI.getData()
                    };
                };

                api.load = function (payload) {
                    var businessEntityDefinitionSettings;
                    var promises = [];
                    if (payload != undefined) {
                        businessEntityDefinitionSettings = payload.businessEntityDefinitionSettings;

                        if (businessEntityDefinitionSettings != undefined) {
                            $scope.scopeModel.selectorSingularTitle = businessEntityDefinitionSettings.SelectorSingularTitle;
                            $scope.scopeModel.selectorPluralTitle = businessEntityDefinitionSettings.SelectorPluralTitle;
                        }
                    }
                    promises.push(loadExtendedSettingsEditor());
                    promises.push(loadSecurityDirective());

                    function loadExtendedSettingsEditor() {
                        var loadExtendedSettingsEditorPromiseDeferred = UtilsService.createPromiseDeferred();
                        extendedSettingsReadyPromiseDeferred.promise.then(function () {
                            var settingPayload = {
                                context: getContext(),
                                settings: businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.ExtendedSettings || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(extendedSettingsAPI, settingPayload, loadExtendedSettingsEditorPromiseDeferred);
                        });
                        return loadExtendedSettingsEditorPromiseDeferred.promise;
                    }

                    function loadSecurityDirective() {
                        var loadSecurityDPromiseDeferred = UtilsService.createPromiseDeferred();
                        securityReadyPromiseDeferred.promise.then(function () {
                            var securityPayload;
                            if (businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.Security != undefined)
                                securityPayload = businessEntityDefinitionSettings.Security;

                            VRUIUtilsService.callDirectiveLoad(securityAPI, securityPayload, loadSecurityDPromiseDeferred);
                        });
                        return loadSecurityDPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

            }
            function getContext() {
                var currentContext = context;
                if(currentContext==undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);