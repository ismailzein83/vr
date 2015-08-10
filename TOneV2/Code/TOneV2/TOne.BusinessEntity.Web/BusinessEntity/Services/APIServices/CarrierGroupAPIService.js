'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetEntityNodes: GetEntityNodes,
        AddGroup: AddGroup,
        //UpdateGroup: UpdateGroup,
        GetCarrierAccountsByGroup: GetCarrierAccountsByGroup,
        //GetCarriersInfoByGroup: GetCarriersInfoByGroup,
        //GetCarrierGroup: GetCarrierGroup
    });

    function GetEntityNodes() {
        return BaseAPIService.get("/api/CarrierGroup/GetEntityNodes");
    }

    function AddGroup(group) {
        return BaseAPIService.post("/api/CarrierGroup/AddGroup", group);
    }


    //function UpdateGroup(group) {
    //    return BaseAPIService.post("/api/CarrierGroup/UpdateGroup", group);
    //}

    function GetCarrierAccountsByGroup(groupId) {
        return BaseAPIService.get("/api/CarrierGroup/GetCarrierAccountsByGroup",
            {
                groupId: groupId
            });
    }

    //function GetCarriersInfoByGroup(groupId) {
    //    return BaseAPIService.get("/api/CarrierGroup/GetCarriersInfoByGroup",
    //        {
    //            groupId: groupId
    //        });
    //}



    //function GetCarrierGroups() {
    //    return BaseAPIService.get("/api/CarrierGroup/GetCarrierGroups");
    //}

    //function GetCarrierGroup(groupId) {
    //    return BaseAPIService.get("/api/CarrierGroup/GetCarrierGroup",
    //        {
    //            groupId: groupId
    //        });
    //}
    
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('CarrierGroupAPIService', serviceObj);