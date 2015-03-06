var appRouting = angular.module('appRouting', ['ngRoute']);
appRouting.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
        when('/default', {
            templateUrl: '/Client/Views/Default.html',
            controller: 'DefaultController'
        }).
        when('/TestView', {
            templateUrl: '/Client/Views/TestView.html',
            controller: 'TestViewController'
        }).
        when('/RouteRuleEditor', {
            templateUrl: '/Client/Views/Routing/RouteRuleEditor.html',
            controller: 'RouteRuleEditorController'
        }).
        when('/ZingChart', {
            templateUrl: '/Client/Views/ZingChart.html',
            controller: 'ZingChartController'
        }).
        when('/HighChart', {
            templateUrl: '/Client/Views/HighChart.html',
            controller: 'HighChartController'
        }).
        otherwise({
            redirectTo: '/default'
        });
  }]);