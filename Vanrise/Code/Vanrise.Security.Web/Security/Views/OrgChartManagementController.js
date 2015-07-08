﻿UserManagementController.$inject = ['$scope', 'OrgChartsAPIService', 'VRModalService'];

function OrgChartManagementController($scope, OrgChartsAPIService, VRModalService) {

    var mainGridAPI;

    defineScope();
    load();

    function defineScope() {
        $scope.orgCharts = [];
        $scope.gridMenuActions = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            getData();
        };

        $scope.loadMoreData = function () {
            return getData();
        }

        $scope.filterOrgCharts = function () {
            mainGridAPI.clearDataAndContinuePaging();
            return getData();
        };

        $scope.addOrgChart = function () {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "New Organizational Chart";
                modalScope.onOrgChartAdded = function (orgChart) {
                    mainGridAPI.itemAdded(orgChart);
                };
            };

            VRModalService.showModal('/Client/Modules/Security/Views/OrgChartEditor.html', null, settings);
        }

        defineMenuActions();
    }

    function load() {

    }

    function getData() {
        var pageInfo = mainGridAPI.getPageInfo();

        var name = $scope.name != undefined ? $scope.name : '';
        return OrgChartsAPIService.GetFilteredOrgCharts(pageInfo.fromRow, pageInfo.toRow, name).then(function (response) {
            angular.forEach(response, function (item) {
                $scope.users.push(item);
            });
        });
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [
            {
                name: "Edit",
                clicked: editOrgChart,
            },
            //{
            //    name: "Delete",
            //    clicked: deleteOrgChart,
            //}
        ];
    }

    function editOrgChart(orgChartObject) {
        var modalSettings = {};

        var parameters = {
            orgChartId: orgChartObject.Id
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Organizational Chart: " + orgChartObject.Name;
            modalScope.onOrgChartUpdated = function (orgChart) {
                mainGridAPI.itemUpdated(orgChart);
            };
        };

        VRModalService.showModal('/Client/Modules/Security/Views/OrgChartEditor.html', parameters, modalSettings);
    }
}

appControllers.controller('Security_OrgChartManagementController', OrgChartManagementController);