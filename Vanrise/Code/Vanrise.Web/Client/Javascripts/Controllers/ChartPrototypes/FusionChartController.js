appControllers.controller('FusionChartController',
    function FusionChartController($scope, $http) {

        $scope.model = "FusionChartController";

       
        $scope.codeGroups = null;
       
       

        FusionCharts.ready(function () {
           
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
           $scope.codeGroups = response;
           var data = [];
           var barChartcategories = [{
               category:[]
           }];
           var barChartDataSet = [{
               seriesName: "Attempts",
               showValues: "1",
               data: []
           },
           {
               seriesName: "Successful Attempts",
               showValues: "1",
               data: []
           }];
           angular.forEach(response, function (item) {
               data.push({
                   label: item.ZoneName,
                   value: item.Attempts
               });
               barChartcategories[0].category.push({
                   "label": item.ZoneName
               });
               barChartDataSet[0].data.push({
                   "value": item.Attempts
               });
               barChartDataSet[1].data.push({
                   "value": item.SuccessfulAttempts
               });
           });

           var codeGroupChart = new FusionCharts({
               type: "pie3d",
               renderAt: "chartCodeGroupsContainer",
               width: "700",
               height: "500",
               dataFormat: "json",
               dataSource: {
                   chart: {
                       caption: "Top Code Group Attemps",
                       subCaption: "Attempts",
                       xAxisName: "Code Croup Name",
                       yAxisName: "Number of Attempts",
                       theme: "ocean"
                   },
                   data: data
               },
               events: {
                   dataplotClick: function (e, args) {
                       $scope.selectCodeGroup($scope.codeGroups[args.dataIndex].OurZoneID);
                   }
               }

           });
           var codeGroupColumnChart = new FusionCharts({
               type: "mscombi3d",
               renderAt: "chartCodeGroupsColumnContainer",
               width: "700",
               height: "500",
               dataFormat: "json",
               dataSource: {
                   chart: {
                       caption: "Top Code Group Attemps",
                       subCaption: "Attempts",
                       xAxisName: "Code Croup Name",
                       yAxisName: "Number of Attempts",
                       theme: "ocean"
                   },
                   categories: barChartcategories,
                   dataset: barChartDataSet
               },
               events: {
                   dataplotClick: function (e, args) {
                       $scope.selectCodeGroup($scope.codeGroups[args.dataIndex].OurZoneID);
                   }
               }

           });


           codeGroupChart.render("chartCodeGroupsContainer");
           codeGroupColumnChart.render("chartCodeGroupsColumnContainer");

       });
            
        });


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
          var data = [];
          angular.forEach(response, function (item) {
              data.push({
                  label: item.ZoneName,
                  value: item.Attempts
              });
          });



          var codeGroupChart = new FusionCharts({
              type: "pie3d",
              renderAt: "chartContainerZones",
              width: "700",
              height: "500",
              dataFormat: "json",
              dataSource: {
                  chart: {
                      caption: "Top Zones Attempts",
                      subCaption: "Attempts",
                      xAxisName: "Zone Name",
                      yAxisName: "Number of Attempts",
                      theme: "ocean"
                  },
                  "data": data
              }

          });
          codeGroupChart.render("chartContainerZones");
      });
        };
    });