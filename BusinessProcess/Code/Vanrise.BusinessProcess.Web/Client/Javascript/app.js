'use strict';

var bussPro = angular.module('bussPro', ['ngRoute', 'ngGrid', 'ngQuickDate']).
  config(['$routeProvider', '$httpProvider', function ($routeProvider, $httpProvider) {
      $routeProvider.
        when('/Main', { templateUrl: '../Client/Views/main.html', controller: MainPageCtrl }). 
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

bussPro.config(function (ngQuickDateDefaultsProvider) {
    return ngQuickDateDefaultsProvider.set({
    });
});