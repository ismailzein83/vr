'use strict'
var serviceObj = function (BaseAPIService) {

    return ({
        GetFilteredSwitches: GetFilteredSwitches,
        getSwitchDetails: getSwitchDetails,
        updateSwitch: updateSwitch,
        insertSwitch: insertSwitch
    });


    function GetFilteredSwitches(switchName, rowFrom, rowTo) {
        return BaseAPIService.get("/api/Switch/GetFilteredSwitches",
           {
               switchName: switchName,
               rowFrom: rowFrom,
               rowTo: rowTo
           });

    }

    function getSwitchDetails(SwitchId) {
        return BaseAPIService.get("/api/Switch/GetSwitchDetails",
           {
               SwitchId: SwitchId
           });
    }

    function updateSwitch(switchObject) {
        return BaseAPIService.get("/api/Switch/updateSwitch", switchObject);
    }
    function insertSwitch(switchObject) {
        return BaseAPIService.post("/api/Switch/insertSwitch", switchObject);
    }


}

serviceObj.$inject = ['BaseAPIService'];
appControllers.service('SwitchManagmentAPIService', serviceObj);
