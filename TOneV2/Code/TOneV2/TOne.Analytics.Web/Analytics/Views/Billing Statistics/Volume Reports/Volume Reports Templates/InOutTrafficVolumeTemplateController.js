InOutTrafficVolumeTemplateController.$inject = ['$scope', 'BillingStatisticsAPIService', 'UtilsService', 'VRNotificationService'];
function InOutTrafficVolumeTemplateController($scope, BillingStatisticsAPIService, UtilsService, VRNotificationService) {

    var durationChartAPI;
    var amountChartAPI;
    var durationFlag = false;
    var amountFlag = false;
    defineScope();
    load();


    function defineScope() {

        $scope.durationChartReady = function (api) {
            durationChartAPI = api;
            if (durationFlag) {
                if (!filter.showChartsInPie)
                    updateDurationChart($scope.durationData, $scope.filter.timePeriod);
                else updateDurationPie($scope.durationData, $scope.filter.timePeriod);
            }
        };

        $scope.amountChartReady = function (api) {
            amountChartAPI = api;
            console.log(durationFlag);
            if (durationFlag) {
                if (!filter.showChartsInPie)
                    updateAmountChart($scope.durationData, $scope.filter.timePeriod);
                else updateAmountPie($scope.durationData, $scope.filter.timePeriod);
            }
        };

        $scope.subViewResultInOutTraffic.getData = function (filter) {
            $scope.filter = filter;
            //return UtilsService.waitMultipleAsyncOperations([getDurations, getAmounts])
            //.catch(function (error) {
            //    VRNotificationService.notifyExceptionWithClose(error, $scope);
            //});
            return getDurations();
        }
    }
    function load() {
    }

    function getDurations() {
        var filter = $scope.filter;
        return BillingStatisticsAPIService.CompareInOutTraffic(filter.fromDate, filter.toDate, filter.requiredCustomerId, filter.timePeriod, filter.showChartsInPie)
             .then(function (response) {
                 $scope.durationData = response;
                 if (durationChartAPI != undefined) {
                     if (!filter.showChartsInPie) {
                         console.log(filter.showChartsInPie);
                         updateDurationChart($scope.durationData, filter.timePeriod);

                     }
                     else {
                         console.log(filter.showChartsInPie);
                         updateDurationPie($scope.durationData, filter.timePeriod);
                     }
                 }
                 if (amountChartAPI != undefined) {
                     if (!filter.showChartsInPie) {
                         console.log(filter.showChartsInPie);
                         updateAmountChart($scope.durationData, filter.timePeriod);
                     }
                     else { console.log(filter.showChartsInPie);  updateAmountPie($scope.durationData, filter.timePeriod); }
                 }
                 durationFlag = true;
             });
    }

    //function getAmounts() {
    //    var filter = $scope.filter;
    //    return BillingStatisticsAPIService.CompareInOutTraffic(filter.fromDate, filter.toDate, filter.customerId, filter.supplierId, filter.zoneId, filter.attempts, filter.timePeriod, filter.topDestination, false)
    //           .then(function (response) {
    //               $scope.amountData = response;
    //               if (amountChartAPI != undefined)
    //                   updateAmountChart($scope.amountData);
    //               amountFlag = true;
    //           });
    //}

    function updateDurationChart(data,timePeriod) {
   
      
        var chartDefinition = {
            type: "column",
            title: "In/Out Traffic Volumes -Duration",
            yAxisTitle: "Duration"
        };
        var result = [];
        var seriesDefinitions = [];

        var xAxisDefinition = { titlePath: "xValue" };
   
        var dates = [];
    

   
        if (timePeriod == 0)
            dates = [""];
        else {
            for (var i = 0; i < data.length; i++)
                if(i==0)
                    dates.push(data[i].Date);
                else {
                    var j= i-1;
                    if (data[i].Date != data[j].Date)
                    dates.push(data[i].Date);
                }
        }
      

        angular.forEach(dates, function (itm) {
          
            result.push({
                xValue: itm,
                Values: []
            });
        });
   
 

        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
    
            seriesDefinitions.push({
                title: dataItem.TrafficDirection,
                valuePath: "Values[" + i + "]",
                type: "column" //areaspline 
            });
            for (var j = 0; j < dates.length ; j++) {
               

                if (dataItem.Date == result[j].xValue)
                    result[j].Values[i] = dataItem.Duration;
                else result[j].Values[i] = 0;
             
            }
        }
        

     
        durationChartAPI.renderChart(result, chartDefinition, seriesDefinitions, xAxisDefinition);

    }
    function updateAmountChart(data,timePeriod) {
       
        var chartDefinition2 = {
            type: "column",
            title: "In/Out Traffic Volumes -Net Sale Amounts",
            yAxisTitle: "Amounts"
        };
      
        var result2 = [];
        var seriesDefinitions2 = [];

        var xAxisDefinition2 = { titlePath: "xValue" };
        var dates = [];
      
        if (timePeriod == 0)
            dates = [""];
        else {
            for (var i = 0; i < data.length; i++)
                if (i == 0)
                    dates.push(data[i].Date);
                else {
                    var j = i - 1;
                    if (data[i].Date != data[j].Date)
                        dates.push(data[i].Date);
                }
        }
      
        angular.forEach(dates, function (itm) {
            result2.push({
                xValue: itm,
                Values: []
            });
        });
      
        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
            seriesDefinitions2.push({
                title: dataItem.TrafficDirection,
                valuePath: "Values[" + i + "]",
                type: "column" //areaspline 
            });
            for (var j = 0; j < dates.length ; j++) {
             
                if (dataItem.Date == result2[j].xValue)
                    result2[j].Values[i] = dataItem.Net;
                else result2[j].Values[i] = 0;
            }
        }


     
      
       
        amountChartAPI.renderChart(result2, chartDefinition2, seriesDefinitions2, xAxisDefinition2);
    }

    function updateDurationPie(data, timePeriod) {
             console.log("Piee");
             console.log(data);
              var chartDefinition = {
            type: "pie",
            title: "In/Out Traffic Volumes -Duration",
            yAxisTitle: "Duration"
        };
        var result = [];
        var seriesDefinitions = [];

       // var xAxisDefinition = { titlePath: "xValue" };
        //   console.log(xAxisDefinition);
  //      var dates = [];
        //var period = data[0].Date.length;
        //  console.log(timePeriod);
   //     if (timePeriod == 0)
   //         dates = [""];
    //    else {
   //         for (var i = 0; i < data.length; i++)
   //             if (i == 0)
   //                 dates.push(data[i].Date);
   //             else {
   //                 var j = i - 1;
   //                 if (data[i].Date != data[j].Date)
   //                     dates.push(data[i].Date);
   //             }
   //     }
 //       console.log(dates);
      //  console.log(data.length);

        //angular.forEach(dates, function (itm) {
        //    console.log("item in results");
        //    console.log(itm);
        //    result.push({
        //        xValue: itm,
        //        Values: []
        //    });
        //});
  //      console.log("resultt1");
  //      console.log(result);
  //      console.log(result[1]);
        // for (var i = 0; i < data.length ; i++) {
        //     dates.push(data[0].Date)
        // }

        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
            //   console.log(dataItem);
            seriesDefinitions.push({
                title: dataItem.TrafficDirection,
                titlePath: "TrafficDirection",
                valuePath: "value"
               // type: "column" //areaspline 
            });
        //    for (var j = 0; j < dates.length ; j++) {
        //        //     console.log(dataItem.Duration);

        //        if (dataItem.Date == result[j].xValue)
        //            result[j].Values[i] = dataItem.Duration;
        //        else result[j].Values[i] = 0;
        //        //     console.log("test");
        //        //}
        //    }
        }

         var chartData = [];
         angular.forEach(data, function (itm) {
            var dataItem = {
                 groupKeyValues: [],
                 entityName: itm.TrafficDirection,
                 value: itm.Duration
              };
            dataItem.groupKeyValues.push(itm.Duration);
             //for (var i = 0; i < $scope.currentSearchCriteria.groupKeys.length; i++) {
             //    if (dataItem.entityName.length > 0)
             //        dataItem.entityName += ' - ';
             //    dataItem.entityName += itm.GroupKeyValues[i].Name;
             //};
            chartData.push(dataItem);
        //    console.log(dataItem);
          //   othersValue -= itm.Data[measure.propertyName];
         });
         console.log(chartDefinition);
         console.log(seriesDefinitions);
         console.log(chartData);
         durationChartAPI.renderSingleDimensionChart(chartData, chartDefinition, seriesDefinitions);

    }
    function updateAmountPie(data, timePeriod) {
        console.log("Piee");
        console.log(data);
        var chartDefinition2 = {
            type: "pie",
            title: "In/Out Traffic Volumes -Net Sale Amounts",
            yAxisTitle: "Amounts"
        };

       // var result2 = [];
        var seriesDefinitions2 = [];

        //var xAxisDefinition2 = { titlePath: "xValue" };
        //var dates = [];

        //if (timePeriod == 0)
        //    dates = [""];
        //else {
        //    for (var i = 0; i < data.length; i++)
        //        if (i == 0)
        //            dates.push(data[i].Date);
        //        else {
        //            var j = i - 1;
        //            if (data[i].Date != data[j].Date)
        //                dates.push(data[i].Date);
        //        }
        //}

        //angular.forEach(dates, function (itm) {
        //    result2.push({
        //        xValue: itm,
        //        Values: []
        //    });
        //});

        for (var i = 0; i < data.length; i++) {
            var dataItem = data[i];
            seriesDefinitions2.push({
                title: dataItem.TrafficDirection,
                titlePath: "TrafficDirection",
                valuePath: "value"
            });
        }
            //for (var j = 0; j < dates.length ; j++) {
            //    // result2[0].Values[i] = dataItem.Net;
            //    if (dataItem.Date == result2[j].xValue)
            //        result2[j].Values[i] = dataItem.Net;
            //    else result2[j].Values[i] = 0;
            //}
            var chartData = [];
            angular.forEach(data, function (itm) {
                var dataItem = {
                    groupKeyValues: [],
                    entityName: itm.TrafficDirection,
                    value: itm.Net
                };
                dataItem.groupKeyValues.push(itm.Net);
                chartData.push(dataItem);
               
            });
            
            console.log(chartDefinition2);
            console.log(seriesDefinitions2);
            console.log(chartData);
            amountChartAPI.renderSingleDimensionChart(chartData, chartDefinition2, seriesDefinitions2);
    }


};
appControllers.controller('Analytics_InOutTrafficVolumeTemplateController', InOutTrafficVolumeTemplateController);
