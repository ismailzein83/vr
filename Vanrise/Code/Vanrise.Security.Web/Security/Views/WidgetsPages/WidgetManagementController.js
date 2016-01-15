'use strict'
WidgetManagementController.$inject = ['$scope', 'UtilsService', 'WidgetDefinitionAPIService', 'VRNotificationService', 'VR_WidgetService'];
function WidgetManagementController($scope, UtilsService, WidgetDefinitionAPIService, VRNotificationService, VR_WidgetService) {
    var mainGridAPI;
    defineScope();
    load();

    function defineScope() {
        $scope.widgetsTypes = [];
        $scope.selectedWidgetsTypes = [];
        $scope.onGridReady = function (api) {
            mainGridAPI = api;
            var filter = {};
            api.loadGrid(filter);
        };

        $scope.Add = addNewWidget;

        $scope.searchClicked = function () {
            if (mainGridAPI != undefined)
                return mainGridAPI.loadGrid(getFilterObject());
        }

    }

    function getFilterObject() {

            var query = {
                WidgetName: $scope.widgetName,
                WidgetTypes: UtilsService.getPropValuesFromArray($scope.selectedWidgetsTypes, "ID")
            }
            return query;
    }
   
    function addNewWidget() {
        var onWidgetAdded = function (widgetObj) {
            if (mainGridAPI != undefined)
                mainGridAPI.onWidgetAdded(widgetObj);
        };
        VR_WidgetService.addWidget(onWidgetAdded);
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {

        return loadWidgets()
           .catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
               $scope.isLoading = false;
           })
          .finally(function () {
              $scope.isLoading = false;
          });
    }

    function loadWidgets() {
        return WidgetDefinitionAPIService.GetWidgetsDefinition().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.widgetsTypes.push(itm);
            });
        });

    }
    
};

appControllers.controller('Security_WidgetManagementController', WidgetManagementController);