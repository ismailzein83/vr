(function (appControllers) {

    "use strict";

    AnalyticReportViewEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_Sec_ViewAPIService', 'VR_Sec_MenuAPIService', 'Analytic_AnalyticService', 'VRLocalizationService'];

    function AnalyticReportViewEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Sec_ViewAPIService, VR_Sec_MenuAPIService, Analytic_AnalyticService, VRLocalizationService) {

        var isEditMode;
        var viewTypeName = "VR_AnalyticReport";
        var recordTypeEntity;
        var viewId;

        var reportDirectiveAPI;
        var reportReadyDeferred = UtilsService.createPromiseDeferred();;

        var menuItems;

        var reportTypeSelectorAPI;
        var reportTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var reportTypeSelectorSelectionChangePromise;

        var viewCommonPropertiesAPI;
        var viewCommonPropertiesReadyDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.onReportSelectorDirectiveReady = function (api) {
                reportDirectiveAPI = api;
                reportReadyDeferred.resolve();
            };

            $scope.scopeModel.onReportTypeSelectorDirectiveReady = function (api) {
                reportTypeSelectorAPI = api;
                reportTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onViewCommonPropertiesReady = function (api) {
                viewCommonPropertiesAPI = api;
                viewCommonPropertiesReadyDeferred.resolve();
            };

            $scope.scopeModel.onReportTypeSelectionChanged = function (selectedItem) {
                if (selectedItem != undefined) {

                    if (reportTypeSelectorSelectionChangePromise != undefined) {
                        reportTypeSelectorSelectionChangePromise.resolve();
                    }
                    else {

                        var setLoader = function (value) { $scope.scopeModel.isLoadingReportDirective = value; };
                        reportReadyDeferred = undefined;
                        var payLoad = {
                            filter: {
                                TypeId: reportTypeSelectorAPI.getSelectedIds()
                            },
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, reportDirectiveAPI, payLoad, setLoader, reportReadyDeferred);
                    }
                }
            };

            $scope.scopeModel.SaveView = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
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
                    reportTypeSelectorSelectionChangePromise = UtilsService.createPromiseDeferred();
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
                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadTree, loadReportTypeSelector, loadReportSelector, loadViewCommonProperties]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                    reportTypeSelectorSelectionChangePromise = undefined;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });



            }

            function setTitle() {

                if (isEditMode && viewEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(viewEntity.Name, 'Analytic Report View Editor');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Analytic Report View Editor');
            }

            function loadStaticData() {

                if (viewEntity != undefined) {
                    $scope.scopeModel.reportName = viewEntity.Name;
                    $scope.scopeModel.reportTitle = viewEntity.Title;
                    if (viewEntity != undefined && viewEntity.Settings != undefined && viewEntity.Settings != undefined) {
                        //for (var i = 0 ; i < viewEntity.Settings.Widgets.length; i++) {
                        //    $scope.scopeModel.widgets.push({ widgetSettings: viewEntity.Settings.Widgets[i] });
                        //}
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
                    return VR_Sec_MenuAPIService.GetAllMenuItems(true, true).then(function (response) {
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

            function loadReportTypeSelector() {

                var loadReportTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                reportTypeSelectorReadyDeferred.promise.then(function () {
                    var payLoad;
                    if (viewEntity != undefined && viewEntity.Settings != undefined) {
                        payLoad = {
                            selectedIds: viewEntity.Settings.TypeId
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(reportTypeSelectorAPI, payLoad, loadReportTypeSelectorPromiseDeferred);
                });
                return loadReportTypeSelectorPromiseDeferred.promise;
            }

            function loadReportSelector() {

                if (viewEntity != undefined && viewEntity.Settings != undefined) {
                    var loadReportPromiseDeferred = UtilsService.createPromiseDeferred();
                    reportReadyDeferred.promise.then(function () {
                        var payLoad;
                        if (viewEntity != undefined && viewEntity.Settings != undefined) {
                            payLoad = {
                                filter: {
                                    TypeId: viewEntity.Settings.TypeId
                                },
                                selectedIds: viewEntity.Settings.AnalyticReportId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(reportDirectiveAPI, payLoad, loadReportPromiseDeferred);
                    });
                    return loadReportPromiseDeferred.promise;
                }

            }
            function loadViewCommonProperties() {
                var viewCommmonPropertiesLoadDeferred = UtilsService.createPromiseDeferred();
                viewCommonPropertiesReadyDeferred.promise.then(function () {
                    var payload = {};
                    if (viewEntity != undefined) {
                        payload.viewEntity = viewEntity;
                    }
                    VRUIUtilsService.callDirectiveLoad(viewCommonPropertiesAPI, payload, viewCommmonPropertiesLoadDeferred);
                });
                return viewCommmonPropertiesLoadDeferred.promise;

            }

            function getView() {
                return VR_Sec_ViewAPIService.GetView(viewId).then(function (viewEntityObj) {
                    viewEntity = viewEntityObj;
                });
            }

        }

        function buildViewObjectFromScope() {

            var viewSettings = {
                $type: "Vanrise.Analytic.Entities.AnalyticReportViewSettings, Vanrise.Analytic.Entities",
                AnalyticReportId: reportDirectiveAPI != undefined ? reportDirectiveAPI.getSelectedIds() : undefined,
                TypeId: reportTypeSelectorAPI != undefined ? reportTypeSelectorAPI.getSelectedIds() : undefined,
            };
            viewCommonPropertiesAPI.setCommonProperties(viewSettings);
            var view = {
                ViewId: (viewEntity != undefined) ? viewEntity.ViewId : null,
                Name: $scope.scopeModel.reportName,
                Title: $scope.scopeModel.reportTitle,
                ModuleId: ($scope.scopeModel.selectedMenuItem != undefined) ? $scope.scopeModel.selectedMenuItem.Id : undefined,
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
                if (VRNotificationService.notifyOnItemAdded('Analytic Report View', response, 'Name')) {
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
                if (VRNotificationService.notifyOnItemUpdated('Analytic Report View', response, 'Name')) {
                    if ($scope.onViewUpdated != undefined) {
                        $scope.onViewUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

    }

    appControllers.controller('VR_Analytic_AnalyticReportViewEditorController', AnalyticReportViewEditorController);
})(appControllers);
