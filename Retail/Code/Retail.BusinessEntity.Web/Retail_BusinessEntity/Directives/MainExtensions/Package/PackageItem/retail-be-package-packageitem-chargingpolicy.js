(function (app) {

    'use strict';

    ServicePackageChargingPolicyItemDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ServicePackageChargingPolicyItemDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var servicePackageChargingPolicyItem = new ServicePackageChargingPolicyItem($scope, ctrl, $attrs);
                servicePackageChargingPolicyItem.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/Package/PackageItem/Templates/ServicePackageChargingPolicyItemTemplate.html"

        };
        function ServicePackageChargingPolicyItem($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;
            var settingsDirectiveAPI;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onChargingPolicySettingReady = function (api) {
                    settingsDirectiveAPI = api;
                    defineAPI();
                };
               
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        mainPayload = payload;
                        var promises =[];

                        var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                        var settingsDirectivePayload = {
                            serviceTypeId: payload.serviceTypeId,
                            chargingPolicy: payload.settings != undefined ? { Settings: payload.settings.ChargingPolicySettings }:undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, settingsDirectivePayload, settingsDirectiveLoadDeferred);

                        promises.push(settingsDirectiveLoadDeferred.promise);

                        return UtilsService.waitMultiplePromises(promises);
                    }
                      
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.ServicePackageItem.ServicePackageChargingPolicyItem,Retail.BusinessEntity.MainExtensions",
                        ChargingPolicySettings: settingsDirectiveAPI.getData()
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBePackagePackageitemChargingpolicy', ServicePackageChargingPolicyItemDirective);

})(app);