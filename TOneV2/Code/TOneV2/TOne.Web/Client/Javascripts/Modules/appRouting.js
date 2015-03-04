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
        otherwise({
            redirectTo: '/default'
        });
  }]);