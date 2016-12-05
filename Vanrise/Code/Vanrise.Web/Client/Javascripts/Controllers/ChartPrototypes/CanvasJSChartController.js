appControllers.controller('CanvasJSChartController',
    function CanvasJSChartController($scope, $http) {

        $scope.model = "CanvasJSChartController";


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
               category: []
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
                   y: item.Attempts
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

           var codeGroupChart = new CanvasJS.Chart("chartCodeGroupsContainer", {
               theme: "theme1",
               title: {
                   text: "Fruits sold in First Quarter"
               },
               data: [
                   {
                       click: function (e) {
                       },
                       type: "pie",
                       dataPoints: data
                   }
               ]
           });

           codeGroupChart.render();

         

       });

        
    });