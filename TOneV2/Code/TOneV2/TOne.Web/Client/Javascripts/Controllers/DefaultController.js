appControllers.controller('DefaultController',
    function DefaultController($scope, $http) {
       
        $scope.testModel = 'initial from default';

        $scope.postMsg = function () {
            $http.post($scope.baseurl + "/api/routing/SaveRouteRule",
          {
              RouteRuleId: 4,
              CodeSet: {
                  $type: "TOne.LCR.Entities.CodeSelectionSet, TOne.LCR.Entities",
                  Code: "5345",
                  ExcludedCodes:["534","5346"]
              },
              CarrierAccountSet: {
                  $type: "TOne.LCR.Entities.CustomerSelectionSet, TOne.LCR.Entities",
                  Customers: {
                      SelectionOption: "AllExceptItems",
                      SelectedValues: ["C4444","4656"]
                  }                 
              },
              ActionData: {
                $type: "TOne.LCR.Entities.OverrideRouteActionData, TOne.LCR.Entities",
                NoOptionAction: "SwitchToLCR",
                Options: [{
                    SupplierId: "C555fd",
                    Percentage:55
                }, {
                    SupplierId: "D543",
                    Percentage: 5
                }, {
                    SupplierId: "G655",
                    Percentage: 34
                }]
            }
          })
      .success(function (response) {
          
      });
        };
    });