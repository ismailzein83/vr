(function (appControllers) {

    'use stict';

    PricingPackageSettingsService.$inject = ['VRModalService'];

    function PricingPackageSettingsService(VRModalService) {


        function addPricingPackageSetting(excludedServiceTypeIds, onPricingPackageSettingsAdded,context) {

            var parameters = {
                excludedServiceTypeIds: excludedServiceTypeIds ,// passed for validation
                context:context
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onPricingPackageSettingsAdded = onPricingPackageSettingsAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Package/PackageExtendedSettings/PricingPackageSettingsEditor.html', parameters, modalSettings);
        }

        function editPricingPackageSetting(pricingPackageSetting, onPricingPackageSettingsUpdated, context) {

            var parameters = {
                pricingPackageSetting: pricingPackageSetting,
                context: context
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onPricingPackageSettingsUpdated = onPricingPackageSettingsUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Package/PackageExtendedSettings/PricingPackageSettingsEditor.html', parameters, modalSettings);
        }


        return ({
            addPricingPackageSetting: addPricingPackageSetting,
            editPricingPackageSetting: editPricingPackageSetting,
        });
    }

    appControllers.service('Retail_BE_PricingPackageSettingsService', PricingPackageSettingsService);

})(appControllers);
