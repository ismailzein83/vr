(function (app) {

    'use strict';

    OrgChartGridDirective.$inject = ['VR_Sec_OrgChartAPIService', 'VR_Sec_OrgChartService', 'VRNotificationService'];

    function OrgChartGridDirective(VR_Sec_OrgChartAPIService, VR_Sec_OrgChartService, VRNotificationService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var orgChartGrid = new OrgChartGrid($scope, ctrl, $attrs);
                orgChartGrid.initialize();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/Security/Directives/OrgChart/Templates/OrgChartGrid.html'
        };

        function OrgChartGrid($scope, ctrl, $attrs) {
            this.initialize = initialize;

            var gridAPI;

            function initialize() {
                ctrl.orgCharts = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady && typeof ctrl.onReady == 'function')
                        ctrl.onReady(getDirectiveAPI());
                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Sec_OrgChartAPIService.GetFilteredOrgCharts(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editOrgChart
                }, {
                    name: 'Delete',
                    clicked: deleteOrgChart
                }];
            }

            function getDirectiveAPI() {
                var directiveAPI = {};

                directiveAPI.loadGrid = function (query) {
                    return gridAPI.retrieveData(query);
                };

                directiveAPI.onOrgChartAdded = function (addedOrgChart) {
                    gridAPI.itemAdded(addedOrgChart);
                };

                return directiveAPI;
            }

            function editOrgChart(orgChart) {
                var onOrgChartUpdated = function (updatedOrgChart) {
                    gridAPI.itemUpdated(updatedOrgChart);
                };
                VR_Sec_OrgChartService.editOrgChart(orgChart.OrgChartId, onOrgChartUpdated);
            }

            function deleteOrgChart(orgChart) {
                var onOrgChartDeleted = function () {
                    gridAPI.itemDeleted(orgChart);
                };
                VR_Sec_OrgChartService.deleteOrgChart($scope, orgChart.OrgChartId, onOrgChartDeleted);
            }
        }

        return directiveDefinitionObject;
    }

    app.directive('vrSecOrgchartGrid', OrgChartGridDirective);

})(app);
