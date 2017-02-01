'use strict';

app.directive('retailBeAccountactiondefinitionsettingsBpaccountbackendexecutor', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/MainExtensions/BPAccountAction/Templates/BPAccountActionBackendExecutorSettingsTemplate.html'
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
                    var accountActionBackendExecutorEntity;

                    if (payload != undefined)
                    {
                        accountActionDefinitionSettings = payload.actionDefinitionSettings;
                        accountActionBackendExecutorEntity = payload.accountActionBackendExecutorEntity;
                        if (accountActionDefinitionSettings != undefined && accountActionDefinitionSettings.BPDefinitionSettings != undefined)
                            $scope.scopeModel.bpAccountActionRuntimeEditor = accountActionDefinitionSettings.BPDefinitionSettings.RuntimeEditor;
                    }
                    var promises = [];


                    promises.push(loadBusinessEntityDefinitionSelector());

                    function loadBusinessEntityDefinitionSelector() {
                        var bPDefinitionSettingsLoadDeferred = UtilsService.createPromiseDeferred();

                        bPDefinitionSettingsPromiseDeferred.promise.then(function () {
                            var payloadBPDefinition = {};
                            if (accountActionBackendExecutorEntity != undefined)
                            {
                                payloadBPDefinition.vrActionEntity = { ActionBPSettings: accountActionBackendExecutorEntity.BPSettings };
                            }
                            if (accountActionDefinitionSettings != undefined) {
                                payloadBPDefinition.bpDefinitionSettings = accountActionDefinitionSettings.BPDefinitionSettings;
                            }
                            VRUIUtilsService.callDirectiveLoad(bPDefinitionSettingsApi, payloadBPDefinition, bPDefinitionSettingsLoadDeferred);
                        });

                        return bPDefinitionSettingsLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountBEActionTypes.BPAccountActionBackendExecutor, Retail.BusinessEntity.MainExtensions',
                        BPSettings: bPDefinitionSettingsApi.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);