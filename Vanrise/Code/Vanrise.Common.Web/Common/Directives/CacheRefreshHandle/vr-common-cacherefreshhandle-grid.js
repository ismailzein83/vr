"use strict";

app.directive("vrCommonCacherefreshhandleGrid", ["UtilsService", "VRNotificationService", "VRCommon_CacheRefreshhandleAPIService",
function (UtilsService, VRNotificationService, VRCommon_CacheRefreshhandleAPIService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new CacheRefreshHandleGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Common/Directives/CacheRefreshHandle/Templates/CacheRefreshHandleGridTemplate.html"

    };

    function CacheRefreshHandleGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.cacheRefreshHandles = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {

                        return gridAPI.retrieveData(query);
                    };

                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_CacheRefreshhandleAPIService.GetFilteredCacheRefreshHandles(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }



        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Set Cache Expired",
                clicked: setCacheExpired,
                haspermission: hasSetCacheExpiredPermission
            }];
        }

        function setCacheExpired(dataItem) {
            gridAPI.showLoader();
            var onItemUpdated = function (updatedObject) {                
                gridAPI.itemUpdated(updatedObject);
            };
            return VRCommon_CacheRefreshhandleAPIService.SetCacheExpired(dataItem.TypeName).then(function (response) {               
                if (VRNotificationService.notifyOnItemAdded("Cache Refresh Handle", response)) {
                    onItemUpdated(response.UpdatedObject);                    
                }
            }).finally(function () {
                gridAPI.hideLoader();
            });
        }

        function hasSetCacheExpiredPermission() {
            return VRCommon_CacheRefreshhandleAPIService.HasSetCacheExpiredPermission();
        }
    }

    return directiveDefinitionObject;

}]);
