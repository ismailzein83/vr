(function (app) {

    'use strict';

    SwitchConnectivityGridDirective.$inject = ['WhS_BE_SwitchConnectivityAPIService', 'WhS_BE_SwitchConnectivityService', 'VRNotificationService', 'VRUIUtilsService'];

    function SwitchConnectivityGridDirective(WhS_BE_SwitchConnectivityAPIService, WhS_BE_SwitchConnectivityService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var switchConnectivityGrid = new SwitchConnectivityGrid($scope, ctrl, $attrs);
                switchConnectivityGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/SwitchConnectivity/Templates/SwitchConnectivityGridTemplate.html'
        };

        function SwitchConnectivityGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataSource = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = WhS_BE_SwitchConnectivityService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.menuActions, true);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_BE_SwitchConnectivityAPIService.GetFilteredSwitchConnectivities(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);

                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onSwitchConnectivityAdded = function (addedSwitchConnectivity) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedSwitchConnectivity);
                    gridAPI.itemAdded(addedSwitchConnectivity);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editSwitchConnectivity,
                    haspermission: hasEditPermission
                }];

                function editSwitchConnectivity(dataItem) {
                    var onSwitchConnectivityUpdated = function (updatedSwitchConnectivity) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(updatedSwitchConnectivity);
                        gridAPI.itemUpdated(updatedSwitchConnectivity);
                    };
                    WhS_BE_SwitchConnectivityService.editSwitchConnectivity(dataItem.Entity.SwitchConnectivityId, onSwitchConnectivityUpdated);
                }

                function hasEditPermission() {
                    return WhS_BE_SwitchConnectivityAPIService.HasEditSwitchConnectivityPermission();
                }
            }
        }
    }

    app.directive('vrWhsBeSwitchconnectivityGrid', SwitchConnectivityGridDirective);

})(app);