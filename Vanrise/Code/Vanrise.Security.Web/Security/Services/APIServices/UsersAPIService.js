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