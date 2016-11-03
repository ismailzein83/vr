(function (app) {

    'use strict';

    ProvisionerDefinitionsettingsTelesSwitchDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ProvisionerDefinitionsettingsTelesSwitchDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var radiusProvisionerDefinitionSetting = new TelesSwitchProvisionerDefinitionSetting($scope, ctrl, $attrs);
                radiusProvisionerDefinitionSetting.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/Action/Definition/ProvisionerDefinition/Templates/ActivateTelesSwitchProvisionerDefinitionSettingsTemplate.html"

        };
        function TelesSwitchProvisionerDefinitionSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;

            var serviceTypeAPI;
            var serviceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onServiceTypeReady = function (api)
                {
                    serviceTypeAPI = api;
                    serviceTypeSelectorReadyDeferred.resolve();
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    console.log(payload);
                    if (payload != undefined) {
                        mainPayload = payload;
                        if (payload.provisionerDefinitionSettings != undefined) {
                        }
                    }

                    var serviceTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    serviceTypeSelectorReadyDeferred.promise.then(function () {
                        var serviceTypeSelectorPayload;
                        if (payload != undefined && payload.provisionerDefinitionSettings != undefined) {
                            serviceTypeSelectorPayload = { selectedIds: payload.provisionerDefinitionSettings.ServiceTypeIds };
                        }
                        VRUIUtilsService.callDirectiveLoad(serviceTypeAPI, serviceTypeSelectorPayload, serviceTypeSelectorLoadDeferred);
                    });
                    return serviceTypeSelectorLoadDeferred.promise

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.ActivateTelesSwitchUserProvisionerDefinitionSettings,Retail.BusinessEntity.MainExtensions",
                        ServiceTypeIds: serviceTypeAPI.getSelectedIds()
                    }
                    return data;
                }
            }
        }
    }

    app.directive('retailBeProvisionerDefinitionsettingsActivatetelesswitch', ProvisionerDefinitionsettingsTelesSwitchDirective);

})(app);