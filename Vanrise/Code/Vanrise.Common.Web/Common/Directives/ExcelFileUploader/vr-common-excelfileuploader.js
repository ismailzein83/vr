"use strict";

app.directive("vrCommonExcelfileuploader", ["UtilsService", "VRNotificationService", "VRModalService", "VRCommon_ExcelFileUploaderService",
function (UtilsService, VRNotificationService, VRModalService, VRCommon_ExcelFileUploaderService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '=',
            value: '=',
            isrequired:'='
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
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.upload = function () {
                var onExcelAdded = function (fileId) {
                 var file = {
                        fileId: fileId
                    };
                    ctrl.value = file;
                };
                VRCommon_ExcelFileUploaderService.addExcelSheets(onExcelAdded);
            };
            defineAPI();
            }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                ctrl.onReady(api);
            }
        }
    }

    return directiveDefinitionObject;

}]);