'use strict';


var app = angular.module('mainModule', ['appControllers', 'appRouting'])
.controller('mainCtrl', function mainCtrl($scope,notify) {
    var pathArray = location.href.split('/');
    var protocol = pathArray[0];
    var host = pathArray[2];
    $scope.baseurl = protocol + '//' + host;
    $scope.carrierAccountSelectionOption = 1;
    
    $scope.findExsite = function (arr, value, attname) {
        var index = -1;
        for (var i = 0; i < arr.length; i++) {
            if (arr[i][attname] == value) {
                index = i
            }
        }
        return index;
    }
    $scope.findExsiteObj = function (arr, value, attname) {
        var obj = null;
        for (var i = 0; i < arr.length; i++) {
            if (arr[i][attname] == value) {
                obj = arr[i];
            }
        }
        return obj;
    }
    var numberReg = /^\d+$/;
    $scope.isNumber = function (s) {
        return String(s).search(numberReg) != -1
    };
    $scope.dateToString = function (date) {
        var dateString = '';
        if (date) {
            try{
                var day = "" + (parseInt(date.getDate()));
                if (day.length == 1)
                    dateString += "0" + day;
                else
                    dateString += day;
                var month = "" + (parseInt(date.getMonth()) + 1);
                if (month.length == 1)
                    dateString += "/0" + month;
                else
                    dateString += "/" + month;
                dateString += "/" + date.getFullYear();
            }
            catch (e) {

            }
           
        }
        return dateString;
    }
    var dateReg = /^(0?[1-9]|[12][0-9]|3[01])\/(0?[1-9]|1[012])\/((199\d)|([2-9]\d{3}))$/;
    $scope.isDate = function (s) {
        var res = String(s).search(dateReg) != -1
        return res;
    }

});
angular.module('mainModule')
.config(function ($timepickerProvider) {
    angular.extend($timepickerProvider.defaults, {
        timeFormat: 'HH:mm:ss',
        length: 7,
        minuteStep: 1
    });
})
.config(function ($datepickerProvider) {
    angular.extend($datepickerProvider.defaults, {
        dateFormat: 'dd/MM/yyyy',
        startWeek: 1
    });
})
.config(function ($popoverProvider) {
    angular.extend($popoverProvider.defaults, {
        animation: 'am-flip-x',
        trigger: 'hover',
        autoClose:true,
        delay: { show:1 , hide: 100000 }
    });
})
