﻿widgetEditorController.$inject = ['$scope', 'WidgetAPIService', 'MenuAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WidgetDefinitionAPIService'];

function widgetEditorController($scope, WidgetAPIService, MenuAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService, VRUIUtilsService, WidgetDefinitionAPIService) {
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
        $scope.scopeModal = {};
        $scope.scopeModal.tabObject;
        $scope.scopeModal.onTemplateDirectiveReady = function (api)
        {
            templateDirectiveAPI = api;
            var payload;
            if (widgetEntity != undefined && widgetEntity.Setting != undefined)
                payload = widgetEntity.Setting.Settings
            var setLoader = function (value) { $scope.scopeModal.isLoadingTemplateDirective = value; };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, templateDirectiveAPI, payload, setLoader);
        }
       
        $scope.scopeModal.onPreviewAPIReady = function (api)
        {
            previewDirectiveAPI = api;
            if (buildWidgetObjFromScope())
            {
                var payload = $scope.scopeModal.widget;
                var setLoader = function (value) { $scope.scopeModal.isLoadingPreviewDirective = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, previewDirectiveAPI, payload, setLoader);
            }
           
        }

        $scope.scopeModal.widgets = [];
        $scope.scopeModal.selectedWidget;
        $scope.scopeModal.widgetName;
        $scope.scopeModal.widget;
        $scope.scopeModal.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.scopeModal.save = function () {
           
            if (!buildWidgetObjFromScope())
            {
                VRNotificationService.showWarning("Please enter all required data!!");
                return;
            }
                
            if (isEditMode)
               return updateWidget();
            else
              return addWidget();
        };

    }
  
    function buildWidgetObjFromScope() {
        if ($scope.scopeModal.selectedWidget == undefined) {
            $scope.scopeModal.widget = null;
            return false;
        }
      
            var widgetSetting = {
                settings: templateDirectiveAPI.getData(),
                directive: $scope.scopeModal.selectedWidget.DirectiveName,
            };
         
            $scope.scopeModal.widget = {
                WidgetDefinitionId: $scope.scopeModal.selectedWidget.ID,
                Name: $scope.scopeModal.widgetName,
                Setting: widgetSetting,
            };
            $scope.scopeModal.widget.onElementReady = function (api) {
                $scope.scopeModal.widget.API = api;

            };
            if ($scope.scopeModal.isEditMode) {
                $scope.scopeModal.widget.Id = $scope.scopeModal.filter.WidgetID;
            }
            return true;
            

    }

    function addWidget() {
        return WidgetAPIService.AddWidget($scope.scopeModal.widget).then(function (response) {
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

        return WidgetAPIService.UpdateWidget($scope.scopeModal.widget).then(function (response) {
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
        $scope.scopeModal.isLoading = true;
        if (isEditMode) {
            getWidget().then(function () {
                loadAllControls()
                    .finally(function () {
                    });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModal.isLoading = false;
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
               $scope.scopeModal.isLoading = false;
          });
    }

    function loadEditModeData() {
        if (isEditMode)
        {
            $scope.scopeModal.widgetName = widgetEntity.Name;
            for (var i = 0; i < $scope.scopeModal.widgets.length; i++) {
                var widget = $scope.scopeModal.widgets[i];
                if (widget.ID == widgetEntity.WidgetDefinitionId) {
                    $scope.scopeModal.selectedWidget = widget;
                }
            }
        }
    }

    function loadWidgetsDefinition() {
        return WidgetDefinitionAPIService.GetWidgetsDefinition().then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.scopeModal.widgets.push(itm);
                });
               
        });
    }

}

appControllers.controller('VR_Sec_WidgetEditorController', widgetEditorController);
