(function (appControllers) {

    'use strict';

    gridColumnsEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService','VRUIUtilsService'];

    function gridColumnsEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var columnEntity;


        var invoiceFieldSelectorApi;
        var invoiceFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

        var invoiceCustomFieldSelectorApi;
        var invoiceCustomFieldSelectorPromiseDeferred;

        var isEditMode;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                columnEntity = parameters.columnEntity;
            }
            isEditMode = (columnEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onInvoiceFieldSelectorReady = function (api) {
                invoiceFieldSelectorApi = api;
                invoiceFieldSelectorPromiseDeferred.resolve();
            };
            $scope.scopeModel.onInvoiceCustomFieldSelectorReady = function (api) {
                invoiceCustomFieldSelectorApi = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                var payload = {
                    connectionId: context != undefined ? context.getConnectionId() : undefined,
                    invoiceTypeId: context != undefined ? context.getInvoiceTypeId() : undefined,
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, invoiceCustomFieldSelectorApi, payload, setLoader, invoiceCustomFieldSelectorPromiseDeferred);
            };

            $scope.scopeModel.isCustomFieldRequired = function () {
                if ($scope.scopeModel.selectedInvoiceField != undefined) {
                    if ($scope.scopeModel.selectedInvoiceField.InvoiceFieldId == 0)
                        return true;
                }
                return false;
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateGridColumn() : addGridColumn();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };


            function builGridColumnObjFromScope() {
                return {
                    Header: $scope.scopeModel.header,
                    Field: $scope.scopeModel.selectedInvoiceField.InvoiceFieldId,
                    CustomFieldName: $scope.scopeModel.isCustomFieldRequired() ? $scope.scopeModel.selectedRecordField.FieldName : undefined
                };
            }

            function addGridColumn() {
                var gridColumnObj = builGridColumnObjFromScope();
                if ($scope.onGridColumnAdded != undefined) {
                    $scope.onGridColumnAdded(gridColumnObj);
                }
                $scope.modalContext.closeModal();
            }

            function updateGridColumn() {
                var gridColumnObj = builGridColumnObjFromScope();
                if ($scope.onGridColumnUpdated != undefined) {
                    $scope.onGridColumnUpdated(gridColumnObj);
                }
                $scope.modalContext.closeModal();
            }
        }

        function load() {

            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {

                function setTitle() {
                    if (isEditMode && columnEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(columnEntity.Header, 'Grid Column');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Grid Column');
                }
                function loadInvoiceFieldSelector() {
                    var invoiceFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                     invoiceFieldSelectorPromiseDeferred.promise.then(function () {
                        var payloadSelector = {
                            connectionId: context != undefined ? context.getConnectionId() : undefined,
                            selectedIds: columnEntity != undefined ? columnEntity.Field : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(invoiceFieldSelectorApi, payloadSelector, invoiceFieldSelectorLoadDeferred);
                    });
                    return invoiceFieldSelectorLoadDeferred.promise;
                }

                function loadInvoiceCustomFieldSelector() {
                    if (columnEntity != undefined && columnEntity.Field == 0)
                    {
                        invoiceCustomFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        var invoiceCustomFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        invoiceCustomFieldSelectorPromiseDeferred.promise.then(function () {
                            invoiceCustomFieldSelectorPromiseDeferred = undefined;
                            var payloadSelector = {
                                connectionId: context != undefined ? context.getConnectionId() : undefined,
                                invoiceTypeId: context != undefined ? context.getInvoiceTypeId() : undefined,
                                selectedIds: columnEntity != undefined ? columnEntity.CustomFieldName : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(invoiceCustomFieldSelectorApi, payloadSelector, invoiceCustomFieldSelectorLoadDeferred);
                        });
                        return invoiceCustomFieldSelectorLoadDeferred.promise;
                    }
                   
                }

                function loadStaticData() {
                    if (columnEntity != undefined) {
                        $scope.scopeModel.header = columnEntity.Header;
                    }
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadInvoiceFieldSelector, loadInvoiceCustomFieldSelector]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
            }

        }

    }
    appControllers.controller('PartnerPortal_Invoice_GridColumnsEditorController', gridColumnsEditorController);

})(appControllers);