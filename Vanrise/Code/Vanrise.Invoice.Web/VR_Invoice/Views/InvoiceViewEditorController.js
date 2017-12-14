(function (appControllers) {

    "use strict";

    InvoiceViewEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_Sec_ViewAPIService', 'VR_Sec_MenuAPIService', 'VRLocalizationService'];

    function InvoiceViewEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Sec_ViewAPIService, VR_Sec_MenuAPIService, VRLocalizationService) {

        var isEditMode;
        var viewTypeName = "VR_Invoice_GenericInvoice";
        var invoiceTypeEntity;
        var viewId;

        var menuItems;

        var invoiceTypeSelectorAPI;
        var invoiceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var treeAPI;
        var treeReadyDeferred = UtilsService.createPromiseDeferred();
        
        var viewCommonPropertiesAPI;
        var viewCommonPropertiesReadyDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.onInvoiceTypeSelectorDirectiveReady = function (api) {
                invoiceTypeSelectorAPI = api;
                invoiceTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.SaveView = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.onViewCommonPropertiesReady = function (api) {
                viewCommonPropertiesAPI = api;
                viewCommonPropertiesReadyDeferred.resolve();
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

            function insert() {
                $scope.scopeModel.isLoading = true;
                var viewEntityObj = buildViewObjectFromScope();
                viewEntityObj.ViewTypeName = viewTypeName;
                return VR_Sec_ViewAPIService.AddView(viewEntityObj).then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded('Invoice View', response, 'Name')) {
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
                    if (VRNotificationService.notifyOnItemUpdated('Invoice View', response, 'Name')) {
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
            
            function buildViewObjectFromScope() {

                var viewSettings = {
                    $type: "Vanrise.Invoice.Entities.InvoiceViewSettings, Vanrise.Invoice.Entities",
                    InvoiceTypeId: invoiceTypeSelectorAPI != undefined ? invoiceTypeSelectorAPI.getSelectedIds() : undefined,
                };
                    viewCommonPropertiesAPI.setCommonProperties(viewSettings);
                var view = {
                    ViewId: (viewEntity != undefined) ? viewEntity.ViewId : null,
                    Name: $scope.scopeModel.invoiceName,
                    Title: $scope.scopeModel.invoiceTitle,
                    ModuleId: $scope.scopeModel.selectedMenuItem != undefined ? $scope.scopeModel.selectedMenuItem.Id : undefined,
                    Settings: viewSettings,
                    Type: viewEntity != undefined ? viewEntity.Type : undefined,

                };

                return view;
            }
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

                function setTitle() {
                    if (isEditMode && viewEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(viewEntity.Name, 'Invoice View Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Invoice View Editor');
                }

                function loadStaticData() {
                    if (viewEntity != undefined) {
                        $scope.scopeModel.invoiceName = viewEntity.Name;
                        $scope.scopeModel.invoiceTitle = viewEntity.Title;
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

                function loadInvoiceTypeSelector() {
                    var loadInvoiceTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                    invoiceTypeSelectorReadyDeferred.promise.then(function () {
                        var payLoad;
                        if (viewEntity != undefined && viewEntity.Settings != undefined) {
                            payLoad = {
                                selectedIds: viewEntity.Settings.InvoiceTypeId
                            }
                        }
                        VRUIUtilsService.callDirectiveLoad(invoiceTypeSelectorAPI, payLoad, loadInvoiceTypeSelectorPromiseDeferred);
                    });
                    return loadInvoiceTypeSelectorPromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadTree, loadInvoiceTypeSelector, loadViewCommonProperties]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }

            function getView() {
                return VR_Sec_ViewAPIService.GetView(viewId).then(function (viewEntityObj) {
                    viewEntity = viewEntityObj;
                });
            }

        }

    }

    appControllers.controller('VR_Invoice_InvoiceViewEditorController', InvoiceViewEditorController);
})(appControllers);
