'use strict'

var TreeController = function ($scope) {
 
    $scope.tree =  [
        {
            label: "Routing", location: '', icon: 'glyphicon-certificate', children: [
               { label: "Rule Management", location: '#/RouteRuleManager',children:[] },
               { label: "Route Manager", location: '' }
            ]
        },
        {
            label: "BI", location: '', icon: 'glyphicon-cog', children: [
               { label: "Top Management Dashboard", location: '#/BI/TopManagementDashboard' },
               { label: "Senior Management Dashboard", location: '#/BI/SeniorManagementDashboard' },
               { label: "Top Destination Dashboard", location: '#/BI/ZoneDashboard' }
            ]
        },
        {
            label: "NOC", location: '', icon: 'glyphicon-flash', children: [
               { label: "Zone Monitor", location: '#/NOC/ZoneMonitor' }
            ]
        },
        {
            label: "Others", icon: 'glyphicon-pencil', location: '', children: [
               { label: "Default", location: '#/Default' },
                { label: "Test View", location: '#/TestView' },
                { label: "ZingChart", location: '#/ZingChart' },
                { label: "HighChart", location: '#/HighChart' },
                { label: "HighChartSparkline", location: '#/HighChartSparkline' },
                { label: "FusionChart", location: '#/FusionChart' },
                { label: "CanvasJSChart", location: '#/CanvasJSChart' },
                { label: "AMChart", location: '#/AMChart' },
                { label: "Tree", location: '#/Tree' }
            ]
        }
        ]
}

TreeController.$inject = ['$scope'];

appControllers.controller('TreeController', TreeController);