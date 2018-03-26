(function (appControllers) {

    'use strict';

    CollegeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig'];

    function CollegeAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig) {

        var controller = 'College';

        function GetFilteredColleges(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredColleges"), input);
        }

        function GetCollegeById(Id) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'GetCollegeById'),
                { collegeId: Id }
                );
        }


        function GetCollegesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetCollegesInfo"));
        }

        function AddCollege(college) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddCollege"), college);
        }
        function UpdateCollege(college) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateCollege"), college);
        }
        function DeleteCollege(Id) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, 'DeleteCollege'), {
                collegeId: Id
            });
        }

        return ({
            GetCollegeById: GetCollegeById,
            GetCollegesInfo: GetCollegesInfo,
            GetFilteredColleges: GetFilteredColleges,
            AddCollege: AddCollege,
            UpdateCollege: UpdateCollege,
            DeleteCollege: DeleteCollege,
        });
    }


    appControllers.service('Demo_Module_CollegeAPIService', CollegeAPIService);
})(appControllers);