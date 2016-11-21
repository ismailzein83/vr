'use strict';
var serviceObj = function (BaseAPIService) {

    return ({
        Get: Get
    });

    function Get() {
        return BaseAPIService.get("/api/Values/Get",
            {

            });
    }
};

serviceObj.$inject = ['BaseAPIService'];
appControllers.service('ValuesAPIService', serviceObj);
