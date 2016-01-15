WidgetEditorController.$inject = ['$scope', 'WidgetAPIService', 'MenuAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService','VRUIUtilsService','WidgetDefinitionAPIService'];

function WidgetEditorController($scope, WidgetAPIService, MenuAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService, VRUIUtilsService, WidgetDefinitionAPIService) {
    var widgetId;
    var widgetEntity;
    var templateDirectiveAPI;
    var previewDirectiveAPI;
    var isEditMode;
    loadParameters();
    defineScope();
    load();


    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != undefined && parameters != null) {
            widgetId = parameters.Id;
        }
        isEditMode = (widgetId != undefined);
    }

    function defineScope() {

        $scope.tabObject;
        $scope.onTemplateDirectiveReady= function(api)
        {
            templateDirectiveAPI = api;
            var payload;
            if (widgetEntity != undefined && widgetEntity.Setting != undefined)
                payload = widgetEntity.Setting.Settings
            var setLoader = function (value) { $scope.isLoadingTemplateDirective = value; };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, templateDirectiveAPI, payload, setLoader);
        }
       
        $scope.onPreviewAPIReady = function(api)
        {
            previewDirectiveAPI = api;
            if (buildWidgetObjFromScope())
            {
                var payload = $scope.widget;
                var setLoader = function (value) { $scope.isLoadingPreviewDirective = value; };

                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, previewDirectiveAPI, payload, setLoader);
            }
           
        }
        $scope.widgets = [];
        $scope.selectedWidget;
        $scope.widgetName;
        $scope.widget;
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
      
            var widgetSetting = {
                settings: templateDirectiveAPI.getData(),
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
        $scope.isLoading = true;
        if (isEditMode) {
            getWidget().then(function () {
                loadAllControls()
                    .finally(function () {
                    });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            });
        }
        else {
            $scope.title = UtilsService.buildTitleForAddEditor("Widget");
            loadAllControls();
        }

    }

    function getWidget()
    {
        return WidgetAPIService.GetWidgetById(widgetId).then(function (widget) {
            widgetEntity = widget;
            $scope.title = UtilsService.buildTitleForUpdateEditor(widgetEntity.Name, "Widget");
        });
    }

    function loadAllControls() {
        return loadWidgetsDefinition().then(function () {
            loadEditModeData();
        }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
           }).finally(function () {
              $scope.isLoading = false;
          });
    }

    function loadEditModeData() {
        if (isEditMode)
        {
            $scope.widgetName = widgetEntity.Name;
            for (var i = 0; i < $scope.widgets.length; i++) {
                var widget = $scope.widgets[i];
                if (widget.ID == widgetEntity.WidgetDefinitionId) {
                    $scope.selectedWidget = widget;
                }
            }
        }
    }

    function loadWidgetsDefinition() {
        return WidgetDefinitionAPIService.GetWidgetsDefinition().then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.widgets.push(itm);
                });
               
        });
    }

}

appControllers.controller('Security_WidgetEditorController', WidgetEditorController);
