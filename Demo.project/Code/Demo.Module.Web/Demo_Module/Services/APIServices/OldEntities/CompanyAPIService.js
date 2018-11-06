(function (appControllers) {
    "use strict";
    companyAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function companyAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Company";

        function AddCompany(company) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddCompany"), company);
        };

        function GetFilteredCompanies(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredCompanies"), input);
        }
        function GetCompanyById(companyId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetCompanyById"),
                {
                    companyId: companyId
                });
        }

        function UpdateCompany(company) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateCompany"), company);
        }

        function GetCompaniesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetCompaniesInfo"), {
                filter: filter
            });
        }

        return {
            AddCompany: AddCompany,            
            GetFilteredCompanies: GetFilteredCompanies,
            GetCompanyById: GetCompanyById,
            UpdateCompany: UpdateCompany,
            GetCompaniesInfo: GetCompaniesInfo
        };
    };
    appControllers.service("Demo_Module_CompanyAPIService", companyAPIService);

})(appControllers);