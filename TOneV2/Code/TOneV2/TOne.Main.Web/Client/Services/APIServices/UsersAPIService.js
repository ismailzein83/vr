app.service('UsersAPIService', function (BaseAPIService) {

    return ({
        GetFilteredUsers: GetFilteredUsers,
        GetUser: GetUser,
        AddUser: AddUser,
        UpdateUser: UpdateUser,
        CheckUserName: CheckUserName,
        ResetPassword: ResetPassword
    });

    function GetFilteredUsers(fromRow, toRow, name, email) {
        return BaseAPIService.get("/api/User/GetFilteredUsers",
            {
                fromRow: fromRow,
                toRow: toRow,
                name: name,
                email: email
            }
           );
    }

    function GetUser(userId) {
        return BaseAPIService.get("/api/User/GetUser",
            {
                UserId: userId
            }
           );
    }

    function AddUser(user) {
        return BaseAPIService.post("/api/User/AddUser",
            user
           );
    }

    function UpdateUser(user) {
        return BaseAPIService.post("/api/User/UpdateUser",
           user
           );
    }

    function CheckUserName(name)
    {
        return BaseAPIService.get("/api/User/CheckUserName",
            {
                Name: name
            }
            );
    }

    function ResetPassword(resetPasswordInput) {
        return BaseAPIService.post("/api/User/ResetPassword",
            resetPasswordInput
           );
    }

});