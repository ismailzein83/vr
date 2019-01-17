(function (appControllers) {

    "use strict";

    companyAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];

    function companyAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Demo_Company";

        function GetFilteredCompanies(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredCompanies"), input);
        };

        function GetCompanyById(companyId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetCompanyById"), {
                companyId: companyId
            });
        };

        function AddCompany(company) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddCompany"), company);
        };

        function UpdateCompany(company) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateCompany"), company);
        };

        function GetCompaniesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetCompaniesInfo"), {
                filter: filter
            });
        };

        return {
            GetFilteredCompanies: GetFilteredCompanies,
            GetCompanyById: GetCompanyById,
            AddCompany: AddCompany,
            UpdateCompany: UpdateCompany,
            GetCompaniesInfo: GetCompaniesInfo
        };
    };

    appControllers.service("Demo_Module_CompanyAPIService", companyAPIService);
})(appControllers);







