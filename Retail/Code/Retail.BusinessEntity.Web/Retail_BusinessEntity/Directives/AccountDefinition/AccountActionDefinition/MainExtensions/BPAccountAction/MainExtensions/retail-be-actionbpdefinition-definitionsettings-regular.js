(function (app) {

    'use strict';

    RegularActionbpdefinitionDefinitionsettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function RegularActionbpdefinitionDefinitionsettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var regularActionbpdefinitionDefinitionsettings = new RegularActionbpdefinitionDefinitionsettings($scope, ctrl, $attrs);
                regularActionbpdefinitionDefinitionsettings.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/MainExtensions/BPAccountAction/MainExtensions/Templates/RegularActionBPDefinitionSettingsTemplate.html"

        };
        function RegularActionbpdefinitionDefinitionsettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;

            var provisionerdefinitionSettingAPI;
            var provisionerdefinitionSettingReadyDeferred = UtilsService.createPromiseDeferred();

            var postActionDirectiveAPI;
            var postActionDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            var accountBEDefinitionId;

            function initializeController() {
                $scope.scopeModel = {};
                
                $scope.scopeModel.onPostActionDirectiveReady = function (api) {
                    postActionDirectiveAPI = api;
                    postActionDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onProvisionerDefinitionSettingReady = function (api) {
                    provisionerdefinitionSettingAPI = api;
                    provisionerdefinitionSettingReadyDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([provisionerdefinitionSettingReadyDeferred.promise, postActionDirectiveReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var bpDefinitionSettings;

                    if (payload != undefined) {
                        mainPayload = payload;
                        bpDefinitionSettings = payload.bpDefinitionSettings;
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                    }

                    function loadPostActionDirective() {
                        var postActionDirectivePayload = { accountBEDefinitionId: accountBEDefinitionId };
                        if (bpDefinitionSettings != undefined)
                            postActionDirectivePayload.accountProvisionDefinitionPostAction = bpDefinitionSettings.ProvisionDefinitionPostAction;
                        return postActionDirectiveAPI.load(postActionDirectivePayload);
                    }

                    promises.push(loadPostActionDirective());

                    function loadProvisionerdefinitionSetting() {
                        var definitionSettingPayload = { accountBEDefinitionId: accountBEDefinitionId };
                        if (bpDefinitionSettings != undefined)
                            definitionSettingPayload.provisionerDefinitionSettings =bpDefinitionSettings.ProvisionerDefinitionSettings;
                        return provisionerdefinitionSettingAPI.load(definitionSettingPayload);
                    }

                    promises.push(loadProvisionerdefinitionSetting());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainActionBPs.Entities.RegularActionBPDefinitionSettings, Retail.BusinessEntity.MainActionBPs.Entities",
                        ProvisionerDefinitionSettings: provisionerdefinitionSettingAPI.getData(),
                        ProvisionDefinitionPostAction: postActionDirectiveAPI.getData()
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBeActionbpdefinitionDefinitionsettingsRegular', RegularActionbpdefinitionDefinitionsettingsDirective);

})(app);