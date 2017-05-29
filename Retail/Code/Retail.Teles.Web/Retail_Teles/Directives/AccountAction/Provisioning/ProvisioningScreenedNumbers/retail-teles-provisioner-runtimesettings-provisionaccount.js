(function (app) {

    'use strict';

    ProvisionerRuntimesettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ProvisionerRuntimesettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ProvisionerRuntimesettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/ProvisioningScreenedNumbers/Templates/ProvisionAccountRuntimeSettingsTemplate.html"

        };
        function ProvisionerRuntimesettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var provisionAccountSettingsAPI;
            var provisionAccountSettingsReadyDeferred = UtilsService.createPromiseDeferred();

            var mainPayload;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onProvisionAccountSettingsReady = function (api) {
                    provisionAccountSettingsAPI = api;
                    provisionAccountSettingsReadyDeferred.resolve();
                };
                var enterpriseNameRegex = new RegExp("^([a-z0-9]{3,}\.[a-z0-9]{3,}\.[a-z0-9]{3,})$");
                $scope.scopeModel.validateEnterpriseName = function () {
                    if ($scope.scopeModel.enterpriseName != undefined && $scope.scopeModel.enterpriseName.match(enterpriseNameRegex))
                        return null;
                    return "Expression must be in this form \"xxx.xxx.xxx\"";
                };
                UtilsService.waitMultiplePromises([provisionAccountSettingsReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var provisionerDefinitionSettings; 
                    var provisionerRuntimeSettings;
                    if (payload != undefined) {
                        provisionerDefinitionSettings = payload.provisionerDefinitionSettings;
                        mainPayload = payload;
                        provisionerRuntimeSettings = payload.provisionerRuntimeSettings;
                        if (provisionerRuntimeSettings != undefined) {
                            $scope.scopeModel.enterpriseName = provisionerRuntimeSettings.EnterpriseName;
                        }

                    }

                    var promises = [];

                    promises.push(loadProvisionAccountSettings());

                    function loadProvisionAccountSettings() {

                        var provisionAccountSettingsPayload = {};
                        if (provisionerDefinitionSettings != undefined)
                            provisionAccountSettingsPayload.provisionAccountSettings = provisionerDefinitionSettings.Settings;
                        if (provisionerRuntimeSettings != undefined) {
                            provisionAccountSettingsPayload.provisionAccountSettings = provisionerRuntimeSettings.Settings;
                        }
                        return provisionAccountSettingsAPI.load(provisionAccountSettingsPayload);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.Teles.Business.Provisioning.ProvisionAccountRuntimeSettings,Retail.Teles.Business",
                        EnterpriseName: $scope.scopeModel.enterpriseName,
                        Settings: provisionAccountSettingsAPI.getData()
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailTelesProvisionerRuntimesettingsProvisionaccount', ProvisionerRuntimesettingsDirective);

})(app);