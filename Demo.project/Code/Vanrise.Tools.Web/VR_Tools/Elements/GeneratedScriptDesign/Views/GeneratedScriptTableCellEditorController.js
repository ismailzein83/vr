(appControllers); (function (appControllers) {
    "use strict";
    generatedScriptTableCellEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Tools_GeneratedScriptService', 'VR_Tools_ColumnsAPIService'];

    function generatedScriptTableCellEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Tools_GeneratedScriptService, VR_Tools_ColumnsAPIService) {

        $scope.scopeModel = {};
        var rowIndex;
        var columnName;
        var selectedTableData;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                rowIndex = parameters.rowIndex;
                columnName = parameters.columnName;
                selectedTableData = parameters.selectedTableData;
            }
        }

        function defineScope() {

            $scope.scopeModel.saveTableCell = function () {
                selectedTableData[rowIndex].Entity.FieldValues[columnName] = $scope.scopeModel.cellValue;
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
         
        }

        function load() {
            $scope.scopeModel.isLoading = true;

                loadAllControls().finally(function () {
                });
           
        }

        function loadAllControls() {
            
            function setTitle() {
                    $scope.title = "Edit Table Cell";
              
            }

            function loadStaticData() {

                $scope.scopeModel.cellValue = typeof (selectedTableData[rowIndex].Entity.FieldValues[columnName]) == "object" ? JSON.stringify(selectedTableData[rowIndex].Entity.FieldValues[columnName]) : selectedTableData[rowIndex].Entity.FieldValues[columnName];
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
           .finally(function () {
               $scope.scopeModel.isLoading = false;
           });
        }


    }
    appControllers.controller('Vanrise_Tools_GeneratedScriptTableCellEditorController', generatedScriptTableCellEditorController);
})(appControllers);