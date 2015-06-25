WidgetsEditorController.$inject = ['$scope','WidgetAPIService', 'MenuAPIService','BIVisualElementService1', 'BIConfigurationAPIService', 'ChartSeriesTypeEnum', 'DynamicPagesManagementAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function WidgetsEditorController($scope,WidgetAPIService, MenuAPIService, BIVisualElementService1, BIConfigurationAPIService, ChartSeriesTypeEnum, DynamicPagesManagementAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {
    //var mainGridAPI;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
    }
    function defineScope() {
        $scope.widgets = [];
        $scope.selectedWidget;
        $scope.WidgetName;
        $scope.visualElement;
        $scope.subViewValue = {};
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        //$scope.filter = {};
        $scope.save = function () {

            $scope.Widget = {
                WidgetDefinitionId: $scope.filter.WidgetDefinitionId,
                Name: $scope.filter.WidgetName,
                Setting: $scope.filter.visualElement,
            };
            console.log( $scope.filter.WidgetName);
            return WidgetAPIService.SaveWidget($scope.Widget).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Widget", response)) {
                    console.log(response);
                    if ($scope.onWidgetAdded != undefined)
                        $scope.onWidgetAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        };
        $scope.addVisualElement = function () {
            
            $scope.subViewValue = $scope.subViewValue.getValue();
            var visualElement = {
                settings: $scope.subViewValue,
                directive: $scope.selectedWidget.directiveName,
            };
            visualElement.onElementReady = function (api) {
                visualElement.API = api;
            };
            $scope.visualElement = visualElement;
       
            $scope.filter = {
                WidgetDefinitionId: $scope.selectedWidget.ID,
                WidgetName: $scope.WidgetName,
                visualElement: $scope.visualElement,
            }
            $scope.selectedWidget = null;


        };
        $scope.removeVisualElement = function (visualElement) {
            $scope.visualElement = null;
        };
    }

    function load() {
        defineChartSeriesTypes();
        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadWidgets]).finally(function () {
            $scope.isInitializing = false;
            $scope.isGettingData = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });

    }

   

    function loadWidgets() {
        return DynamicPagesManagementAPIService.GetWidgets().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.widgets.push(itm);
            });
        });

    }
    function defineChartSeriesTypes() {
        $scope.chartSeriesTypes = [];
        for (var m in ChartSeriesTypeEnum) {
            $scope.chartSeriesTypes.push(ChartSeriesTypeEnum[m]);
        }
    }

  

}
appControllers.controller('Security_WidgetsEditorController', WidgetsEditorController);
