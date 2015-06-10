﻿app.service('UsersAPIService', function (BaseAPIService) {

    return ({
        GetFilteredUsers: GetFilteredUsers,
        GetUserbyId: GetUserbyId,
        AddUser: AddUser,
        UpdateUser: UpdateUser,
        CheckUserName: CheckUserName,
        ResetPassword: ResetPassword,
        GetUsers: GetUsers,
        GetMembers: GetMembers
    });

    function GetFilteredUsers(fromRow, toRow, name, email) {
        return BaseAPIService.get("/api/Users/GetFilteredUsers",
            {
                fromRow: fromRow,
                toRow: toRow,
                name: name,
                email: email
            }
           );
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

    function GetMembers(roleId) {
        return BaseAPIService.get("/api/Users/GetMembers", 
             {
                 roleId: roleId
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

});