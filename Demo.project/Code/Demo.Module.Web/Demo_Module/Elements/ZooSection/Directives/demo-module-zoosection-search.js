'use strict';

app.directive('demoModuleZoosectionSearch', ['Demo_Module_ZooSectionService',
    function (Demo_Module_ZooSectionService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ZooSectionSearch($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/ZooSection/Directives/Templates/ZooSectionSearchTemplate.html'
        };

        function ZooSectionSearch($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var zooId;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.addZooSection = function () {
                    var zooIdItem = { ZooId: zooId };
                    var onZooSectionAdded = function (obj) {
                        gridAPI.onZooSectionAdded(obj);
                    };

                    Demo_Module_ZooSectionService.addZooSection(onZooSectionAdded, zooIdItem);
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        zooId = payload.zooId;
                    }

                    return gridAPI.load(getGridPayload());
                };

                api.onZooSectionAdded = function (zooSectionObject) {
                    gridAPI.onZooSectionAdded(zooSectionObject);
                };

                if (ctrl.onReady != undefined && typeof ctrl.onReady == 'function')
                    ctrl.onReady(api);
            }

            function getGridPayload() {
                return {
                    query: { ZooIds: [zooId] },
                    zooId: zooId,
                    hideZooColumn: true
                };
            }
        }
    }]);