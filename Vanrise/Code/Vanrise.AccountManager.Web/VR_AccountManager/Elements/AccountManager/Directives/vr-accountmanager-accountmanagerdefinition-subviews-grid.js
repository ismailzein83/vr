"use strict";

app.directive("vrAccountmanagerAccountmanagerdefinitionSubviewsGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_AccountManager_AccountManagerService",
function (UtilsService, VRNotificationService, VRUIUtilsService, VR_AccountManager_AccountManagerService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var subViewsGrid = new SubViewsGrid($scope, ctrl, $attrs);
            subViewsGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/VR_AccountManager/Elements/AccountManager/Directives/Template/AccountManagerSubViewsGrid.html'
    };

    function SubViewsGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        var gridAPI;
        var context;
        function initializeController() {
            $scope.subViews = [];
            var subviewsArray;
            $scope.addNewSubView = function () {
                var onSubViewnAdded = function (subView) {
                    $scope.subViews.push( {Entity:subView} );
                    subviewsArray = $scope.subViews;
                };
                VR_AccountManager_AccountManagerService.addSubView(onSubViewnAdded, context);
            };
            $scope.removerow = function (dataItem) {
                        var index = $scope.subViews.indexOf(dataItem);
                        $scope.subViews.splice(index, 1);
            s};
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveAPI());
                }
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (payload) {
                        
                        if (payload != undefined) {
                            if (payload.context != undefined) {
                                context = payload.context;
                            }
                            if (payload.subViews != undefined) {
                                for (var i = 0; i < payload.subViews.length; i++) {
                                    var subView = payload.subViews[i];
                                    $scope.subViews.push({ Entity: subView });
                                }
                            }
                        }
                    };
                    directiveAPI.getData = function () {
                        return getSubviewsData();
                    };
                    return directiveAPI;
                };

            };
            defineMenuActions();
        }
        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editSubView,
            }];
        }
        function getSubviewsData() {
            var subViews = [];
            if ($scope.subViews != undefined) {
                for (var i = 0; i < $scope.subViews.length; i++) {
                    var subView = $scope.subViews[i];
                    subViews.push(subView.Entity);
                }
            }
            return subViews;
        }
        
        function editSubView(subViewObject) {
            var onSubViewUpdated = function (subView) {
                var index = $scope.subViews.indexOf(subViewObject);
                $scope.subViews[index] = { Entity: subView };
            };
            VR_AccountManager_AccountManagerService.editSubView(subViewObject.Entity, onSubViewUpdated,$scope.subViews, context);
        }
    }
    return directiveDefinitionObject;
}]);