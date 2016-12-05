appControllers.controller('HighChartController',
    function HighChartController($scope, $http, AnalyticsAPIService) {

        defineScopeObjects();
        defineScopeMethods();
        load();

        var dynamicChartSeries = [];

        function defineScopeObjects() {
            $scope.model = "HighChartController";
            $scope.data = [];            
            $scope.data2 = [];
        }

        function defineScopeMethods() {
        }

        function load()
        {
            getData();

            setTimeout(function () {
                loadChart();
            }, 600);
            
            $http.get($scope.baseurl + "/api/ChartTest/GetLatestValues",
              {
              })
          .success(function (response) {

              for (var i = 0; i < response.length; i++) {
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
              loadDynamicChart();
          });
        }

        function loadChart() {
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
                                selectCodeGroup($scope.data[e.point.index].OurZoneID);
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
        }

        function getData() {
            AnalyticsAPIService.GetTopNDestinations('2014-04-28', '2014-04-29', 1, 10, 10, 'Y', 'Y')
       .then(function (response) {
           $scope.data.length = 0;
           angular.forEach(response, function (item) {
               item.selected = true;
               $scope.data.push(item)
           });
       }).catch(function (error) {
           alert('error: ' + error);
       });
        }

        function selectCodeGroup(codeGroup) {
            AnalyticsAPIService.GetTopNDestinations('2014-04-28', '2014-04-29', 1, 10, 10, 'Y', undefined, codeGroup)
           .then(function (response) {
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
        }

        function loadDynamicChart() {
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