(function (appControllers) {

    'use stict';

    PackageService.$inject = ['VRModalService'];

    function PackageService(VRModalService) {
        return ({
            addPackage: addPackage,
            editPackage: editPackage
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
    }

    appControllers.service('Retail_BE_PackageService', PackageService);

})(appControllers);
