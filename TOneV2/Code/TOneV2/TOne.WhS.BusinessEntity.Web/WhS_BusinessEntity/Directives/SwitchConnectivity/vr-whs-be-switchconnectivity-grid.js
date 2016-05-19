﻿(function (app) {

    'use strict';

    SwitchConnectivityGridDirective.$inject = ['WhS_BE_SwitchConnectivityAPIService', 'WhS_BE_SwitchConnectivityService', 'VRNotificationService'];

    function SwitchConnectivityGridDirective(WhS_BE_SwitchConnectivityAPIService, WhS_BE_SwitchConnectivityService, VRNotificationService) {
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

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataSource = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_BE_SwitchConnectivityAPIService.GetFilteredSwitchConnectivities(dataRetrievalInput).then(function (response) {
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