"use strict";

app.directive("vrCommonExcelfileuploader", ["UtilsService", "VRNotificationService", "VRModalService", "VRCommon_ExcelFileUploaderService",
function (UtilsService, VRNotificationService, VRModalService, VRCommon_ExcelFileUploaderService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var excelFileUploaderCtor = new ExcelFileUploader($scope, ctrl, $attrs);
            excelFileUploaderCtor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Common/Directives/ExcelFileUploader/Templates/ExcelFileUploader.html"
       
    };

    function ExcelFileUploader($scope, ctrl, $attrs) {
        var api ={};
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.upload = function () {
                var onExcelAdded = function (fileId) {
                    $scope.scopeModel.file = {
                        fileId: fileId
                    };
                };
                VRCommon_ExcelFileUploaderService.addExcelSheets(onExcelAdded);
            }
            defineAPI();
        }

        function defineAPI() {
            api.load = function (payload) {
                var promises = [];
                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    fileId: $scope.scopeModel.file != undefined ? $scope.scopeModel.file.fileId : undefined
                };
            };

        }
    }

    return directiveDefinitionObject;

}]);