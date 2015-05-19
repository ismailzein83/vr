app.service('UsersAPIService', function (BaseAPIService) {


    return ({
        GetUserList: GetUserList,
        DeleteUser: DeleteUser,
        AddUser: AddUser,
        EditUser: EditUser,
        SearchUser: SearchUser
    });

    function GetUserList(params) {
        return BaseAPIService.get("/api/User/GetUsers",
            {
                pageSize: params.pageSize,
                pageNumber: params.pageNumber
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
                Description: user.Description
            }
           );
    }

    function EditUser(user) {
        return BaseAPIService.post("/api/User/EditUser",
            {
                UserId: user.UserId,
                Name: user.Name,
                Password: user.Password,
                Email: user.Email,
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

});