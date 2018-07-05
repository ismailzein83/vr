(function (appControllers) {

    "use strict";
    excelFileUploadeController.$inject = ['$scope', 'VRCommon_ExcelFileUploaderAPIService', 'VRNotificationService', 'UtilsService'];

    function excelFileUploadeController($scope, VRCommon_ExcelFileUploaderAPIService, VRNotificationService, UtilsService) {

        var gridApi;
        var dataItem = {};
        defineScope();

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.sheetsNames = [];
            dataItem.name = "Sheet1";
            $scope.scopeModel.sheetsNames.push(dataItem);


            $scope.scopeModel.addSheet = function (data) {
                dataItem = {};
                $scope.scopeModel.sheetsNames.push(dataItem);
            };

            $scope.scopeModel.removeSheet = function (dataItem) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.sheetsNames, dataItem.name, 'name');
                if (index > -1) {
                    $scope.scopeModel.sheetsNames.splice(index, 1);
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.save = function () {
               return uploadExcelFile();
            };

            $scope.title = UtilsService.buildTitleForAddEditor("Excel File Uploader");

            $scope.scopeModel.validateColumns = function () {
                if ($scope.scopeModel.sheetsNames.length == 0) {
                    $scope.scopeModel.isDisabled = true;
                    return 'Please, one record must be added at least.';
                } else {
                    $scope.scopeModel.isDisabled = false;
                }

                var columnName = [];
                for (var i = 0; i < $scope.scopeModel.sheetsNames.length; i++) {
                    if ($scope.scopeModel.sheetsNames[i].name != undefined) {
                        columnName.push($scope.scopeModel.sheetsNames[i].name);
                    }
                }
                while (columnName.length > 0) {
                    var nameToValidate = columnName[0];
                    columnName.splice(0, 1);
                    if (!validateName(nameToValidate, columnName)) {//duplicate sheets names
                        $scope.scopeModel.isDisabled = true;
                        return 'Two or more columns have the same Name';
                    }
                }
                function validateName(name, array) {
                    for (var j = 0; j < array.length; j++) {
                        if (array[j] == name)
                            return false;
                    }
                    return true;
                }

            };
        };

        function buildExcelFileObjectFromScope() {
            var sheets = [];
            for (var i = 0; i < $scope.scopeModel.sheetsNames.length; i++) {
                var sheet = $scope.scopeModel.sheetsNames[i];
                sheets.push(sheet.name);
            }
            var object = {
                sheets: sheets
            };
            return object;
        };

        function uploadExcelFile(){
            $scope.scopeModel.isLoading = true;

            var excelUploaderInput = buildExcelFileObjectFromScope();
            return VRCommon_ExcelFileUploaderAPIService.UploadExcelFile(excelUploaderInput).then(function(response) {
                if (response != undefined && response.IsSucceeded) {
                    if ($scope.onExcelAdded != undefined) {
                        $scope.onExcelAdded(response.FileId);
                    }
                    $scope.modalContext.closeModal();
                }
                
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;

            });

        };

    };

    appControllers.controller('VRCommon_ExcelFileUploaderEditorController', excelFileUploadeController);
})(appControllers);