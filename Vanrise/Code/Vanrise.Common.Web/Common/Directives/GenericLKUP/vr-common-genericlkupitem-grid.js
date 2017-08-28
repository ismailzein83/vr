'use strict';

app.directive('vrCommonGenericlkupitemGrid', ['VR_Common_GnericLKUPAPIService', 'VR_Common_GenericLKUPService', 'VRNotificationService', 'VRUIUtilsService',
    function (VR_Common_GnericLKUPAPIService, VR_Common_GenericLKUPService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',

            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericLKUPItemGrid = new GenericLKUPItemGrid($scope, ctrl, $attrs);
                genericLKUPItemGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/GenericLKUP/Templates/GenericLKUPItemGrid.html'
        };

        function GenericLKUPItemGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.genericLKUP = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VR_Common_GenericLKUPService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Common_GnericLKUPAPIService.GetFilteredGenericLKUPItems(dataRetrievalInput).then(function (response) {
                        if (response.Data != undefined) {
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

                api.onGenericLKUPAdded = function (addedGenericLKUP) {
                    gridAPI.itemAdded(addedGenericLKUP);
                };

                api.onGenericLKUPUpdated = function (updatedGenericLKUP) {
                    gridAPI.itemUpdated(updatedGenericLKUP);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editGenericLKUPItem,
                    haspermission:hasEditGenericLKUPPermission

                });
            }


            function editGenericLKUPItem(genericLKUPItem) {

                var onGenericLKUPUpdated = function (updatedGenericLKUPItem) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedGenericLKUPItem);
                    gridAPI.itemUpdated(updatedGenericLKUPItem);
                };

                VR_Common_GenericLKUPService.editGenericLKUP(genericLKUPItem.Entity.GenericLKUPItemId, onGenericLKUPUpdated);
            }

            function hasEditGenericLKUPPermission() {
                return VR_Common_GnericLKUPAPIService.HasEditGenericLKUPItemPermission();
            }
        }
    }]);
