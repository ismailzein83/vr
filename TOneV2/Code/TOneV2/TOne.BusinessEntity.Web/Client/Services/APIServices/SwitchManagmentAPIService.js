'use strict'
var serviceObj = function (BaseAPIService) {

    return ({
        getFilteredSwitches: getFilteredSwitches,
        getSwitchDetails: getSwitchDetails,
        updateSwitch: updateSwitch,
        insertSwitch: insertSwitch
    });


    function getFilteredSwitches(switchName, rowFrom, rowTo) {
        return BaseAPIService.get("/api/Switch/getFilteredSwitches",
           {
               switchName: switchName,
               rowFrom: rowFrom,
               rowTo: rowTo
           });

    }

    function getSwitchDetails(SwitchId) {
        return BaseAPIService.get("/api/Switch/getSwitchDetails",
           {
               SwitchId: SwitchId
           });
    }

    function updateSwitch(switchObject) {
        alert('updateSwitch' + switchObject);
        return BaseAPIService.get("/api/Switch/updateSwitch", switchObject);
    }
    function insertSwitch(switchObject) {
      
        return BaseAPIService.post("/api/Switch/insertSwitch", switchObject);
    }


}

serviceObj.$inject = ['BaseAPIService'];
appControllers.service('SwitchManagmentAPIService', serviceObj);
