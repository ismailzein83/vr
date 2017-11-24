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

            $scope.scopeModel.onFileChanged = function (obj) {
                cacheObjectName = UtilsService.guid();
                if (obj != undefined)
                    {
                            $scope.scopeModel.file = { fileId: obj.fileId, fileName: obj.fileName};
                         WhS_Sales_BulkActionUtilsService.onBulkActionChanged(bulkActionContext);
                    }
                    
            };

            $scope.scopeModel.onSwitchValueChanged = function () {
                cacheObjectName = UtilsService.guid();
            };
            $scope.scopeModel.onDateTimeFormatChanged = function () {
                cacheObjectName = UtilsService.guid();
            };

            $scope.scopeModel.download = function () {
                WhS_Sales_RatePlanAPIService.DownloadImportRatePlanTemplate().then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
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
                return WhS_Sales_RatePlanAPIService.GetSystemDateFormat().then(function (response) {
                    $scope.scopeModel.dateTimeFormat = response;
                });
            };

            api.getData = function () {
                var data = {
                    $type: 'TOne.WhS.Sales.MainExtensions.ImportBulkAction, TOne.WhS.Sales.MainExtensions',
                    HeaderRowExists: $scope.scopeModel.headerRowExists,
                    DateTimeFormat: $scope.scopeModel.dateTimeFormat,
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
                var dateTimeFormatAsString = ($scope.scopeModel.dateTimeFormat != undefined) ? $scope.scopeModel.dateTimeFormat : 'None';
                return 'Uploaded File: ' + uploadedFileName + ' | ' + headerRowExistsAsString + ' | Date Time Format: ' + dateTimeFormatAsString;
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }

    function getTemplate(attrs) {
        return '<span vr-loader="scopeModel.isLoading">\
                    <vr-columns colnum="{{importBulkActionCtrl.normalColNum / 2}}">\
                        <vr-fileupload tempfile="true" label="Rates" extension="xls,xlsx" value="scopeModel.file" onvaluechanged="scopeModel.onFileChanged" isrequired="importBulkActionCtrl.isrequired"></vr-fileupload>\
                    </vr-columns>\
                    <vr-columns colnum="{{importBulkActionCtrl.normalColNum}}">\
                        <vr-timeformat label="Date Time Format" value="scopeModel.dateTimeFormat" normal-col-num="{{importBulkActionCtrl.normalColNum}}" isrequired="importBulkActionCtrl.isrequired" onvaluechanged="scopeModel.onDateTimeFormatChanged"></vr-timeformat>\
                    </vr-columns>\
                    <vr-columns colnum="{{importBulkActionCtrl.normalColNum / 6}}">\
                        <vr-switch label="Header" value="scopeModel.headerRowExists" onvaluechanged="scopeModel.onSwitchValueChanged"></vr-switch>\
                    </vr-columns>\
                    <vr-columns colnum="{{importBulkActionCtrl.normalColNum / 6}}" withemptyline>\
                        <vr-button type="Download" data-onclick="scopeModel.download" standalone></vr-button>\
                    </vr-columns>\
                </span>';
    }
}]);