(function (appControllers) {

    "use strict";

    AnalyticReportSettingsEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_Sec_ViewAPIService', 'VR_Sec_MenuAPIService','Analytic_AnalyticService'];

    function AnalyticReportSettingsEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Sec_ViewAPIService, VR_Sec_MenuAPIService, Analytic_AnalyticService) {

        var isEditMode;
        var viewTypeName = "VR_Analytic";
        var recordTypeEntity;
        var viewId;

        var searchSettingDirectiveAPI;
        var searchSettingReadyDeferred;


        var menuItems;

        var tableSelectorAPI;
        var tableSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var treeAPI;
        var treeReadyDeferred = UtilsService.createPromiseDeferred();
        var viewEntity;
        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                viewId = parameters.viewId;
            }
            isEditMode = (viewId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.selectedTables = [];

            $scope.scopeModel.onSearchSettingsDirectiveReady = function (api) {
                searchSettingDirectiveAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingSearchSettingsDirective = value };
                var payload = {
                    tableIds: tableSelectorAPI.getSelectedIds()
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, searchSettingDirectiveAPI, payload, setLoader, searchSettingReadyDeferred);
            };

            $scope.scopeModel.onTableSelectorDirectiveReady = function (api) {
                tableSelectorAPI = api;
                tableSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onTableSelectionChanged = function () {
                if (searchSettingDirectiveAPI != undefined && tableSelectorAPI != undefined) {
                    var setLoader = function (value) { $scope.scopeModel.isLoadingSearchSettingsDirective = value };
                    var payload = {
                        tableIds: tableSelectorAPI.getSelectedIds()
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, searchSettingDirectiveAPI, payload, setLoader, searchSettingReadyDeferred);
                }
            };

            $scope.scopeModel.SaveGenericBEEditor = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.widgets = [];
            $scope.scopeModel.addWidget = function () {
                var onWidgetAdd = function (widget) {
                    $scope.scopeModel.widgets.push({ widgetSettings: widget });
                };
                Analytic_AnalyticService.addWidget(onWidgetAdd, tableSelectorAPI.getSelectedIds());
            };

            $scope.scopeModel.removeWidget = function (dataItem) {
                var datasourceIndex = $scope.scopeModel.widgets.indexOf(dataItem);
                $scope.scopeModel.widgets.splice(datasourceIndex, 1);
            };
            $scope.scopeModel.isWidgetValid = function () {
                if ($scope.scopeModel.widgets.length > 0)
                    return null;
                return "At least one widget should be selected.";
            };

            $scope.widgetsGridMenuActions = function (dataItem) {
                return getwidgetGridMenuActions();
            };
           
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.onTreeReady = function (api) {
                treeAPI = api;
                treeReadyDeferred.resolve();
            };

            $scope.scopeModel.validateMenuLocation = function () {
                return ($scope.scopeModel.selectedMenuItem != undefined) ? null : 'No menu location selected';
            };
        }
        function getwidgetGridMenuActions()
        {
            var defaultMenuActions = [
              {
                  name: "Edit",
                  clicked: editWidget,
              }];
            return defaultMenuActions;
        }
        function editWidget(dataItem)
        {
            var onWidgetUpdated = function (widgetObj) {
                var index = $scope.scopeModel.widgets.indexOf(dataItem);
                $scope.scopeModel.widgets[index].widgetSettings = widgetObj;
            };
            Analytic_AnalyticService.editWidget(dataItem.widgetSettings, onWidgetUpdated, tableSelectorAPI.getSelectedIds());

        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getView().then(function () {
                        loadAllControls();
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadTree, loadTableSelector, loadSearchSettings]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });


              
            }

            function setTitle() {
                if (isEditMode && viewEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(viewEntity.Name, 'Analytic Report Editor');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Analytic Report Editor');
            }

            function loadStaticData() {
                if (viewEntity != undefined) {
                    $scope.scopeModel.reportName = viewEntity.Name;
                    $scope.scopeModel.reportTitle = viewEntity.Title;
                    if (viewEntity != undefined && viewEntity.Settings != undefined && viewEntity.Settings.Widgets != undefined && viewEntity.Settings.Widgets.length > 0) {
                        for (var i = 0 ; i < viewEntity.Settings.Widgets.length; i++) {
                            $scope.scopeModel.widgets.push({ widgetSettings: viewEntity.Settings.Widgets[i] });
                        }
                    }

                }
            }

            function loadTree() {
                var treeLoadDeferred = UtilsService.createPromiseDeferred();

                loadMenuItems().then(function () {
                    treeReadyDeferred.promise.then(function () {
                        if (viewEntity != undefined) {

                            $scope.scopeModel.selectedMenuItem = treeAPI.setSelectedNode(menuItems, viewEntity.ModuleId, "Id", "Childs");
                        }
                        treeAPI.refreshTree(menuItems);
                        treeLoadDeferred.resolve();
                    });
                }).catch(function (error) {
                    treeLoadDeferred.reject(error);
                });

                function loadMenuItems() {
                    return VR_Sec_MenuAPIService.GetAllMenuItems(true,true).then(function (response) {
                        if (response) {
                            menuItems = [];
                            for (var i = 0; i < response.length; i++) {
                                menuItems.push(response[i]);
                            }
                        }
                    });
                }

                return treeLoadDeferred.promise;

               
            }

            function loadTableSelector() {
                var loadTableSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                tableSelectorReadyDeferred.promise.then(function () {
                    var payLoad;
                    if (viewEntity != undefined && viewEntity.Settings != undefined) {
                        payLoad = {
                            selectedIds: viewEntity.Settings.AnalyticTableIds
                        }
                    }

                    VRUIUtilsService.callDirectiveLoad(tableSelectorAPI, payLoad, loadTableSelectorPromiseDeferred);
                });
                return loadTableSelectorPromiseDeferred.promise;
            }

            function loadSearchSettings() {
                if (viewEntity != undefined && viewEntity.Settings != undefined) {
                    searchSettingReadyDeferred = UtilsService.createPromiseDeferred();
                    var loadSearchSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
                    searchSettingReadyDeferred.promise.then(function () {
                        searchSettingReadyDeferred = undefined;
                        var payLoad;
                        if (viewEntity != undefined && viewEntity.Settings != undefined) {
                            payLoad = {
                                tableIds: viewEntity.Settings.AnalyticTableIds,
                                searchSettings: viewEntity.Settings.SearchSettings
                            }
                        }

                        VRUIUtilsService.callDirectiveLoad(searchSettingDirectiveAPI, payLoad, loadSearchSettingsPromiseDeferred);
                    });
                    return loadSearchSettingsPromiseDeferred.promise;

                }

            }

            function getView() {
                return VR_Sec_ViewAPIService.GetView(viewId).then(function (viewEntityObj) {
                    viewEntity = viewEntityObj;
                });
            }

        }


        function buildViewObjectFromScope() {
            var widgets = [];
            if ($scope.scopeModel.widgets != undefined && $scope.scopeModel.widgets.length > 0)
            {
                for(var i=0;i<$scope.scopeModel.widgets.length;i++)
                {
                    widgets.push($scope.scopeModel.widgets[i].widgetSettings);
                }
            }

            var viewSettings = {
                $type: "Vanrise.Analytic.Entities.AnalyticHistoryReportSettings, Vanrise.Analytic.Entities",
                AnalyticTableIds:tableSelectorAPI!=undefined?tableSelectorAPI.getSelectedIds():undefined,
                SearchSettings: searchSettingDirectiveAPI != undefined ? searchSettingDirectiveAPI.getData() : undefined,
                Widgets: widgets
            };
            var view = {
                ViewId: (viewEntity != undefined) ? viewEntity.ViewId : null,
                Name: $scope.scopeModel.reportName,
                Title: $scope.scopeModel.reportTitle,
                ModuleId: $scope.scopeModel.selectedMenuItem.Id,
                Settings: viewSettings,
                Type: viewEntity != undefined ? viewEntity.Type : undefined,

            };
            
            return view;
        }


        function insert() {
            $scope.scopeModel.isLoading = true;
            var viewEntityObj = buildViewObjectFromScope();
            viewEntityObj.ViewTypeName = viewTypeName;
            return VR_Sec_ViewAPIService.AddView(viewEntityObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Analytic Report', response, 'Name')) {
                    if ($scope.onViewAdded != undefined) {
                        $scope.onViewAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function update() {
            $scope.scopeModel.isLoading = true;
            var viewEntityObj = buildViewObjectFromScope();
            return VR_Sec_ViewAPIService.UpdateView(viewEntityObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Analytic Report', response, 'Name')) {
                    if ($scope.onViewUpdated != undefined) {
                        $scope.onViewUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

    }

    appControllers.controller('VR_Analytic_AnalyticReportSettingsEditorController', AnalyticReportSettingsEditorController);
})(appControllers);
