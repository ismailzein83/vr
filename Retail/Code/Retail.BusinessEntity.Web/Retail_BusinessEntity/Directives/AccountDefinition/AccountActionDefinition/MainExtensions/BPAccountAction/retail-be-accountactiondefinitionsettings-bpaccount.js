'use strict';

app.directive('retailBeAccountactiondefinitionsettingsBpaccount', ['UtilsService','VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BPAccountActionDefinitionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/MainExtensions/BPAccountAction/Templates/BPAccountActionSettingsTemplate.html'
        };

        function BPAccountActionDefinitionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var bPDefinitionSettingsApi;
            var bPDefinitionSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};


                $scope.scopeModel.onBPAccountActionSelectiveReady = function (api) {
                    bPDefinitionSettingsApi = api;
                    bPDefinitionSettingsPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var accountActionDefinitionSettings;
                    if (payload != undefined)
                    {
                        accountActionDefinitionSettings = payload.accountActionDefinitionSettings;
                    }
                    var promises = [];


                    promises.push(loadBusinessEntityDefinitionSelector());

                    function loadBusinessEntityDefinitionSelector() {
                        var bPDefinitionSettingsLoadDeferred = UtilsService.createPromiseDeferred();

                        bPDefinitionSettingsPromiseDeferred.promise.then(function () {
                            var payloadBPDefinition = accountActionDefinitionSettings != undefined ? { bpDefinitionSettings: accountActionDefinitionSettings.BPDefinitionSettings } : undefined;
                            VRUIUtilsService.callDirectiveLoad(bPDefinitionSettingsApi, payloadBPDefinition, bPDefinitionSettingsLoadDeferred);
                        });

                        return bPDefinitionSettingsLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountBEActionTypes.BPAccountActionSettings, Retail.BusinessEntity.MainExtensions',
                        BPDefinitionSettings: bPDefinitionSettingsApi.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);