(function (appControllers) {

    'use strict';

    mainGridColumnsEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService','VR_Invoice_InvoiceFieldEnum'];

    function mainGridColumnsEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VR_Invoice_InvoiceFieldEnum) {

        var gridColumns = [];
        var columnEntity;

        var isEditMode;
        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                gridColumns = parameters.gridColumns;
                columnEntity = parameters.columnEntity;
            }
            isEditMode = (columnEntity != undefined);
        }

        function defineScope() {
            $scope.invoiceFields = UtilsService.getArrayEnum(VR_Invoice_InvoiceFieldEnum);

            $scope.isCustomFieldRequired = function()
            {
                if ($scope.selectedInvoiceField != undefined)
                {
                    if ($scope.selectedInvoiceField.value == VR_Invoice_InvoiceFieldEnum.CustomField.value)
                        return true;
                }
                
                return false;
            }

            $scope.save = function () {
                return (isEditMode)?  updateGridColumn() : addGridColumn();
            };
            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {

            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle,loadStaticData]).then(function () {

            }).finally(function () {
                $scope.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
        }
        function setTitle() {
            if (isEditMode && columnEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(columnEntity.Header, 'Grid Column');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Grid Column');
        }

        function loadStaticData()
        {
            if(columnEntity != undefined)
            {
                $scope.header = columnEntity.Header;
                $scope.customFieldName = columnEntity.CustomFieldName;
                $scope.selectedInvoiceField = UtilsService.getItemByVal($scope.invoiceFields, columnEntity.Field,"value");
            }
        }

        function builGridColumnObjFromScope()
        {
            return {
                Header: $scope.header,
                Field:$scope.selectedInvoiceField.value,
                CustomFieldName:$scope.customFieldName
            };
        }

        function addGridColumn()
        {
            var gridColumnObj = builGridColumnObjFromScope();
            if ($scope.onGridColumnAdded != undefined) {
                $scope.onGridColumnAdded(gridColumnObj);
            }
            $scope.modalContext.closeModal();
        }

        function updateGridColumn()
        {
            var gridColumnObj = builGridColumnObjFromScope();
            if ($scope.onGridColumnUpdated != undefined) {
                $scope.onGridColumnUpdated(gridColumnObj);
            }
            $scope.modalContext.closeModal();
        }

    }
    appControllers.controller('VR_Invoice_MainGridColumnsEditorController', mainGridColumnsEditorController);

})(appControllers);