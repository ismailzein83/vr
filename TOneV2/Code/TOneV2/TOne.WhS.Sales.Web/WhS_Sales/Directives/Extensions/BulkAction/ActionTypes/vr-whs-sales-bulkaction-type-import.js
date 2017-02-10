'use strict';

app.directive('vrWhsSalesBulkactionTypeImport', ['WhS_Sales_RatePlanAPIService', 'WhS_Sales_BulkActionUtilsService', 'UtilsService', 'VRUIUtilsService', function (WhS_Sales_RatePlanAPIService, WhS_Sales_BulkActionUtilsService, UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var importBulkAction = new ImportBulkAction($scope, ctrl, $attrs);
            importBulkAction.initializeController();
        },
        controllerAs: "importBulkActionCtrl",
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function ImportBulkAction($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var bulkActionContext;

        var validDataByZoneId;
        var invalidDataByRowIndex;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.headerRowExists = true;

            $scope.scopeModel.onUploadedFileChanged = function (uploadedFile) {
                validateFile(uploadedFile);
            };

            $scope.scopeModel.onSwitchValueChanged = function () {
                validateFile($scope.scopeModel.file);
            };

            defineAPI();
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                var bulkAction;

                if (payload != undefined) {
                    bulkActionContext = payload.bulkActionContext;
                    bulkAction = payload.bulkAction;
                }
            };

            api.getData = function () {
                // Otherwise, the serializer would throw an exception
                deleteObjectTypeProperty(validDataByZoneId);
                deleteObjectTypeProperty(invalidDataByRowIndex);

                var data = {
                    $type: 'TOne.WhS.Sales.MainExtensions.ImportBulkAction, TOne.WhS.Sales.MainExtensions',
                    ValidDataByZoneId: validDataByZoneId,
                    InvalidDataByRowIndex: invalidDataByRowIndex
                };

                function deleteObjectTypeProperty(object) {
                    if (object != undefined) {
                        delete object.$type;
                    }
                }

                return data;
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }

        function validateFile(uploadedFile) {
            WhS_Sales_BulkActionUtilsService.onBulkActionChanged(bulkActionContext);

            if (uploadedFile == undefined)
                return;

            $scope.scopeModel.isLoading = true;

            var importedDataValidationInput = {
                FileId: uploadedFile.fileId,
                HeaderRowExists: $scope.scopeModel.headerRowExists
            };
            if (bulkActionContext != undefined) {
                importedDataValidationInput.OwnerType = bulkActionContext.ownerType;
                importedDataValidationInput.OwnerId = bulkActionContext.ownerId;
            }
            return WhS_Sales_RatePlanAPIService.ValidateImportedData(importedDataValidationInput).then(function (response) {
                if (response != undefined) {
                    validDataByZoneId = response.ValidDataByZoneId;
                    invalidDataByRowIndex = response.InvalidDataByRowIndex;
                }
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }

    function getTemplate(attrs) {
        return '<span vr-loader="scopeModel.isLoading">\
                    <vr-columns colnum="{{importBulkActionCtrl.normalColNum}}">\
                        <vr-fileupload label="Rates" extension="xls,xlsx" value="scopeModel.file" onvaluechanged="scopeModel.onUploadedFileChanged" isrequired="importBulkActionCtrl.isrequired"></vr-fileupload>\
                    </vr-columns>\
                    <vr-columns colnum="{{importBulkActionCtrl.normalColNum}}">\
                        <vr-switch label="Header" value="scopeModel.headerRowExists" onvaluechanged="scopeModel.onSwitchValueChanged"></vr-switch>\
                    </vr-columns>\
                </span>';
    }
}]);