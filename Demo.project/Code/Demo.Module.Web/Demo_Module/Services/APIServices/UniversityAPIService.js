(function (appControllers) {

    'use strict';

    UniversityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig'];

    function UniversityAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig) {

        var controller = 'University';

        function GetFilteredUniversities(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredUniversities"), input);
        }

        function GetUniversityById(Id) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetUniversityById'),
                { universityId: Id }
                );
        }


        function GetUniversitiesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetUniversitiesInfo"));
        }

        function AddUniversity(university) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddUniversity"), university);
        }
        function UpdateUniversity(university) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateUniversity"), university);
        }
        function DeleteUniversity(Id) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'DeleteUniversity'), {
                universityId: Id
            });
        }

        return ({
            GetUniversityById: GetUniversityById,
            GetUniversitiesInfo: GetUniversitiesInfo,
            GetFilteredUniversities: GetFilteredUniversities,
            AddUniversity: AddUniversity,
            UpdateUniversity: UpdateUniversity,
            DeleteUniversity: DeleteUniversity,
        });
    }


    appControllers.service('Demo_Module_UniversityAPIService', UniversityAPIService);
})(appControllers);