app.service('StrategyAPIService', function (BaseAPIService) {

    return ({
        GetAllStrategies: GetAllStrategies,
        GetFilteredStrategies: GetFilteredStrategies,
        GetNormalCDRs: GetNormalCDRs,
        GetNumberProfiles:GetNumberProfiles,
        AddStrategy: AddStrategy,
        UpdateStrategy: UpdateStrategy,
        GetStrategy: GetStrategy,
        GetFilters: GetFilters,
        GetPeriods: GetPeriods
    });


    function GetNormalCDRs( fromRow,  toRow,  fromDate,  toDate,  msisdn) {
        return BaseAPIService.get("/api/Strategy/GetNormalCDRs",
            {
                fromRow: fromRow,
                toRow: toRow,
                fromDate: fromDate,
                toRow: toRow,
                msisdn: msisdn
            }
           );
    }



        function GetNumberProfiles( fromRow,  toRow,  fromDate,  toDate,  subscriberNumber) {
            return BaseAPIService.get("/api/Strategy/GetNumberProfiles",
            {
                fromRow: fromRow,
                toRow: toRow,
                fromDate: fromDate,
                toRow: toRow,
                subscriberNumber: subscriberNumber
            }
           );
    }



    function GetFilteredStrategies(fromRow, toRow, name, description) {
        return BaseAPIService.get("/api/Strategy/GetFilteredStrategies",
            {
                fromRow: fromRow,
                toRow: toRow,
                name: name,
                description: description
            }
           );
    }

    function GetAllStrategies() {
        return BaseAPIService.get("/api/Strategy/GetAllStrategies");
    }


    function GetStrategy(StrategyId) {
        return BaseAPIService.get("/api/Strategy/GetStrategy",
            {
                StrategyId: StrategyId
            }
           );
    }


    function AddStrategy(strategy) {
        return BaseAPIService.post("/api/Strategy/AddStrategy",
            strategy
           );
    }


    function UpdateStrategy(strategy) {
        return BaseAPIService.post("/api/Strategy/UpdateStrategy",
            strategy
           );
    }


    function GetFilters() {
        return BaseAPIService.get("/api/Strategy/GetFilters");
    }

    function GetPeriods() {
        return BaseAPIService.get("/api/Strategy/GetPeriods");
    }




});