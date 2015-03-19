'use strict'

app.service('CarriersService', function (HttpService, MainService) {

    return ({
        getCustomers: getCustomers
    });

    function getCustomers() {

        var getCarriersURL = MainService.getBaseURL() + "/api/BusinessEntity/GetCarriers";
        return HttpService.get(getCarriersURL, { carrierType: 1 });
    }
});