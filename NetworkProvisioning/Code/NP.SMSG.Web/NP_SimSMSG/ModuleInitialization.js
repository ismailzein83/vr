app.run(['NP_SimSMSG_Service',
    function (NP_SimSMSG_Service) {
        NP_SimSMSG_Service.registerClientDrillDownToCarrierAccount();
        NP_SimSMSG_Service.registerServerDrillDownToCarrierAccount();
    }]);

