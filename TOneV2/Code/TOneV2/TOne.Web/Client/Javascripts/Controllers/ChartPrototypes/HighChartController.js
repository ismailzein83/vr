appControllers.controller('HighChartController',
    function HighChartController($scope, $http) {

        $scope.model = "HighChartController";
        $scope.data = [];
        
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
        $scope.getData();
        var isFirstTimeCodeGroup = true;
        $scope.loadChart = function () {
            var series = [];
            var barSeriers = [
                {
                    name: "Attempts",
                    data: []
                },
                {
                    name: "Successful Attempts",
                    data: []
                }
            ];
            var xAxis = [];
            angular.forEach($scope.data, function (item) {
                if (item.selected == true) {
                    series.push({
                        name: item.ZoneName,
                        y: item.Attempts
                    });
                    barSeriers[0].data.push(item.Attempts);
                    barSeriers[1].data.push(item.SuccessfulAttempts);
                    xAxis.push(item.ZoneName);
                }
            });
            //var chartData = {
            //    "type": "pie3d",
            //    "series": series
            //};
            $('#container').highcharts({
                chart: {
                    type: 'pie',
                    options3d: {
                        enabled: true,
                        alpha: 35,
                        beta: 0
                    }
                },
                title: {
                    text: 'Group Attempts'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        depth: 35,
                        dataLabels: {
                            enabled: true,
                            format: '{point.name}'
                        }
                    }
                },
                yAxis: {
                    title: {
                        text: 'Attempts'
                    }
                },
                series: [{
                    name: 'Attempts',
                    data: series,
                    events:
                        {
                            click: function (e) {
                                console.log(e);
                                console.log(e.point.index);
                                console.log($scope.data[e.point.index].OurZoneID);
                                $scope.selectCodeGroup($scope.data[e.point.index].OurZoneID);
                            }
                        }
                }]
            });

            $('#container1').highcharts({
                chart: {
                    type: 'column',
                    options3d: {
                        enabled: true,
                        alpha: 15,
                        beta: 15,
                        depth: 50,
                        viewDistance: 25
                    }
                },
                title: {
                    text: 'Group Attempts'
                },
                plotOptions: {
                    column: {
                        depth: 25
                    }
                },
                xAxis: {
                    categories: xAxis
                },
                yAxis: {
                    title: {
                        text: 'Attemps'
                    }
                },
                series: barSeriers
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
                  name: item.ZoneName,
                  y: item.Attempts
              });
          });


          $('#container2').highcharts({
              chart: {
                  type: 'pie',
                  options3d: {
                      enabled: true,
                      alpha: 35,
                      beta: 0
                  }
              },
              title: {
                  text: 'Zone Attempts'
              },
              plotOptions: {
                  pie: {
                      allowPointSelect: true,
                      cursor: 'pointer',
                      depth: 35,
                      dataLabels: {
                          enabled: true,
                          format: '{point.name}'
                      }
                  }
              },
              yAxis: {
                  title: {
                      text: 'Attempts'
                  }
              },
              series: [{
                  name: 'Attempts',
                  data: series
              }]
          });
      });
        };

        setTimeout(function () {
            $scope.loadChart();
        }, 600);
        var dynamicChartSeries = [];
        $http.get($scope.baseurl + "/api/ChartTest/GetLatestValues",
          {
          })
      .success(function (response) {
          
          for (var i = 0; i < response.length; i++)
          {
              var ser = {
                  name: "Serie " + i,
                  data: []
              };

              var time = (new Date()).getTime(),
                        j;

              for (j = -500; j <= 0; j += 1) {
                  ser.data.push({
                      x: time + j * 1000,
                      y: null
                  });
              }
              dynamicChartSeries.push(ser);
          }
          $scope.loadDynamicChart();
      });


        $scope.loadDynamicChart = function () {
            Highcharts.setOptions({
                global: {
                    useUTC: false
                }
            });

            $('#container4').highcharts({
                chart: {
                    type: 'spline',
                    animation: Highcharts.svg, // don't animate in old IE
                    marginRight: 10,
                    events: {
                        load: function () {

                            // set up the updating of the chart each second
                            var dynSeries = this.series;
                            setInterval(function () {
                                
                                $http.get($scope.baseurl + "/api/ChartTest/GetLatestValues",
          {              
          })
      .success(function (response) {
          for (var i = 0; i < response.length; i++) {
              angular.forEach(response[i], function (item) {

                  var x = new Date(item.Time).getTime(), // current time
                  y = item.Value;
                  dynSeries[i].addPoint([x, y], true, true);
              });
          }
      });

                               
                            }, 2000);
                        }
                    }
                },
                title: {
                    text: 'Live random data'
                },
                xAxis: {
                    type: 'datetime',
                    tickPixelInterval: 150
                },
                yAxis: {
                    title: {
                        text: 'Value'
                    },
                    plotLines: [{
                        value: 0,
                        width: 1,
                        color: '#808080'
                    }]
                },
                tooltip: {                    
                    shared: true
                },
                legend: {
                    enabled: true
                },
                exporting: {
                    enabled: false
                },
                series: dynamicChartSeries
            });
        };
       
    });