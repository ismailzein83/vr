(function (appControllers) {

    "use strict";

    excelFileParserController.$inject = ["$scope", "VRCommon_ExcelFileParserAPIService", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService"];

    function excelFileParserController($scope, VRCommon_ExcelFileParserAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var fieldName;
        var type;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                fieldName = parameters.fieldName;
                type = parameters.type;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.dataValues = [];

            $scope.scopeModel.onFileUploadChange = function (obj) {
                if (obj != undefined) {
                    $scope.scopeModel.isLoadingValues = true;
                    VRCommon_ExcelFileParserAPIService.GetUploadedDataValues(obj.fileId, type).then(function (response) {
                        $scope.scopeModel.dataValues.length = 0;
                        if (response) {
                            for (var i = 0; i < response.length ; i++) {
                                if ($scope.scopeModel.dataValues.indexOf(response[i]) == -1)
                                    $scope.scopeModel.dataValues.push(response[i]);
                            }
                        }
                    }).finally(function () {
                        $scope.scopeModel.isLoadingValues = false;
                    });
                }
            };

            $scope.scopeModel.downloadTemplate = function (obj) {
                return VRCommon_ExcelFileParserAPIService.DowloadFileExcelParserTemplate(fieldName).then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            };
            $scope.scopeModel.returnDataValues = function () {
                if ($scope.onOkPressed != undefined)
                    $scope.onOkPressed(getValuesArray());
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }
        function load() {
            setTitle();
        }

        function setTitle() {
            $scope.title = "Excel File Parser";
        }

        function getValuesArray() {
            var values = [];
            for (var i = 0; i < $scope.scopeModel.dataValues.length ; i++) {
                values.push($scope.scopeModel.dataValues[i]);
            }
            return values;
        }

    }

    appControllers.controller("VRCommon_ExcelFileParserController", excelFileParserController);
})(appControllers);
