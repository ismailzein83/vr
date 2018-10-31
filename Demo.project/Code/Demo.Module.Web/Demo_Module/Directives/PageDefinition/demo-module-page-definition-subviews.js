"use strict"
app.directive("demoModulePageDefinitionSubviews", ["UtilsService", "VRNotificationService", "Demo_Module_PageDefinitionService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_PageDefinitionService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var pageDefinitionSubviewsGrid = new PageDefinitionSubviewsGrid($scope, ctrl, $attrs);
            pageDefinitionSubviewsGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/PageDefinition/Templates/PageDefinitionSubviewsGridTemplate.html"
    };

    function PageDefinitionSubviewsGrid($scope, ctrl) {

        var gridApi;
        $scope.scopeModel = {};
        $scope.scopeModel.pageDefinitionSubviews = [];

        this.initializeController = initializeController;

            function initializeController() {
            
                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveApi());
                    }

                    function getDirectiveApi() {

                        var directiveApi = {};

                        directiveApi.load = function (payload) {
                            if (payload != undefined && payload.Subviews != undefined) {
                                for (var i = 0; i < payload.Subviews.length; i++) {
                                    var subview = payload.Subviews[i];
                                    $scope.scopeModel.pageDefinitionSubviews.push(subview);
                                }
                            }
                        };

                        directiveApi.getData = function () {
                            var subviews = [];
                            for (var j = 0; j < $scope.scopeModel.pageDefinitionSubviews.length; j++) {
                                var subview = $scope.scopeModel.pageDefinitionSubviews[j];
                                subviews.push(subview);
                            }
                            return { Subviews: subviews };
                        }

                        return directiveApi;
                    };
                };

                $scope.scopeModel.onPageDefinitionSubviewAdded = function () {

                    var onPageDefinitionSubviewAdded = function (pageDefinitionSubview) {
                        $scope.scopeModel.pageDefinitionSubviews.push(pageDefinitionSubview);
                    };

                    Demo_Module_PageDefinitionService.addPageDefinitionSubview(onPageDefinitionSubviewAdded);
                };

                defineMenuActions();
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editPageDefinitionSubview,
                }];
            }

            function editPageDefinitionSubview(PageDefinitionSubview) {

                var index = $scope.scopeModel.pageDefinitionSubviews.indexOf(PageDefinitionSubview);
                var onPageDefinitionSubviewUpdated = function (pageDefinitionSubview) {
                    $scope.scopeModel.pageDefinitionSubviews[index] = pageDefinitionSubview;
                };

                Demo_Module_PageDefinitionService.editPageDefinitionSubview(onPageDefinitionSubviewUpdated, PageDefinitionSubview);
            }

        }

        return directiveDefinitionObject;
   
}]);

