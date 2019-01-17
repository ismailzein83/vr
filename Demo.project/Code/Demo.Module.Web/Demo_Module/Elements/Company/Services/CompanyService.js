(function (appControllers) { // Edit/Add functions to company grid

    "use strict";

    companyService.$inject = ['VRModalService'];

    function companyService(VRModalService) {

        function addCompany(onCompanyAdded) {
            var parameters = {};
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onCompanyAdded = onCompanyAdded;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Company/Views/CompanyEditor.html', parameters, settings);
        };

        function editCompany(onCompanyUpdated, companyId) {
            var parameters = {
                companyId: companyId
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onCompanyUpdated = onCompanyUpdated;
            };
            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Company/Views/CompanyEditor.html', parameters, settings);

        };

        return {
            addCompany: addCompany,
            editCompany: editCompany
        };

    };

    appControllers.service("Demo_Module_CompanyService", companyService);
})(appControllers);