'use strict'
var serviceObj = function (BaseAPIService) {

    return ({
        GetFilteredSwitches: GetFilteredSwitches,
        GetSwitchDetails: GetSwitchDetails,
        UpdateSwitch: UpdateSwitch,
        InsertSwitch: InsertSwitch
    });


    function GetFilteredSwitches(switchName, rowFrom, rowTo) {
        return BaseAPIService.get("/api/Switch/GetFilteredSwitches",
           {
               switchName: switchName,
               rowFrom: rowFrom,
               rowTo: rowTo
           });

    }

    function GetSwitchDetails(SwitchId) {
        return BaseAPIService.get("/api/Switch/GetSwitchDetails",
           {
               SwitchId: SwitchId
           });
    }

    function UpdateSwitch(switchObject) {
        return BaseAPIService.post("/api/Switch/UpdateSwitch", switchObject);
    }
    function InsertSwitch(switchObject) {
        return BaseAPIService.post("/api/Switch/InsertSwitch", switchObject);
    }


}

serviceObj.$inject = ['BaseAPIService'];
appControllers.service('SwitchManagmentAPIService', serviceObj);
