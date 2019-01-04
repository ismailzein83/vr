'use strict';
app.directive('vrCommonNamespaceitemGrid', ['VRUIUtilsService', 'VRNotificationService', 'VRCommon_VRNamespaceItemService', 'VRCommon_VRNamespaceItemAPIService',
    function (VRUIUtilsService, VRNotificationService, VRCommon_VRNamespaceItemService, VRCommon_VRNamespaceItemAPIService) {
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
            templateUrl: '/Client/Modules/Common/Directives/VRNamespace/Templates/VRNamespaceItemGridTemplate.html'
        };

        function VRNamespaceItemGridDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var nameSpaceId;


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.vrNamespaceItems = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_VRNamespaceItemAPIService.GetFilteredVRNamespaceItems(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var query;
                    if (payload != undefined) {
                        nameSpaceId = payload.query.NameSpaceId;
                        query = payload.query;
                    }
                    return gridAPI.retrieveData(query);
                };

                api.onVRNameSpaceItemAdded = function (addedVRDNameSpaceItem) {
                    gridAPI.itemAdded(addedVRDNameSpaceItem);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editVRNamespaceItem
                });
            }

            function editVRNamespaceItem(vrNameSpaceItemObj) {
                var isGridOpenedFromGridDrillDown = true;

                var onVRNameSpaceItemUpdated = function (updatedVRNameSpaceItem) {
                    gridAPI.itemUpdated(updatedVRNameSpaceItem);
                };

                VRCommon_VRNamespaceItemService.editVRNamespaceItem(onVRNameSpaceItemUpdated, vrNameSpaceItemObj.VRNamespaceItemId, nameSpaceId, isGridOpenedFromGridDrillDown);
            }

        }
    }]);
