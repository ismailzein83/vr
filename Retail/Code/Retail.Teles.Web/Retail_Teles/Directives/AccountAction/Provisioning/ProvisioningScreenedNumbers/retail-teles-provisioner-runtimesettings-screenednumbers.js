﻿(function (app) {

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
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/ProvisioningScreenedNumbers/Templates/ProvisioningScreenedNumbersRuntimeSettingsTemplate.html"

        };
        function ProvisionerRuntimesettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;
            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var provisionerRuntimeSettings;
                    if (payload != undefined) {
                        mainPayload = payload;
                        provisionerRuntimeSettings = payload.provisionerRuntimeSettings;
                        if (provisionerRuntimeSettings != undefined) {
                            $scope.scopeModel.enterpriseName = provisionerRuntimeSettings.EnterpriseName;
                        }

                    }

                    var promises = [];


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.Teles.Business.Provisioning.ProvisioningScreenedNumbersRuntimeSettings,Retail.Teles.Business",
                        EnterpriseName: $scope.scopeModel.enterpriseName
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailTelesProvisionerRuntimesettingsScreenednumbers', ProvisionerRuntimesettingsDirective);

})(app);