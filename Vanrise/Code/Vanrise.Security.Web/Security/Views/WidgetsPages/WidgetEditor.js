WidgetsEditorController.$inject = ['$scope', 'WidgetAPIService', 'MenuAPIService', 'BIVisualElementService1', 'BIConfigurationAPIService', 'ChartSeriesTypeEnum', 'DynamicPagesAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function WidgetsEditorController($scope, WidgetAPIService, MenuAPIService, BIVisualElementService1, BIConfigurationAPIService, ChartSeriesTypeEnum, DynamicPagesAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {
    loadParameters();
    defineScope();
    load();
    
    function loadParameters() {
        console.log(parameters);
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != null) {
            $scope.filter = {
                WidgetID: parameters.Id,
                WidgetName: parameters.Name,
                selectedWidget: parameters.WidgetDefinitionId,
                Setting: parameters.Setting
            }
            $scope.isEditMode = true;
        }
        else
        $scope.isEditMode = false;
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
        $scope.save = function () {
            if ($scope.isEditMode)
                updateWidget();
             else
            saveWidget();
        };
        $scope.previewSelectionChanged = function () {
            console.log("test");
            var visualElement = {
                settings: $scope.subViewValue.getValue(),
                directive: $scope.selectedWidget.DirectiveName,
            };

            var Widget = {
                WidgetDefinitionId: $scope.selectedWidget.ID,
                Name: $scope.WidgetName,
                Setting: visualElement,
            };
            $scope.visualElement = Widget;
        }
 
    }
   
    function saveWidget() {
        var visualElement = {
            settings: $scope.subViewValue.getValue(),
            directive: $scope.selectedWidget.DirectiveName,
        };
      
        $scope.Widget = {
            WidgetDefinitionId: $scope.selectedWidget.ID,
            Name: $scope.WidgetName,
            Setting: visualElement,
        };

        $scope.Widget.onElementReady = function (api) {
            $scope.Widget.API = api;
        };
        return WidgetAPIService.SaveWidget($scope.Widget).then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Widget", response)) {
                if ($scope.onWidgetAdded != undefined)
                    $scope.onWidgetAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }
    function updateWidget() {

        var visualElement = {
            settings: $scope.subViewValue.getValue(),
            directive: $scope.selectedWidget.DirectiveName,
        };
      
        $scope.Widget = {
            WidgetDefinitionId: $scope.selectedWidget.ID,
            Name: $scope.WidgetName,
            Setting: visualElement,
        };
        $scope.Widget.Id = $scope.filter.WidgetID;

        $scope.Widget.onElementReady = function (api) {
            $scope.Widget.API = api;
        };
        return WidgetAPIService.UpdateWidget($scope.Widget).then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Widget", response)) {
                console.log(response);
                if ($scope.onWidgetUpdated != undefined)
                    $scope.onWidgetUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }

    function load() {
        defineChartSeriesTypes();
        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadWidgets]).finally(function () {
            if ($scope.isEditMode == true) {
                loadEditModeData();
            }
            $scope.isInitializing = false;
            $scope.isGettingData = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });

    }
    function loadEditModeData() {
        $scope.WidgetName = $scope.filter.WidgetName;
        for (var i = 0; i < $scope.widgets.length; i++) {
            if ($scope.widgets[i].ID == $scope.filter.selectedWidget) {
                $scope.selectedWidget = $scope.widgets[i];
                $scope.subViewValue.value = $scope.filter.Setting.Settings;
            }
        }
    }
    function loadWidgets() {
        return WidgetAPIService.GetWidgetsDefinition().then(function (response) {
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
