'use strict'
var serviceObj = function (BaseAPIService) {

    return ({
        GetFilteredSwitches: GetFilteredSwitches
    });


    function GetFilteredSwitches(switchName, rowFrom, rowTo) {
        return BaseAPIService.get("/api/SwitchManagement/GetFilteredSwitches",
           {
               switchName: switchName,
               rowFrom: rowFrom,
               rowTo: rowTo
           });
    }
}

serviceObj.$inject = ['BaseAPIService'];
appControllers.service('SwitchManagmentAPIService', serviceObj);
