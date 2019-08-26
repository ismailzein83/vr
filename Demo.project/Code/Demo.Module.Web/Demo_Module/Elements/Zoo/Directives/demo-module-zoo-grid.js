'use strict';

app.directive('demoModuleZooGrid', ['Demo_Module_ZooAPIService', 'VRNotificationService', 'Demo_Module_ZooService', 'ZooSizeEnum', 'UtilsService',
    function zooManagementController(Demo_Module_ZooAPIService, VRNotificationService, Demo_Module_ZooService, ZooSizeEnum, UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope) {
                var ctrl = this;
                var zooGrid = new ZooGrid($scope, ctrl);
                zooGrid.intializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/Zoo/Directives/Templates/ZooGridTemplate.html'
        };

        function ZooGrid($scope, ctrl) {
            this.intializeController = intializeController;

            var gridAPI;

            function intializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.zoos = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Demo_Module_ZooAPIService.GetFilteredZoos(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            var zoos = response.Data;
                            var nbOfZoos = zoos.length;

                            for (var i = 0; i < nbOfZoos; i++)
                                zoos[i].sizeDescription = UtilsService.getEnumDescription(ZooSizeEnum, zoos[i].Size, 'value');
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    return gridAPI.retrieveData(payload);
                };

                api.onZooAdded = function (zoo) {
                    gridAPI.itemAdded(zoo);
                };

                if (ctrl.onReady != undefined && typeof ctrl.onReady == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: 'Edit',
                    clicked: editZoo
                }];
            }

            function editZoo(zoo) {
                var onZooUpdated = function (zoo) {
                    zoo.sizeDescription = UtilsService.getEnumDescription(ZooSizeEnum, zoo.Size, 'value');
                    gridAPI.itemUpdated(zoo);
                };

                Demo_Module_ZooService.editZoo(onZooUpdated, zoo.ZooId);
            }
        }

        return directiveDefinitionObject;
    }]);