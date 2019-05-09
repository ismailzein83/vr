(function (appControllers) {

    'use strict';

    mainGridColumnsEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VR_Invoice_InvoiceFieldEnum', 'VRCommon_GridWidthFactorEnum', 'VRUIUtilsService'];

    function mainGridColumnsEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VR_Invoice_InvoiceFieldEnum, VRCommon_GridWidthFactorEnum, VRUIUtilsService) {

        var context;
        var columnEntity;
        var gridWidthFactorEditorAPI;
        var gridWidthFactorEditorPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        var invoiceUIGridColumnFilterAPI;
        var invoiceUIGridColumnFilterPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        var localizationTextResourceSelectorAPI;
        var localizationTextResourceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onGridWidthFactorEditorReady = function (api) {
                gridWidthFactorEditorAPI = api;
                gridWidthFactorEditorPromiseReadyDeferred.resolve();
            };
            $scope.scopeModel.invoiceFields = UtilsService.getArrayEnum(VR_Invoice_InvoiceFieldEnum);
            $scope.scopeModel.recordFields = context != undefined ? context.getFields() : [];
            $scope.scopeModel.isCustomFieldRequired = function () {
                if ($scope.scopeModel.selectedInvoiceField != undefined) {
                    if ($scope.scopeModel.selectedInvoiceField.value == VR_Invoice_InvoiceFieldEnum.CustomField.value)
                        return true;
                }

                return false;
            };

            $scope.scopeModel.onInvoiceUIGridColumnFilterReady = function (api) {
                invoiceUIGridColumnFilterAPI = api;
                invoiceUIGridColumnFilterPromiseReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode)?  updateGridColumn() : addGridColumn();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.onLocalizationTextResourceSelectorReady = function (api) {
                localizationTextResourceSelectorAPI = api;
                localizationTextResourceSelectorReadyPromiseDeferred.resolve();
            };

            function builGridColumnObjFromScope() {
                return {
                    Header: $scope.scopeModel.header,
                    Field: $scope.scopeModel.selectedInvoiceField.value,
                    CustomFieldName: $scope.scopeModel.isCustomFieldRequired() ? $scope.scopeModel.selectedRecordField.FieldName : undefined,
                    GridColumnSettings: gridWidthFactorEditorAPI.getData(),
                    UseDescription: $scope.scopeModel.isCustomFieldRequired() ? $scope.scopeModel.useDescription : undefined,
                    Filter: invoiceUIGridColumnFilterAPI.getData(),
                    TextResourceKey: localizationTextResourceSelectorAPI != undefined ? localizationTextResourceSelectorAPI.getSelectedValues() : undefined
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

                function loadStaticData() {
                    if (columnEntity != undefined) {
                        $scope.scopeModel.header = columnEntity.Header;
                        $scope.scopeModel.selectedRecordField = UtilsService.getItemByVal($scope.scopeModel.recordFields, columnEntity.CustomFieldName, "FieldName");
                        $scope.scopeModel.selectedInvoiceField = UtilsService.getItemByVal($scope.scopeModel.invoiceFields, columnEntity.Field, "value");
                        $scope.scopeModel.useDescription = columnEntity.UseDescription;
                    }
                }

                function loadGridWidthFactorEditor()
                {
                    var gridWidthFactorEditorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    gridWidthFactorEditorPromiseReadyDeferred.promise.then(function () {
                        var gridWidthFactorEditorPayload = { 
                            data: {
                                Width: VRCommon_GridWidthFactorEnum.Normal.value
                            }
                        };
                        if (columnEntity != undefined)
                            gridWidthFactorEditorPayload.data = columnEntity.GridColumnSettings;
                        VRUIUtilsService.callDirectiveLoad(gridWidthFactorEditorAPI, gridWidthFactorEditorPayload, gridWidthFactorEditorLoadPromiseDeferred);
                    });
                    return gridWidthFactorEditorLoadPromiseDeferred.promise;
                }

                function loadInvoiceUIGridColumnFilter() {
                    var invoiceUIGridColumnFilterLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    invoiceUIGridColumnFilterPromiseReadyDeferred.promise.then(function () {
                        var invoiceUIGridColumnFilterPayload = {};
                     
                        if (columnEntity != undefined)
                            invoiceUIGridColumnFilterPayload.invoiceUIGridColumnFilterEntity = columnEntity.Filter;
                        VRUIUtilsService.callDirectiveLoad(invoiceUIGridColumnFilterAPI, invoiceUIGridColumnFilterPayload, invoiceUIGridColumnFilterLoadPromiseDeferred);
                    });
                    return invoiceUIGridColumnFilterLoadPromiseDeferred.promise;
                }
                function loadLocalizationTextResourceSelector() {
                    var localizationTextResourceSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    var localizationTextResourcePayload = columnEntity != undefined ? { selectedValue: columnEntity.TextResourceKey } : undefined;

                    localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, localizationTextResourcePayload, localizationTextResourceSelectorLoadPromiseDeferred);
                    });
                    return localizationTextResourceSelectorLoadPromiseDeferred.promise;
                }
                var promises = [setTitle, loadStaticData, loadGridWidthFactorEditor, loadInvoiceUIGridColumnFilter, loadLocalizationTextResourceSelector];
                return UtilsService.waitMultipleAsyncOperations(promises).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }

        }

    }
    appControllers.controller('VR_Invoice_MainGridColumnsEditorController', mainGridColumnsEditorController);

})(appControllers);