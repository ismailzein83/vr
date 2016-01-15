'use strict'
WidgetManagementController.$inject = ['$scope', 'UtilsService', 'WidgetDefinitionAPIService', 'VRModalService', 'VRNotificationService', 'DeleteOperationResultEnum', 'VR_WidgetService'];
function WidgetManagementController($scope, UtilsService, WidgetDefinitionAPIService, VRModalService, VRNotificationService, DeleteOperationResultEnum, VR_WidgetService) {
    var mainGridAPI;
    defineScope();
    load();

    function defineScope() {
        $scope.widgetsTypes = [];
        $scope.selectedWidgetsType;
        $scope.onGridReady = function (api) {
            mainGridAPI = api;
            var filter = {};
            api.loadGrid(filter);
        };
        

        $scope.mainGridPagerSettings = {
            currentPage: 1,
            totalDataCount: 0,
            pageChanged: function () {
                return getData();
            }
        };

        $scope.Add = function () {
            addNewWidget();
        };
        $scope.searchClicked = function () {
            if (mainGridAPI != undefined)
                return mainGridAPI.loadGrid(getFilterObject());
        }

    }
    function getFilterObject() {
            var widgetType;
            if ($scope.selectedWidgetsType != undefined)
                widgetType = $scope.selectedWidgetsType.ID;
            else
                widgetType = 0;
            var query = {
                WidgetName: $scope.widgetName,
                WidgetType: widgetType
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