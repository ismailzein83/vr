"use strict";
(function (appControllers) {
    TreeController.$inject = ['$scope', '$rootScope'];
    var TreeController = function ($scope, $rootScope) {

        $scope.tree = [
            {
                label: "Routing", location: '', icon: 'glyphicon-certificate', children: [
                   { label: "Rule Management", location: '#/RouteRuleManager', children: [] },
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
                   { label: "Traffic Monitor", location: '#/NOC/TrafficMonitor' }
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
        ];
        $scope.$on("tree View-show", function (e) {
            $rootScope.hisnav.push({
                name: 'tree View',
                show: true
            });
        });

        $scope.$on("tree View", function (e) {

        });

    };
    appControllers.controller('TreeController', TreeController);
})(appControllers)
