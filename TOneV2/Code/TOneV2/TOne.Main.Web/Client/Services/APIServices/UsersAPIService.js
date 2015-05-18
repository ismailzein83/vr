app.service('UsersAPIService', function (BaseAPIService) {


    return ({
        GetUserList: GetUserList,
        DeleteUser: DeleteUser,
        SaveUser: SaveUser,
        SearchUser: SearchUser
    });

    function GetUserList() {
        return BaseAPIService.get("/api/User/GetUsers",
            {
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

    function SaveUser(user) {
        return BaseAPIService.post("/api/User/SaveUser",
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