appControllers.controller('AMChartController',
    function AMChartController($scope, $http) {

        $scope.model = "AMChartController";
        var codeGroups = null;
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
           codeGroups = response;
           

           var codeGroupChart = new AmCharts.AmPieChart();
           codeGroupChart.dataProvider = codeGroups;
           codeGroupChart.titleField = "ZoneName";
           codeGroupChart.valueField = "Attempts";
           codeGroupChart.outlineColor = "#FFFFFF";
           codeGroupChart.outlineAlpha = 0.8;
           codeGroupChart.outlineThickness = 2;
           codeGroupChart.balloonText = "[[title]]<br><span style='font-size:14px'><b>[[value]]</b> ([[percents]]%)</span>";
           // this makes the chart 3D
           codeGroupChart.depth3D = 15;
           codeGroupChart.angle = 30;
           codeGroupChart.clickSlice = function (e) {
               $scope.selectCodeGroup(e.dataContext.OurZoneID);
           };
           // WRITE
           codeGroupChart.write("chartCodeGroupsContainer");           

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
          var zones = response;

          var zoneChart = new AmCharts.AmPieChart();
          zoneChart.dataProvider = zones;
          zoneChart.titleField = "ZoneName";
          zoneChart.valueField = "Attempts";
          zoneChart.outlineColor = "#FFFFFF";
          zoneChart.outlineAlpha = 0.8;
          zoneChart.outlineThickness = 2;
          zoneChart.balloonText = "[[title]]<br><span style='font-size:14px'><b>[[value]]</b> ([[percents]]%)</span>";
          // this makes the chart 3D
          zoneChart.depth3D = 15;
          zoneChart.angle = 30;
          // WRITE
          zoneChart.write("chartZonesContainer");
      });
        };
    });