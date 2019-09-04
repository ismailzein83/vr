'use strict';

app.directive('demoModuleZooGrid', ['Demo_Module_ZooAPIService', 'VRNotificationService', 'Demo_Module_ZooService', 'ZooSizeEnum', 'UtilsService', 'VRUIUtilsService',
    function (Demo_Module_ZooAPIService, VRNotificationService, Demo_Module_ZooService, ZooSizeEnum, UtilsService, VRUIUtilsService) {
        return {
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
            var gridDrillDownTabsObj;

            function intializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.zoos = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(buildDrillDownDefinitions(), gridAPI);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Demo_Module_ZooAPIService.GetFilteredZoos(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            var zoos = response.Data;
                            var nbOfZoos = zoos.length;

                            for (var i = 0; i < nbOfZoos; i++) {
                                var currentZoo = zoos[i];
                                currentZoo.sizeDescription = UtilsService.getEnumDescription(ZooSizeEnum, currentZoo.Size, 'value');
                                gridDrillDownTabsObj.setDrillDownExtensionObject(currentZoo);
                            }
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
                    gridDrillDownTabsObj.setDrillDownExtensionObject(zoo);
                    gridAPI.itemAdded(zoo);
                };

                if (ctrl.onReady != undefined && typeof ctrl.onReady == 'function')
                    ctrl.onReady(api);
            }

            function buildDrillDownDefinitions() {
                var drillDownDefinitions = [];
                drillDownDefinitions.push(buildChildDrillDownDefinitions());
                return drillDownDefinitions;
            }

            function buildChildDrillDownDefinitions() {
                var drillDownDefinition = {};

                drillDownDefinition.title = 'Section';
                drillDownDefinition.directive = 'demo-module-zoosection-search';

                drillDownDefinition.loadDirective = function (directiveAPI, zooItem) {
                    zooItem.zooSectionGridAPI = directiveAPI;
                    var payload = {
                        zooId: zooItem.ZooId
                    };
                    return zooItem.zooSectionGridAPI.load(payload);
                };

                return drillDownDefinition;
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
    }]);