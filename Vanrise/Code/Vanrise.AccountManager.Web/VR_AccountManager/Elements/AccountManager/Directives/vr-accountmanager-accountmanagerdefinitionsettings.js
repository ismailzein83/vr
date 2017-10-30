﻿(function (app) {

    'use strict';

    accountmanagerdefinitionsSelectiveDirective.$inject = ['VR_AccountManager_AccountManagerAPIService', 'UtilsService', 'VRUIUtilsService'];

    function accountmanagerdefinitionsSelectiveDirective(VR_AccountManager_AccountManagerAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var accountmanagerdefinitionsSelective = new AccountmanagerdefinitionsSelective($scope, ctrl, $attrs);
                accountmanagerdefinitionsSelective.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_AccountManager/Elements/AccountManager/Directives/Template/AccountManagerDefinitionSettings.html",

        };


        function AccountmanagerdefinitionsSelective($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    var extendedSettings;
                    var promises = [];
                    if (payload != undefined) {
                        extendedSettings = payload.extendedSettings;
                    }
                    if (extendedSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }
                    var getAccountManagerDefinitionConfigsPromise = getAccountManagerDefinitionConfigs();
                    promises.push(getAccountManagerDefinitionConfigsPromise);

                    function getAccountManagerDefinitionConfigs() {
                        return VR_AccountManager_AccountManagerAPIService.GetAccountManagerDefinitionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (extendedSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, extendedSettings.ConfigId, 'ExtensionConfigurationId');
                                }

                            }
                        });
                    }

                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = {
                                extendedSettings: extendedSettings
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);

                };
                
                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                }

            }

        }
    }

    app.directive('vrAccountmanagerAccountmanagerdefinitionsettings', accountmanagerdefinitionsSelectiveDirective);

})(app);