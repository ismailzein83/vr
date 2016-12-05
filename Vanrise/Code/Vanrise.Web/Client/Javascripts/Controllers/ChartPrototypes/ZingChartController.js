appControllers.controller('ZingChartController',
    function ZingChartController($scope, $http) {

        $scope.model = "ZingChartController Model";
        $scope.data = [];
        var isFirstTimeCodeGroup = true;
        $scope.loadChart = function () {
            var series = [];
            var barSeriers = [
                {
                    text: "Attempts",
                    values: []
                },
                {
                    text: "Successful Attempts",
                    values: []
                }
            ];
            var xAxis = [];
            angular.forEach($scope.data, function (item) {
                if (item.selected == true) {
                    series.push({
                        "text": item.ZoneName,
                        "values": [item.Attempts]
                    });
                    barSeriers[0].values.push(item.Attempts);
                    barSeriers[1].values.push(item.SuccessfulAttempts);
                    xAxis.push(item.ZoneName);
                }
            });
            var chartData = {
                "type": "pie3d",                
                "series": series
            };
            var chartData2 = {
                "type": "bar",
                "legend": {

                },
                "scale-x": {
                    values: xAxis
                },
                "series": barSeriers
            };
            if (isFirstTimeCodeGroup) {
                zingchart.render({
                    id: 'chartDiv',
                    data: chartData
                });
                zingchart.render({
                    id: 'chartDiv3',
                    data: chartData2
                });
                isFirstTimeCodeGroup = false;
            }
            else
            {
                zingchart.exec("chartDiv", "setseriesdata", {
                    "data": series
                });
                zingchart.render({
                    id: 'chartDiv3',
                    data: chartData2
                });
            }
        };

        $scope.getData = function () {
            $http.get($scope.baseurl + "/api/Analytics/GetTopNDestinations",
           {
               params: {
                   fromDate: '2014-04-28',
                   toDate: '2014-04-29',
                   from: 1,
                   to: 10,
                   topCount: 10,
                   showSupplier: 'Y',
                   groupByCodeGroup: 'Y'
               }
           })
       .success(function (response) {
           $scope.data.length = 0;
           angular.forEach(response, function (item) {
               item.selected = true;
               $scope.data.push(item)
           });
       });
        };


        $scope.data2 = [];
        var isFirstTime = true;
        $scope.selectCodeGroup = function (codeGroup) {
            $http.get($scope.baseurl + "/api/Analytics/GetTopNDestinations",
          {
              params: {
                  fromDate: '2014-04-28',
                  toDate: '2014-04-29',
                  from: 1,
                  to: 10,
                  topCount: 10,
                  showSupplier: 'Y',
                  codeGroup: codeGroup
              }
          })
      .success(function (response) {
          $scope.data2.length = 0;
          angular.forEach(response, function (item) {
              $scope.data2.push(item)
          });

          var series = [];
          angular.forEach($scope.data2, function (item) {
              series.push({
                      "text": item.ZoneName,
                      "values": [item.Attempts]
                  });
          });
          var chartData = {
              "type": "pie3d",
              "series": series
          };
          if (isFirstTime) {
              zingchart.render({
                  id: 'chartDiv2',
                  data: chartData
              });
              isFirstTime = false;
          }
          else {
              zingchart.exec("chartDiv2", "setseriesdata", {
                  "data": series
              });
          }
      });
        };

        zingchart.bind('chartDiv', 'node_click', function (p) {
            $scope.selectCodeGroup($scope.data[p.plotindex].OurZoneID);
        });        
        $scope.getData();
        setTimeout(function () {
            $scope.loadChart();
        }, 600);
    });