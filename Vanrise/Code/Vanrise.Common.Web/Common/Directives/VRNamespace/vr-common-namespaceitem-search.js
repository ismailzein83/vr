'use strict';
app.directive('vrCommonNamespaceitemSearch', ['VRUIUtilsService', 'VRNotificationService', 'VRCommon_VRNamespaceItemService',
    function (VRUIUtilsService, VRNotificationService, VRCommon_VRNamespaceItemService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VRNamespaceItemGridDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRNamespace/Templates/VRNamespaceItemSearchTemplate.html'
        };

        function VRNamespaceItemGridDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var nameSpaceId;


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.search = function () {
                    return gridAPI.load(getGridPayload());
                }

                $scope.scopeModel.addVRNamespaceItem = function () {
                    var isGridOpenedFromGridDrillDown = true;

                    var onVRNameSpaceItemAdded = function (addedVRDNameSpaceItem) {
                        gridAPI.onVRNameSpaceItemAdded(addedVRDNameSpaceItem);
                    };
                    VRCommon_VRNamespaceItemService.addVRNamespaceItem(onVRNameSpaceItemAdded, nameSpaceId, isGridOpenedFromGridDrillDown);
                };
                
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        nameSpaceId = payload.NameSpaceId;
                       return gridAPI.load(getGridPayload());
                    }
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function getGridPayload() {
                var payload = {
                    query: {
                        NameSpaceId: nameSpaceId,
                        Name: $scope.scopeModel.name
                    },
                };
                return payload;
            }
        }
    }]);
