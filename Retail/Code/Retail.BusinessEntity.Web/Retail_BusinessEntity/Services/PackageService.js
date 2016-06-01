(function (appControllers) {

    'use stict';

    PackageService.$inject = ['VRModalService'];

    function PackageService(VRModalService) {
        return ({
            addPackage: addPackage,
            editPackage: editPackage,
            addService: addService,
            editService: editService
        });

        function addPackage(onPackageAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {

                modalScope.onPackageAdded = onPackageAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Package/PackageEditor.html', null, settings);
        }

        function editPackage(packageId, onPackageUpdated) {
            var modalSettings = {
            };

            var parameters = {
                PackageId: packageId,
            };

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
    }

    appControllers.service('Retail_BE_PackageService', PackageService);

})(appControllers);
