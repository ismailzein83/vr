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
            controller: 'TestViewController',
            controllerAs : 'testviewCtrl'
        }).
        when('/RouteRuleEditorOld', {
            templateUrl: '/Client/Views/Routing/RouteRuleEditorOld.html',
            controller: 'RouteRuleEditorControllerOld'
        }).
        when('/RouteRuleManager', {
            templateUrl: '/Client/Views/Routing/RouteRuleManager.html',
            controller: 'RouteRuleManagerController'
        }).
        when('/RouteRuleEditor/:RouteRuleId', {
            templateUrl: '/Client/Views/Routing/RouteRuleEditor.html',
            controller: 'RouteRuleEditorController'
        }).
        when('/RouteRuleEditor2/:RouteRuleId', {
            templateUrl: '/Client/Views/Routing/RouteRuleEditor2.html',
            controller: 'RouteRuleEditorController2',
            controllerAs: 'RouteRuleEditorController2'
        }).
        when('/ZingChart', {
            templateUrl: '/Client/Views/ChartPrototypes/ZingChart.html',
            controller: 'ZingChartController'
        }).
        when('/HighChart', {
            templateUrl: '/Client/Views/ChartPrototypes/HighChart.html',
            controller: 'HighChartController'
        }).
        when('/HighChartSparkline', {
            templateUrl: '/Client/Views/ChartPrototypes/HighChartSparkline.html',
            controller: 'HighChartSparklineController'
        }).
        when('/FusionChart', {
            templateUrl: '/Client/Views/ChartPrototypes/FusionChart.html',
            controller: 'FusionChartController'
        }).
        when('/CanvasJSChart', {
            templateUrl: '/Client/Views/ChartPrototypes/CanvasJSChart.html',
            controller: 'CanvasJSChartController'
        }).
        when('/AMChart', {
            templateUrl: '/Client/Views/ChartPrototypes/AMChart.html',
            controller: 'AMChartController'
        }).
        when('/BI/TopManagementDashboard', {
            templateUrl: '/Client/Modules/BI/Views/Dashboards/TopManagementDashboard.html',
            controller: 'TopManagementDashboardController'
        }).
        otherwise({
            redirectTo: '/default'
        });
  }]);