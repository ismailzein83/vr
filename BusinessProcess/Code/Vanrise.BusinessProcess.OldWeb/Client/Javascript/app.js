'use strict';

var bussPro = angular.module('bussPro', ['ui.grid', 'ui.grid.edit', 'ui.grid.pagination', 'ngRoute', 'mgcrea.ngStrap', 'isteven-multi-select']).
  config(['$routeProvider', '$httpProvider', function ($routeProvider, $httpProvider) {
      $routeProvider.
        when('/Main', { templateUrl: '../Client/Views/main.html', controller: MainPageCtrl }).
        when('/ListTracking/:ProcessInstanceID', { templateUrl: '../Client/Views/listtracking.html', controller: ListTrackingCtrl }).
        otherwise({ redirectTo: '/Main'});
        $httpProvider.responseInterceptors.push(function ($q, $rootScope) {
            return function (promise) {
                return promise.then(function (response) {
                    $rootScope.request = response.config;
                    return response;
                }, function (response) {
                    return $q.reject(response);
                });
            };
        });
     
  }]);
angular.module('bussPro')
.config(function ($timepickerProvider) {
    angular.extend($timepickerProvider.defaults, {
        timeFormat: 'HH:mm:ss',
        length: 7,
        minuteStep:1
    });
})
.config(function ($datepickerProvider) {
    angular.extend($datepickerProvider.defaults, {
        dateFormat: 'dd/MM/yyyy',
        startWeek: 1
    });
})