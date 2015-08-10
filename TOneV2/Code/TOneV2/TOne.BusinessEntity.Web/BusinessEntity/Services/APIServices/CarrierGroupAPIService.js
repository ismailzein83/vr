'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetEntityNodes: GetEntityNodes,
        AddGroup: AddGroup,
        UpdateGroup: UpdateGroup,
        GetCarrierAccountsByGroup: GetCarrierAccountsByGroup,//List of all carrier accounts for the ddl // type CarrierAccount
        GetCarrierGroupMembers: GetCarrierGroupMembers,//List of selected carrier accounts for the ddl // type CarrierInfo
        GetCarrierGroup: GetCarrierGroup// Get the name of the carrier group
    });

    function GetEntityNodes() {
        return BaseAPIService.get("/api/CarrierGroup/GetEntityNodes");
    }

    function AddGroup(group) {
        return BaseAPIService.post("/api/CarrierGroup/AddGroup", group);
    }


    function UpdateGroup(group) {
        return BaseAPIService.post("/api/CarrierGroup/UpdateGroup", group);
    }

    function GetCarrierAccountsByGroup(groupId) {
        return BaseAPIService.get("/api/CarrierGroup/GetCarrierAccountsByGroup",
            {
                groupId: groupId
            });
    }

    function GetCarrierGroupMembers(groupId) {
        return BaseAPIService.get("/api/CarrierGroup/GetCarrierGroupMembers",
            {
                groupId: groupId
            });
    }

    function GetCarrierGroup(groupId) {
        return BaseAPIService.get("/api/CarrierGroup/GetCarrierGroup",
            {
                groupId: groupId
            });
    }
    
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('CarrierGroupAPIService', serviceObj);