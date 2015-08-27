app.service('RateAnalysisAPIService', function (BaseAPIService) {

    return ({
        GetRateAnalysis: GetRateAnalysis
    });

    function GetRateAnalysis(input) {
        return BaseAPIService.post('/api/RateAnalysis/GetRateAnalysis', input);
    }
  
});