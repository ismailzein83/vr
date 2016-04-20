(function (appControllers) {

    "use strict";

    AnalyticReportSettingsEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_Sec_ViewAPIService', 'VR_Sec_MenuAPIService', 'VR_Sec_ViewTypeEnum', 'InsertOperationResultEnum'];

    function AnalyticReportSettingsEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Sec_ViewAPIService, VR_Sec_MenuAPIService, VR_Sec_ViewTypeEnum, InsertOperationResultEnum) {

        var isEditMode;
        var viewTypeName = "VR_Analytic";
        var recordTypeEntity;
        var viewId;

        //var dataRecordTypeSelectorAPI;
        //var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        


        var menuItems;

        var tableSelectorAPI;
        var tableSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var groupingDimensionSelectorAPI;
        var filterDimensionSelectorAPI;
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
            $scope.scopeModel.onTableSelectorDirectiveReady = function(api)
            {
                tableSelectorAPI = api;
                tableSelectorReadyDeferred.resolve();
            }

            $scope.scopeModel.onGroupingDimensionSelectorDirectiveReady = function (api)
            {
                groupingDimensionSelectorAPI = api;
                var setLoader = function (value) { $scope.isLoadingDimensionDirective = value };
                var payload = {
                    filter: { TableIds: tableSelectorAPI.getSelectedIds()}
                }
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, groupingDimensionSelectorAPI, payload, setLoader);
            }
            $scope.scopeModel.groupingDimensions = [];
            $scope.scopeModel.isValidGroupingDimensions = function () {

                if ($scope.scopeModel.groupingDimensions.length > 0)
                    return null;
                return "At least one dimention should be selected.";
            }
            $scope.scopeModel.onSelectGroupingDimensionItem = function (dimension) {
                var dataItem ={
                    AnalyticItemConfigId: dimension.AnalyticItemConfigId,
                    Title: dimension.Title,
                    Name: dimension.Name,
                    IsSelected :false
                };
                $scope.scopeModel.groupingDimensions.push(dataItem);
            }
            $scope.scopeModel.onDeselectGroupingDimensionItem = function (dataItem) {
                var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.groupingDimensions, dataItem.AnalyticItemConfigId, 'AnalyticItemConfigId');
                $scope.scopeModel.groupingDimensions.splice(datasourceIndex, 1);
            }
            $scope.scopeModel.removeGroupingDimension = function (dataItem) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedGroupingDimensions, dataItem.AnalyticItemConfigId, 'AnalyticItemConfigId');
                $scope.scopeModel.selectedGroupingDimensions.splice(index, 1);
                var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.groupingDimensions, dataItem.AnalyticItemConfigId, 'AnalyticItemConfigId');
                $scope.scopeModel.groupingDimensions.splice(datasourceIndex, 1);
            };


            $scope.scopeModel.filterDimensions = [];
            $scope.scopeModel.onFilterDimensionSelectorDirectiveReady = function (api) {
                filterDimensionSelectorAPI = api;
                var setLoader = function (value) { $scope.isLoadingDimensionDirective = value };
                var payload = {
                    filter: { TableIds: tableSelectorAPI.getSelectedIds() }
                }
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filterDimensionSelectorAPI, payload, setLoader);
            }
            $scope.scopeModel.isValidFilterDimensions = function () {

                if ($scope.scopeModel.filterDimensions.length > 0)
                    return null;
                return "At least one dimention should be selected.";
            }
            $scope.scopeModel.onSelectFilterDimensionItem = function (dimension) {
                var dataItem = {
                    AnalyticItemConfigId: dimension.AnalyticItemConfigId,
                    Title: dimension.Title,
                    Name: dimension.Name,
                    IsRequired: false,
                    onFieldTypeReady:function(api)
                    {
                        dataItem.fieldAPI = api;
                        var setLoader = function (value) { $scope.isLoadingDimensionDirective = value };
                        var payload;
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.fieldAPI, payload, setLoader);
                    }
                };
                $scope.scopeModel.filterDimensions.push(dataItem);
            }
            $scope.scopeModel.onDeselectFilterDimensionItem = function (dataItem) {
                var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.filterDimensions, dataItem.AnalyticItemConfigId, 'AnalyticItemConfigId');
                $scope.scopeModel.filterDimensions.splice(datasourceIndex, 1);
            }
            $scope.scopeModel.removeFilterDimension = function (dataItem) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedGroupingDimensions, dataItem.AnalyticItemConfigId, 'AnalyticItemConfigId');
                $scope.scopeModel.selectedGroupingDimensions.splice(index, 1);
                var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.filterDimensions, dataItem.AnalyticItemConfigId, 'AnalyticItemConfigId');
                $scope.scopeModel.filterDimensions.splice(datasourceIndex, 1);
            };


            $scope.scopeModel.SaveGenericBEEditor = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            //$scope.scopeModal.hasSaveGenericBEEditor = function () {
            //    if (isEditMode) {
            //        return VR_GenericData_BusinessEntityDefinitionAPIService.HasUpdateBusinessEntityDefinition();
            //    }
            //    else {
            //        return VR_GenericData_BusinessEntityDefinitionAPIService.HasAddBusinessEntityDefinition();
            //    }
            //}
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
            var viewSettings = {
                $type: "Vanrise.GenericData.Entities.GenericBEDefinitionSettings, Vanrise.GenericData.Entities",
                DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                FieldPath: $scope.scopeModel.selectedTitleFieldPath.Name,
                EditorDesign: genericEditorDesignAPI.getData(),
                ManagementDesign: {
                    GridDesign: gridFieldsDirectiveAPI.getData(),
                    FilterDesign: filterFieldsDirectiveAPI.getData()
                }
            };
            return {
                ViewId: (viewEntity != undefined) ? viewEntity.ViewId : null,
                Name: $scope.scopeModel.businessEntityName,
                Title: $scope.scopeModel.businessEntityTitle,
                ModuleId: $scope.scopeModel.selectedMenuItem.Id,
                Settings: viewSettings,
                Type: viewEntity !=undefined?viewEntity.Type:undefined,
                
            };
        }


        function insert() {
            $scope.scopeModel.isLoading = true;
            var serverResponse;
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
