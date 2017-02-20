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
        var cacheObjectName; // The cacheObjectName is defined on the client-side to preserve it between the following method calls: ValidateBulkActionZones, GetZoneLetters, GetZoneItems

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.headerRowExists = true;
            cacheObjectName = UtilsService.guid();

            $scope.scopeModel.onFileChanged = function () {
                cacheObjectName = UtilsService.guid();
            };

            $scope.scopeModel.onSwitchValueChanged = function () {
                cacheObjectName = UtilsService.guid();
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
                var data = {
                    $type: 'TOne.WhS.Sales.MainExtensions.ImportBulkAction, TOne.WhS.Sales.MainExtensions',
                    HeaderRowExists: $scope.scopeModel.headerRowExists,
                    CacheObjectName: cacheObjectName
                };
                if ($scope.scopeModel.file != undefined) {
                    data.FileId = $scope.scopeModel.file.fileId;
                }
                if (bulkActionContext != undefined) {
                    data.OwnerType = bulkActionContext.ownerType;
                    data.OwnerId = bulkActionContext.ownerId;
                }
                return data;
            };

            api.getSummary = function () {
                var uploadedFileName = ($scope.scopeModel.file != undefined) ? $scope.scopeModel.file.fileName : 'None';
                var headerRowExistsAsString = ($scope.scopeModel.headerRowExists === true) ? 'Header Exists' : 'Header Does Not Exist';
                return 'Uploaded File: ' + uploadedFileName + ' | ' + headerRowExistsAsString;
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }

    function getTemplate(attrs) {
        return '<span vr-loader="scopeModel.isLoading">\
                    <vr-columns colnum="{{importBulkActionCtrl.normalColNum}}">\
                        <vr-fileupload label="Rates" extension="xls,xlsx" value="scopeModel.file" onvaluechanged="scopeModel.onFileChanged" isrequired="importBulkActionCtrl.isrequired"></vr-fileupload>\
                    </vr-columns>\
                    <vr-columns colnum="{{importBulkActionCtrl.normalColNum}}">\
                        <vr-switch label="Header" value="scopeModel.headerRowExists" onvaluechanged="scopeModel.onSwitchValueChanged"></vr-switch>\
                    </vr-columns>\
                </span>';
    }
}]);