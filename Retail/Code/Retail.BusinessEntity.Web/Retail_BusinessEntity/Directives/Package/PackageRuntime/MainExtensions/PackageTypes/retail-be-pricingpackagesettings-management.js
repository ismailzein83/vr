(function (app) {

    'use strict';

    PricingPackageSettingsManagementDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_PricingPackageSettingsService'];

    function PricingPackageSettingsManagementDirective(UtilsService, VRNotificationService, Retail_BE_PricingPackageSettingsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var pricingPackageSettingsManagementCtor = new PricingPackageSettingsManagementCtor($scope, ctrl);
                pricingPackageSettingsManagementCtor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageRuntime/MainExtensions/PackageTypes/Templates/PricingPackageSettingsManagementTemplate.html'
        };

        function PricingPackageSettingsManagementCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var context;
            function initializeController() {
                ctrl.pricingPackageSettings = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                ctrl.onAddPricingPackageSetting = function () {
                    var onPackageSettingAdded = function (addedPackageSetting) {
                        ctrl.pricingPackageSettings.push(addedPackageSetting);
                    };

                    //Excluded ServiceTypeIds for ServiceTypeSelector filter
                    var excludedServiceTypeIds = [];
                    if (ctrl.pricingPackageSettings != undefined) {
                        for (var index = 0 ; index < ctrl.pricingPackageSettings.length; index++) {
                            excludedServiceTypeIds.push(ctrl.pricingPackageSettings[index].ServiceTypeId);
                        }
                    }

                    Retail_BE_PricingPackageSettingsService.addPricingPackageSetting(excludedServiceTypeIds, onPackageSettingAdded, getContext());
                };
                ctrl.onDeletePricingPackageSetting = function (pricingPackageSetting) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal(ctrl.pricingPackageSettings, pricingPackageSetting.ServiceTypeName, 'ServiceTypeName');
                            ctrl.pricingPackageSettings.splice(index, 1);
                        }
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.pricingPackageSettings.length = 0;
                    var pricingPackageSettings;
                    var serviceTypeUsageChargingPolicies;
                    var pricingPackageSettingsEditorRuntime;

                    if (payload != undefined) {
                        pricingPackageSettings = payload.extendedSettings;
                        pricingPackageSettingsEditorRuntime = payload.extendedSettingsEditorRuntime;
                        context = payload.context;
                        if (pricingPackageSettings != undefined) {
                            serviceTypeUsageChargingPolicies = pricingPackageSettings.ServiceTypeUsageChargingPolicies;
                        }
                    }

                    //Loading Grid
                    if (serviceTypeUsageChargingPolicies != undefined) {

                        for (var key in serviceTypeUsageChargingPolicies) {
                            if (key != '$type') {
                                var serviceTypeId = key;
                                var chargingPolicyId = serviceTypeUsageChargingPolicies[key].UsageChargingPolicyId;

                                var packageSetting = {
                                    ServiceTypeId: serviceTypeId,
                                    ServiceTypeName: pricingPackageSettingsEditorRuntime.ServiceTypes[serviceTypeId],
                                    UsageChargingPolicyId: chargingPolicyId,
                                    UsageChargingPolicyName: pricingPackageSettingsEditorRuntime.ChargingPolicies[chargingPolicyId]
                                };
                                ctrl.pricingPackageSettings.push(packageSetting);
                            }
                        }
                    }
                };

                api.getData = function () {

                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.PackageTypes.PricingPackageSettings, Retail.BusinessEntity.MainExtensions",
                        ServiceTypeUsageChargingPolicies: buildPricingPackageSettingsDictionary(ctrl.pricingPackageSettings)
                    };

                    return obj;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildPricingPackageSettingsDictionary(extendedSettings) {
                var pricingPackageSettingsDictionary = {};

                for (var index = 0 ; index < extendedSettings.length; index++) {
                    var currentItem = extendedSettings[index];
                    pricingPackageSettingsDictionary[currentItem.ServiceTypeId] = { UsageChargingPolicyId: currentItem.UsageChargingPolicyId };
                }
                return pricingPackageSettingsDictionary;
            }

            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editPricingPackageSetting
                }];
            }
            function editPricingPackageSetting(pricingPackageSetting) {
                var onPricingPackageSettingUpdated = function (updatedPricingPackageSetting) {
                    var index = UtilsService.getItemIndexByVal(ctrl.pricingPackageSettings, pricingPackageSetting.ServiceTypeName, 'ServiceTypeName');
                    ctrl.pricingPackageSettings[index] = updatedPricingPackageSetting;
                };

                Retail_BE_PricingPackageSettingsService.editPricingPackageSetting(pricingPackageSetting, onPricingPackageSettingUpdated, getContext());
            }
            function getContext()
            {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }
    }

    app.directive('retailBePricingpackagesettingsManagement', PricingPackageSettingsManagementDirective);

})(app);