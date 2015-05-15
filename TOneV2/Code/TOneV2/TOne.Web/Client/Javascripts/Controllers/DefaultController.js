appControllers.controller('DefaultController',
    function DefaultController($scope, $http, BusinessEntityAPIService) {
       
        $scope.testModel = 'initial from default';
        $scope.html = '<input ng-click="click(1)" value="Click me" type="button">';
        $scope.click = function (arg) {
            alert('Clicked ' + arg);
        }
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
        var current = 0;
        $scope.gridData = [];
        $scope.loadMoreData = function (asyncHandle) {
            BusinessEntityAPIService.GetCodeGroups().then(function (response) {
                var count = current + 20;
                for (current; current < count; current++) {
                    $scope.gridData.push({
                        col1: "test " + current + "1",
                        col2: "test " + current + "2",
                        col3: "test " + current + "3",
                    });
                }

                
            })
                .finally(function () {
                    if (asyncHandle)
                        asyncHandle.operationDone();
                });
            //setTimeout(function () {
            //    $scope.$apply(function () {
                    
                    
            //    });
                
            //}, 2000);

        };
        $scope.loadMoreData();

        $scope.addItem = function () {
            var item = {
                col1: "test " + ++current + "1",
                col2: "test " + current + "2",
                col3: "test " + current + "3",
            };
            $scope.gridData.push(item);
            gridApi.itemAdded(item); 
        }

        var gridApi;
        $scope.gridReady = function (api) {
            gridApi = api;
        };
       
    });