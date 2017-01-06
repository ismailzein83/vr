(function (appControllers) {

    'use stict';

    PackageService.$inject = ['VRModalService'];

    function PackageService(VRModalService) {


        function addPackage(onPackageAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPackageAdded = onPackageAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Package/PackageEditor.html', null, settings);
        }
        function editPackage(packageId, onPackageUpdated) {

            var parameters = {
                PackageId: packageId,
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onPackageUpdated = onPackageUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Package/PackageEditor.html', parameters, modalSettings);
        }

        function addService(onServiceAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {

                modalScope.onServiceAdded = onServiceAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Package/ServiceEditor.html', null, settings);
        }
        function editService(serviceEntity, onServiceUpdated) {
            var modalSettings = {
            };

            var parameters = {
                serviceEntity: serviceEntity,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onServiceUpdated = onServiceUpdated;
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Package/ServiceEditor.html', parameters, modalSettings);
        }

        function addPackageItem(onPackageItemAdded, context) {
            var parameters = {
                context: context
            };
            var settings = {};

            settings.onScopeReady = function (modalScope) {

                modalScope.onPackageItemAdded = onPackageItemAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Package/PackageItemEditor.html', parameters, settings);
        }
        function editPackageItem(packageItem, onPackageItemUpdated,context) {
            var modalSettings = {
            };

            var parameters = {
                packageItem: packageItem,
                context: context
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onPackageItemUpdated = onPackageItemUpdated;
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Package/PackageItemEditor.html', parameters, modalSettings);
        }


        return ({
            addPackage: addPackage,
            editPackage: editPackage,
            addService: addService,
            editService: editService,
            addPackageItem: addPackageItem,
            editPackageItem: editPackageItem
        });
    }

    appControllers.service('Retail_BE_PackageService', PackageService);

})(appControllers);
