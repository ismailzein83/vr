'use strict';

app.directive('demoModuleZoosectionGrid', ['Demo_Module_ZooSectionAPIService', 'VRNotificationService', 'Demo_Module_ZooSectionService',
    function (Demo_Module_ZooSectionAPIService, VRNotificationService, Demo_Module_ZooSectionService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope) {
                var ctrl = this;
                var zooSectionGrid = new ZooSectionGrid($scope, ctrl);
                zooSectionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/ZooSection/Directives/Templates/ZooSectionGridTemplate.html'
        };

        function ZooSectionGrid($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var zooId;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.zooSections = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Demo_Module_ZooSectionAPIService.GetFilteredZooSections(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            onResponseReady(response);
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var query;

                    if (payload != undefined) {
                        query = payload.query;
                        zooId = payload.zooId;

                        $scope.scopeModel.hideZooColumn = payload.hideZooColumn != undefined;
                    }

                    return gridAPI.retrieveData(query);
                };

                api.onZooSectionAdded = function (zooSection) {
                    gridAPI.itemAdded(zooSection);
                };

                if (ctrl.onReady != undefined && typeof ctrl.onReady == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: 'Edit',
                    clicked: editZooSection
                }];
            }

            function editZooSection(zooSection) {
                var onZooSectionUpdated = function (zooSection) {
                    gridAPI.itemUpdated(zooSection);
                };

                var zooIdItem;
                if (zooId != undefined) {
                    zooIdItem = { ZooId: zooId };
                }

                Demo_Module_ZooSectionService.editZooSection(onZooSectionUpdated, zooSection.ZooSectionId, zooIdItem);
            }
        }
    }]);