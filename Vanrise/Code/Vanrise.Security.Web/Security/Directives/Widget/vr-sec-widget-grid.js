"use strict";

app.directive("vrSecWidgetGrid", ["VRNotificationService", "VR_Sec_WidgetAPIService", "VR_Sec_WidgetService",
function (VRNotificationService, VR_Sec_WidgetAPIService, VR_Sec_WidgetService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var ctor = new ctorGrid($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Security/Directives/Widget/Templates/WidgetsGrid.html"

    };

    function ctorGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            defineMenuActions();

            ctrl.widgets = [];

            ctrl.onMainGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onWidgetAdded = function (WidgetObject) {
                        gridAPI.itemAdded(WidgetObject);
                    };
                    return directiveAPI;
                }
            };

            ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Sec_WidgetAPIService.GetFilteredWidgets(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });;
            };

        }

        function defineMenuActions() {
            ctrl.menuActions = [{
                name: "Edit",
                clicked: editWidget,
                haspermission: hasUpdateWidgetPermission
            }, {
                name: "Delete",
                clicked: deleteWidget,
                haspermission: hasDeleteWidgetPermission
            }];

        }
        function hasUpdateWidgetPermission() {
            return VR_Sec_WidgetAPIService.HasUpdateWidgetPermission();
        }
        function hasDeleteWidgetPermission() {
            return VR_Sec_WidgetAPIService.HasDeleteWidgetPermission();
        }
        function editWidget(dataItem) {
            var onWidgetUpdated = function (updatedItem) {
                gridAPI.itemUpdated(updatedItem);
            };
            VR_Sec_WidgetService.updateWidget(dataItem.Entity.Id, onWidgetUpdated);
        }

        function deleteWidget(dataItem) {
            var onWidgetDeleted = function (deletedItem) {
                gridAPI.itemDeleted(deletedItem);
            };

            VR_Sec_WidgetService.deleteWidget($scope, dataItem, onWidgetDeleted);
        }

    }

    return directiveDefinitionObject;

}]);