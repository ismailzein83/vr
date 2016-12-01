'use strict';

app.directive('retailBeSwitchGrid', ['Retail_BE_SwitchAPIService', 'Retail_BE_SwitchService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService',
    function (Retail_BE_SwitchAPIService, Retail_BE_SwitchService, UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var switchGrid = new RetailBeSwitchGrid($scope, ctrl, $attrs);
                switchGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Switch/Templates/SwitchGridTemplate.html'
        };

        function RetailBeSwitchGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.switches = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_BE_SwitchAPIService.GetFilteredSwitches(dataRetrievalInput).then(function (response) {
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

                api.onSwitchAdded = function (addedSwitch) {
                    gridAPI.itemAdded(addedSwitch);
                };

                api.onSwitchUpdated = function (updatedSwitch) {
                    gridAPI.itemUpdated(updatedSwitch);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editSwitch,
                });
            }

            function editSwitch(switchItem) {
                var onSwitchUpdated = function (updatedSwitch) {
                    gridAPI.itemUpdated(updatedSwitch);
                };

                Retail_BE_SwitchService.editSwitch(switchItem.Entity.SwitchId, onSwitchUpdated);
            }
        }
    }]);
