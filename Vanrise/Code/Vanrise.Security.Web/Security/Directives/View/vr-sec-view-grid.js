"use strict";

app.directive("vrSecViewGrid", ['VRNotificationService', 'VR_Sec_ViewAPIService', 'VR_Sec_ViewService', 'VRModalService', 'VR_Sec_ViewTypeAPIService', 'UtilsService', 'VRUIUtilsService', function (VRNotificationService, VR_Sec_ViewAPIService, VR_Sec_ViewService, VRModalService, VR_Sec_ViewTypeAPIService, UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var viewsGrid = new ViewsGrid($scope, ctrl, $attrs);
            viewsGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Security/Directives/View/Templates/ViewGridTemplate.html"

    };

    function ViewsGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;

        function initializeController() {
            ctrl.viewTypes = [];
            $scope.views = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var drillDownDefinitions = VR_Sec_ViewService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        var promises = [];
                        var loadViewTypes = VR_Sec_ViewTypeAPIService.GetViewTypes().then(function (response) {
                            ctrl.viewTypes.length = 0;
                            ctrl.viewTypes = response;
                        });
                        promises.push(loadViewTypes);
                        promises.push(gridAPI.retrieveData(query));
                        return UtilsService.waitMultiplePromises(promises);
                    };
                    directiveAPI.onViewAdded = function (viewObj) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(viewObj);
                        gridAPI.itemAdded(viewObj);
                    };
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Sec_ViewAPIService.GetFilteredViews(dataRetrievalInput)
                    .then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editView,
                haspermission: hasUpdateViewPermission // System Entities:Assign Permissions
            }];
        }
        function hasUpdateViewPermission() {
            return VR_Sec_ViewAPIService.HasUpdateViewPermission();
        }
        function editView(viewObj) {
            var viewType = UtilsService.getItemByVal(ctrl.viewTypes, viewObj.Entity.Type, "ExtensionConfigurationId");

            var modalParameters = {
                viewId: viewObj.Entity.ViewId,
                viewType: viewType
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onViewUpdated = function (viewObj) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(viewObj);
                    gridAPI.itemUpdated(viewObj);
                };
            };
            VRModalService.showModal(viewType.Editor, modalParameters, modalSettings);

        }
    }

    return directiveDefinitionObject;

}]);