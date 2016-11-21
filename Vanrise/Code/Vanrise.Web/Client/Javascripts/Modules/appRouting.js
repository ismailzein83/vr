'use strict';
var appRouting = angular.module('appRouting', ['ngRoute']);
appRouting.config(['$routeProvider',
  function ($routeProvider) {
      var folderPart = '';
      for (var i = 1; i <= 10; i++) {
          
          $routeProvider.
                when('/view/:module' + folderPart + '/:pagename', {
                    templateUrl: function (params) {
                        return getTemplateURLFromParams(params);
                    }
                }).
                when('/viewwithparams/:module' + folderPart + '/:pagename/:params', {
                    templateUrl: function (params) {
                        return getTemplateURLFromParams(params);
                    }
                });
          folderPart += '/:folder' + i;
      }

      function getTemplateURLFromParams(params) {
          var templateURL = '/Client/Modules/' + params.module;
          var folder = '';
          var current = 1;
          while (folder != undefined) {
              folder = params['folder' + current];
              if (folder != undefined)
                  templateURL += '/' + folder;
              current++;
          }
          templateURL += '/' + params.pagename + '.html';
          return templateURL;
      }
      $routeProvider.
          when('/Error/:params', {
              templateUrl: '/Client/Modules/Common/Views/Error.html'
          }).
        when('/default', {
            templateUrl: '/Client/Views/Default.html',
            controller: 'DefaultController'
        }).
        when('/TestView', {
            templateUrl: '/Client/Views/TestView.html',
            controller: 'TestViewController',
            controllerAs: 'testviewCtrl'
        }).
        when('/RouteRuleEditorOld', {
            templateUrl: '/Client/Views/Routing/RouteRuleEditorOld.html',
            controller: 'RouteRuleEditorControllerOld'
        }).
        when('/RouteRuleManager', {
            templateUrl: '/Client/Modules/Routing/Views/Management/RouteRuleManager.html',
            controller: 'RouteRuleManagerController'
        }).
        when('/RouteRuleEditor/:RouteRuleId', {
            templateUrl: '/Client/Modules/Routing/Views/Management/RouteRuleEditor.html',
            controller: 'RouteRuleEditorController'
        }).
            when('/RatePlanning', {
                templateUrl: '/Client/Modules/Routing/Views/Management/RatePlanning.html',
                controller: 'RatePlanningController'
            }).
            when('/RouteManager', {
                templateUrl: '/Client/Modules/Routing/Views/Management/RouteManager.html',
                controller: 'RouteManagerController'
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
        when('/BI/SeniorManagementDashboard', {
            templateUrl: '/Client/Modules/BI/Views/Dashboards/SeniorManagementDashboard.html',
            controller: 'SeniorManagementDashboardController'
        }).
        when('/BI/ZoneDashboard', {
            templateUrl: '/Client/Modules/BI/Views/Dashboards/ZoneDashboard.html',
            controller: 'ZoneDashboardController'
        }).
        when('/BI/DynamicDashboard', {
            templateUrl: '/Client/Modules/BI/Views/Dashboards/DynamicDashboard.html'
        }).
       when('/BI/EntityReport', {
           templateUrl: '/Client/Modules/BI/Views/Reports/EntityReport.html'
       }).
        when('/BI/ZoneDetails/:params', {
            templateUrl: '/Client/Modules/BI/Views/Reports/ZoneDetails.html',
            controller: 'ZoneDetailsController'
        }).
        when('/BusinessEntity/DumySwitchs', {
            templateUrl: '/Client/Modules/BusinessEntity/Views/Switchs.html',
            controller: 'SwitchsController'
        }).
        when('/BusinessEntity/Switch Managments', {
            templateUrl: '/Client/Modules/BusinessEntity/Views/SwitchManagment.html',
            controller: 'SwitchManagmentController'
        }).
          when('/SwitchEditor/:SwitchId', {
              templateUrl: '/Client/Modules/BusinessEntity/Views/SwitchEditor.html',
              controller: 'SwitchEditorController'
          }).
        when('/NOC/ZoneMonitor', {
            templateUrl: '/Client/Modules/Analytics/Views/Traffic Statistics/ZoneMonitor.html'
        }).
        when('/NOC/VariationReports', {
            templateUrl: '/Client/Modules/Analytics/Views/Billing Statistics/VariationReports.html'
        }).
        when('/NOC/BillingReports', {
            templateUrl: '/Client/Modules/Analytics/Views/Billing Reports/BillingReports.html'
        }).
          when('/Tree', {
              templateUrl: '/Client/Views/TreeView.html',
              controller: 'TreeController'
          }).
          when('/Analytics/TestPage', {
              templateUrl: '/Client/Modules/Analytics/Views/TestPage.html',
              controller: 'TestPageController'
          }).
        when('/User', {
            templateUrl: '/Client/Modules/Main/Views/UserManagement.html',
            controller: 'UserManagementController'
        }).
         when('/Role', {
             templateUrl: '/Client/Modules/Main/Views/RoleManagement.html',
             controller: 'RoleManagementController'
         }).
          when('/TestNew', {
              templateUrl: '/Client/Modules/BusinessEntity/Views/TestNew.html',
              controller: 'TestNewController'
          }).
          when('/Strategy', {
              templateUrl: '/Client/Modules/FraudAnalysis/Views/StrategyManagement.html'
          }).
          when('/Security/RoleManagement', {
              templateUrl: '/Client/Modules/Security/Views/RoleManagement.html'
          }).
          when('/Security/UserManagement', {
              templateUrl: '/Client/Modules/Security/Views/UserManagement.html'
          }).
          when('/Analytics/CDRLog', {
              templateUrl: '/Client/Modules/Analytics/Views/CDR/CDRLog.html'
          }).
          when('/BusinessEntity/OperatorAccount Managments', {
              templateUrl: '/Client/Modules/BusinessEntity/Views/OperatorAccountManagement.html'
          }).
          when('/NOC/VolumeReports', {
              templateUrl: '/Client/Modules/Analytics/Views/Volume Reports/VolumeReports.html'
          }).
        otherwise({
            redirectTo: '/default'
        });
  }]);