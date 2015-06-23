﻿var BusinessProcessAPIService = function (BaseAPIService) {

    "use strict";

    function GetDefinitions() {
        return BaseAPIService.get("/api/BusinessProcess/GetDefinitions");
    }

    function GetStatusList() {
        return BaseAPIService.get("/api/BusinessProcess/GetStatusList");
    }

    function GetFilteredBProcess(param) {
        return BaseAPIService.post("/api/BusinessProcess/GetFilteredBProcess", param);
    }

    function GetFilteredDefinitions(fromRow, toRow, title) {
        return BaseAPIService.get("/api/BusinessProcess/GetFilteredDefinitions",
            {
                fromRow: fromRow,
                toRow: toRow,
                title: title
            });
    }

    function GetTrackingsByInstanceId(processInstanceID, fromRow, toRow) {
        return BaseAPIService.get("/api/BusinessProcess/GetTrackingsByInstanceId", {
            processInstanceID: processInstanceID,
            fromRow: fromRow,
            toRow: toRow
        });
    }

    return ({
        GetDefinitions: GetDefinitions,
        GetFilteredDefinitions: GetFilteredDefinitions,
        GetStatusList: GetStatusList,
        GetFilteredBProcess: GetFilteredBProcess,
        GetTrackingsByInstanceId: GetTrackingsByInstanceId
    });

};
BusinessProcessAPIService.$inject = ['BaseAPIService'];
appControllers.service('BusinessProcessAPIService', BusinessProcessAPIService);