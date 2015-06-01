﻿appControllers.controller('DefaultController',
    function DefaultController($scope, $http, ValuesAPIService) {
       
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
        $scope.loadMoreData = function () {
            var pageInfo = gridApi.getPageInfo();
            return ValuesAPIService.Get().then(function (response) {
                for (current = pageInfo.fromRow; current <= pageInfo.toRow; current++) {
                    $scope.gridData.push({
                        col1: "test " + current + "1",
                        col2: "test " + current + "2",
                        col3: "test " + current + "3",
                    });
                }


            });
            //setTimeout(function () {
            //    $scope.$apply(function () {
                    
                    
            //    });
                
            //}, 2000);

        };

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
            $scope.loadMoreData();
        };
        $scope.testObj = {};
        $scope.choiceSelectionChanged = function () {
            //console.log($scope.testObj);
        };

        var choicesApi;
        $scope.choicesReady = function (api) {
            choicesApi = api;
        };
       
        $scope.selectChoice = function () {
            choicesApi.selectChoice($scope.choiceIndex);
        };
    });