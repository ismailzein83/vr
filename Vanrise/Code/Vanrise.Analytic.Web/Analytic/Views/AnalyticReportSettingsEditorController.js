(function (appControllers) {

    "use strict";

    AnalyticReportSettingsEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_Sec_ViewAPIService', 'VR_Sec_MenuAPIService', 'VR_Sec_ViewTypeEnum', 'InsertOperationResultEnum','Analytic_AnalyticService'];

    function AnalyticReportSettingsEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Sec_ViewAPIService, VR_Sec_MenuAPIService, VR_Sec_ViewTypeEnum, InsertOperationResultEnum, Analytic_AnalyticService) {

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
            $scope.scopeModel = {}
            $scope.scopeModel.selectedTables = [];

            $scope.scopeModel.onSearchSettingsDirectiveReady = function(api)
            {
                searchSettingDirectiveAPI = api;
                    var setLoader = function (value) { $scope.isLoadingDimensionDirective = value };
                    var payload = {
                        tableIds: tableSelectorAPI.getSelectedIds()
                    }
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, searchSettingDirectiveAPI, payload, setLoader);
            }

            $scope.scopeModel.onTableSelectorDirectiveReady = function(api)
            {
                tableSelectorAPI = api;
                tableSelectorReadyDeferred.resolve();
            }


            $scope.scopeModel.SaveGenericBEEditor = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.widgets = [];
            $scope.scopeModel.addWidget = function()
            {
                console.log($scope.scopeModel.widgets);
                var onWidgetAdd=function(widget)
                {
                    $scope.scopeModel.widgets.push({ widgetSettings: widget });
                }
                Analytic_AnalyticService.addWidget(onWidgetAdd, tableSelectorAPI.getSelectedIds());
            }


            $scope.scopeModel.isWidgetValid = function()
            {
                if ($scope.scopeModel.widgets.length > 0)
                    return null;
                return "At least one widget should be selected.";
            }


            //$scope.scopeModal.hasSaveGenericBEEditor = function () {
            //    if (isEditMode) {
            //        return VR_GenericData_BusinessEntityDefinitionAPIService.HasUpdateBusinessEntityDefinition();
            //    }
            //    else {
            //        return VR_GenericData_BusinessEntityDefinitionAPIService.HasAddBusinessEntityDefinition();
            //    }
            //}

           

            $scope.widgetsGridMenuActions = function (dataItem) {
                return getwidgetGridMenuActions();
            }

           
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
            }
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
                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadTree, loadTableSelector]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });


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

                    return treeLoadDeferred.promise;

                    function loadMenuItems() {
                        return VR_Sec_MenuAPIService.GetAllMenuItems(false).then(function (response) {
                            if (response) {
                                menuItems = [];
                                for (var i = 0; i < response.length; i++) {
                                    menuItems.push(response[i]);
                                }
                            }
                        });
                    }
                }

                function loadTableSelector() {
                    var loadTableSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                    tableSelectorReadyDeferred.promise.then(function () {
                        var payLoad;
                        if (viewEntity != undefined && viewEntity.Settings != undefined)
                        {
                            payLoad = {
                                selectedIds: viewEntity.Settings.AnalyticTableIds
                            }
                        }
                            
                        VRUIUtilsService.callDirectiveLoad(tableSelectorAPI, payLoad, loadTableSelectorPromiseDeferred);
                    });
                    return loadTableSelectorPromiseDeferred.promise;
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
                $type: "Vanrise.Analytic.Entities.AnalyticReportSettings, Vanrise.Analytic.Entities",
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
            var serverResponse;
            var viewEntityObj = buildViewObjectFromScope();
            viewEntityObj.ViewTypeName = viewTypeName; console.log(viewEntityObj);
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
