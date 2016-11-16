widgetEditorController.$inject = ['$scope', 'VR_Sec_WidgetAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_Sec_WidgetDefinitionAPIService'];

function widgetEditorController($scope, VR_Sec_WidgetAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Sec_WidgetDefinitionAPIService) {
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
        $scope.scopeModal.onTemplateDirectiveReady = function (api) {
            templateDirectiveAPI = api;
            var payload;
            if (widgetEntity != undefined && widgetEntity.Setting != undefined)
                payload = widgetEntity.Setting.Settings;
            var setLoader = function (value) {
                $scope.scopeModal.isLoadingTemplateDirective = value;
            };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, templateDirectiveAPI, payload, setLoader);
        };

        $scope.scopeModal.onPreviewAPIReady = function (api) {
            previewDirectiveAPI = api;
            if (buildWidgetObjFromScope()) {
                var payload = $scope.scopeModal.widget;
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingPreviewDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, previewDirectiveAPI, payload, setLoader);
            }

        };
        $scope.scopeModal.buildWidgetObjFromScope = function () {
            return buildWidgetObjFromScope();
        };
        $scope.scopeModal.widgets = [];
        $scope.scopeModal.selectedWidget;
        $scope.scopeModal.widgetName;
        $scope.scopeModal.widget;
        $scope.scopeModal.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.scopeModal.save = function () {

            if (!buildWidgetObjFromScope()) {
                VRNotificationService.showWarning("Please enter all required data!!");
                return;
            }
            $scope.scopeModal.isLoading = true;
            if (isEditMode)
                return updateWidget();
            else
                return addWidget();
        };


        $scope.scopeModal.hasSaveWidgetPermission = function () {
            if (isEditMode) {
                return VR_Sec_WidgetAPIService.HasUpdateWidgetPermission();
            }
            else {
                return VR_Sec_WidgetAPIService.HasAddWidgetPermission();
            }

        }

    }

    function buildWidgetObjFromScope() {
        if ($scope.scopeModal.selectedWidget == undefined || templateDirectiveAPI == undefined) {
            $scope.scopeModal.widget = null;
            return false;
        }

        var settings = templateDirectiveAPI.getData();
        for (var prop in settings)
        {

            if (settings[prop] === undefined && prop != 'TimeEntity')
            {
                return false;
            }
                
        }
        var widgetSetting = {
            settings: settings,
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
        return VR_Sec_WidgetAPIService.AddWidget($scope.scopeModal.widget)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Widget", response, "Name")) {
                    if ($scope.onWidgetAdded != undefined)
                        $scope.onWidgetAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });
    }

    function updateWidget() {
        $scope.scopeModal.widget.Id = widgetId;
        return VR_Sec_WidgetAPIService.UpdateWidget($scope.scopeModal.widget)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Widget", response, "Name")) {
                    if ($scope.onWidgetUpdated != undefined)
                        $scope.onWidgetUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });
    }

    function load() {
        $scope.scopeModal.isLoading = true;
        if (isEditMode) {
            getWidget()
                .then(function () {
                    loadAllControls()
                        .finally(function () { });
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
        } else {
            $scope.title = UtilsService.buildTitleForAddEditor("Widget");
            loadAllControls();
        }

    }

    function getWidget() {
        return VR_Sec_WidgetAPIService.GetWidgetById(widgetId)
            .then(function (widget) {
                widgetEntity = widget;
                $scope.title = UtilsService.buildTitleForUpdateEditor(widgetEntity.Name, "Widget");
            });
    }

    function loadAllControls() {
        return loadWidgetsDefinition()
            .then(function () {
                loadEditModeData();
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.scopeModal.isLoading = false;
            });
    }

    function loadEditModeData() {
        if (isEditMode) {
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
        return VR_Sec_WidgetDefinitionAPIService.GetWidgetsDefinition()
            .then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.scopeModal.widgets.push(itm);
                });

            });
    }

}

appControllers.controller('VR_Sec_WidgetEditorController', widgetEditorController);