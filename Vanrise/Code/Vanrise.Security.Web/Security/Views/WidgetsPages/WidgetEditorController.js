WidgetEditorController.$inject = ['$scope', 'WidgetAPIService', 'MenuAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function WidgetEditorController($scope, WidgetAPIService, MenuAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
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
        $scope.widgetName;
        $scope.widget;
        
        $scope.subViewConnector = {};
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.save = function () {
            if (!buildWidgetObjFromScope())
            {
                VRNotificationService.showWarning("Please enter all required data!!");
                return;
            }
                
            if ($scope.isEditMode)
               return updateWidget();
            else
              return addWidget();
        };
        $scope.previewSelectionChanged = function () {
            buildWidgetObjFromScope();
        }

    }
  
    function buildWidgetObjFromScope() {
        if ($scope.selectedWidget == undefined) {
            $scope.widget = null;
            return false;
        }
          
        if ($scope.subViewConnector.getValue() == false) {
            $scope.widget = null;
            return false;
        }
      
            var widgetSetting = {
                settings: $scope.subViewConnector.getValue(),
                directive: $scope.selectedWidget.DirectiveName,
            };
         
            $scope.widget = {
                WidgetDefinitionId: $scope.selectedWidget.ID,
                Name: $scope.widgetName,
                Setting: widgetSetting,
            };
            $scope.widget.onElementReady = function (api) {
                $scope.widget.API = api;

            };
            if ($scope.isEditMode) {
                $scope.widget.Id = $scope.filter.WidgetID;
            }
            return true;
            

    }

    function addWidget() {
        return WidgetAPIService.AddWidget($scope.widget).then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Widget", response, "Name")) {
                    if ($scope.onWidgetAdded != undefined)
                        $scope.onWidgetAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function updateWidget() {

            return WidgetAPIService.UpdateWidget($scope.widget).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Widget", response, "Name")) {
                    if ($scope.onWidgetUpdated != undefined)
                        $scope.onWidgetUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    function load() {
        $scope.isGettingData = true;
        $scope.isInitializing = true;
        UtilsService.waitMultipleAsyncOperations([loadWidgets]).then(function () {
            if ($scope.isEditMode == true) {
                loadEditModeData();
            }
        }).finally(function () {
               
                $scope.isInitializing = false;
                $scope.isGettingData = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });

    }

    function loadEditModeData() {
        $scope.widgetName = $scope.filter.WidgetName;
            for (var i = 0; i < $scope.widgets.length; i++) {
                var widget = $scope.widgets[i];
                if (widget.ID == $scope.filter.selectedWidget) {
                    $scope.selectedWidget = widget;
                    $scope.subViewConnector.value = $scope.filter.Setting.Settings;
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

    }

appControllers.controller('Security_WidgetEditorController', WidgetEditorController);
