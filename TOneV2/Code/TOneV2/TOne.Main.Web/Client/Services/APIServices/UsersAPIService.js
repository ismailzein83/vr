app.service('UsersAPIService', function (BaseAPIService) {

    return ({
        GetUserList: GetUserList,
        DeleteUser: DeleteUser,
        AddUser: AddUser,
        UpdateUser: UpdateUser,
        SearchUser: SearchUser,
        CheckUserName: CheckUserName,
        ResetPassword: ResetPassword
    });

    function GetUserList(params) {
        return BaseAPIService.get("/api/User/GetUsers",
            {
                fromRow: params.fromRow,
                toRow: params.toRow
            }
           );
    }

    function DeleteUser(id) {
        return BaseAPIService.get("/api/User/DeleteUser",
            {
                Id: id
            }
           );
    }

    function AddUser(user) {
        return BaseAPIService.post("/api/User/AddUser",
            {
                UserId: user.UserId,
                Name: user.Name,
                Password: user.Password,
                Email: user.Email,
                IsActive: user.IsActive,
                Description: user.Description
            }
           );
    }

    function UpdateUser(user) {
        return BaseAPIService.post("/api/User/UpdateUser",
            {
                UserId: user.UserId,
                Name: user.Name,
                Password: user.Password,
                Email: user.Email,
                IsActive: user.IsActive,
                Description: user.Description
            }
           );
    }

    function SearchUser(name, email) {
        return BaseAPIService.get("/api/User/SearchUser",
            {
                Name: name,
                Email: email
            }
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

    function ResetPassword(ResetPasswordInput) {
        return BaseAPIService.post("/api/User/ResetPassword",
            {
                UserId: ResetPasswordInput.UserId,
                Password: ResetPasswordInput.Password
            }
           );
    }

});