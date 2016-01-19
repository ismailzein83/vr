app.service('UsersAPIService', function (BaseAPIService) {

    return ({
        GetFilteredUsers: GetFilteredUsers,
        GetUserbyId: GetUserbyId,
        AddUser: AddUser,
        UpdateUser: UpdateUser,
        CheckUserName: CheckUserName,
        ResetPassword: ResetPassword,
        GetUsers: GetUsers,
        GetMembers: GetMembers,
        EditUserProfile: EditUserProfile,
        LoadLoggedInUserProfile: LoadLoggedInUserProfile

    });

    function GetFilteredUsers(input) {
        return BaseAPIService.post("/api/Users/GetFilteredUsers", input);
    }

    function GetUsers() {
        return BaseAPIService.get("/api/Users/GetUsers");
    }

    function GetUserbyId(userId) {
        return BaseAPIService.get("/api/Users/GetUserbyId",
            {
                UserId: userId
            }
           );
    }

    function GetMembers(groupId) {
        return BaseAPIService.get("/api/Users/GetMembers", 
             {
                 groupId: groupId
             });
    }

    function AddUser(user) {
        return BaseAPIService.post("/api/Users/AddUser",
            user
           );
    }

    function UpdateUser(user) {
        return BaseAPIService.post("/api/Users/UpdateUser",
           user
           );
    }

    function CheckUserName(name) {
        return BaseAPIService.get("/api/Users/CheckUserName",
            {
                Name: name
            }
            );
    }

    function ResetPassword(resetPasswordInput) {
        return BaseAPIService.post("/api/Users/ResetPassword",
            resetPasswordInput
           );
    }

    function EditUserProfile(userProfileObject) {
        return BaseAPIService.post("/api/Users/EditUserProfile", userProfileObject);
    }

    function LoadLoggedInUserProfile() {
        return BaseAPIService.get("/api/Users/LoadLoggedInUserProfile");
    }

});



(function (appControllers) {

    "use strict";
    userAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function userAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {



        function GetFilteredUsers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Users", "GetFilteredUsers"), input);
        }

        function GetUsers() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Users", "GetUsers"));

        }

        function GetUserbyId(userId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Users", "GetUserbyId"),
                {
                  UserId: userId
                }
            );

         
        }

        function GetMembers(groupId) {

            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Users", "GetMembers"),
                {
                  groupId: groupId
                }
            );
        }

        function AddUser(user) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Users", "AddUser"),
                user
               );
        }

        function UpdateUser(user) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Users", "UpdateUser"),
               user
              );
        }

        function CheckUserName(name) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Users", "CheckUserName"),
             {
                 Name: name
             }
            );
           
        }

        function ResetPassword(resetPasswordInput) {   
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Users", "ResetPassword"),
                resetPasswordInput
             );
        }

        function EditUserProfile(userProfileObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Users", "EditUserProfile"),
                userProfileObject
             );
        }

        function LoadLoggedInUserProfile() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Users", "LoadLoggedInUserProfile"));
        }

        return ({
            GetFilteredUsers: GetFilteredUsers,
            GetUserbyId: GetUserbyId,
            AddUser: AddUser,
            UpdateUser: UpdateUser,
            CheckUserName: CheckUserName,
            ResetPassword: ResetPassword,
            GetUsers: GetUsers,
            GetMembers: GetMembers,
            EditUserProfile: EditUserProfile,
            LoadLoggedInUserProfile: LoadLoggedInUserProfile

        });
    }

    appControllers.service('VR_Sec_UserAPIService', userAPIService);
})(appControllers);