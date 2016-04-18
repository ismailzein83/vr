(function (app) {

    'use strict';

    FileCDRSourceDirective.$inject = ['CDRComparison_CDRComparisonAPIService', 'VRCommon_FileService', 'VRCommon_FileAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function FileCDRSourceDirective(CDRComparison_CDRComparisonAPIService, VRCommon_FileService, VRCommon_FileAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var fileCDRSource = new FileCDRSource($scope, ctrl, $attrs);
                fileCDRSource.initializeController();
            },
            controllerAs: "fileSourceCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/CDRComparison/Directives/Templates/FileCDRSourceTemplate.html"
        };

        function FileCDRSource($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var fileReaderSelectiveAPI;
            var cdrSourceContext;
            var isCompressed;
            var sizeInMegaBytes;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.validateFile = function (fileName, fileSizeInBytes) {

                    var fileSizeInMegaBytes = fileSizeInBytes * 0.000001;
                    var maxSize = (sizeInMegaBytes != null) ? sizeInMegaBytes : 5

                    var nameParts = fileName.split('.');
                    var fileExtension = nameParts[nameParts.length - 1];
                    
                    if (fileSizeInMegaBytes <= maxSize) {
                        isCompressed = isCompressedFormat(fileExtension);
                        return true;
                    }
                    else if (isCompressedFormat(fileExtension)) {
                        isCompressed = true;
                        return true;
                    }
                    else {
                        VRNotificationService.showWarning("File '" + fileName + "' is > " + maxSize + " MB. Please upload a compressed version <= " + maxSize + " MB");
                        return false;
                    }

                    function isCompressedFormat(extension) {
                        return (extension == 'zip' || extension == 'rar');
                    }
                };

                $scope.scopeModel.viewRecentFiles = function () {
                    var onRecentFileSelected = function (fileId) {
                        $scope.scopeModel.file = {
                            fileId: fileId
                        };
                    };
                    VRCommon_FileService.viewRecentFiles('CDRComparison_FileCDRSource', onRecentFileSelected);
                };

                $scope.scopeModel.onFileReaderSelectiveReady = function (api) {
                    fileReaderSelectiveAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var fileReader;

                    if (payload != undefined) {
                        cdrSourceContext = payload.cdrSourceContext;
                        fileReader = payload.FileReader;
                        isCompressed = payload.IsCompressed; // It won't matter because it's reset when a file is uploaded
                    }

                    var loadFileReaderSelectivePromise = loadFileReaderSelective();
                    promises.push(loadFileReaderSelectivePromise);

                    var getMaxUncompressedFileSizeInMegaBytesPromise = getMaxUncompressedFileSizeInMegaBytes();
                    promises.push(getMaxUncompressedFileSizeInMegaBytesPromise);

                    return UtilsService.waitMultiplePromises(promises);

                    function loadFileReaderSelective() {
                        var fileReaderSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        var payload = (fileReader != undefined) ? fileReader : {};
                        payload.cdrSourceContext = cdrSourceContext;

                        setFileCDRSourceContext(payload);

                        VRUIUtilsService.callDirectiveLoad(fileReaderSelectiveAPI, payload, fileReaderSelectiveLoadDeferred);
                        return fileReaderSelectiveLoadDeferred.promise;
                    }

                    function setFileCDRSourceContext(payloadObject) {
                        payloadObject.fileCDRSourceContext = {};
                        payloadObject.fileCDRSourceContext.disableReadSampleButton = function () {
                            return ($scope.scopeModel.file == null || $scope.scopeModel.file.fileId == null);
                        };
                    }

                    function getMaxUncompressedFileSizeInMegaBytes() {
                        return VRCommon_FileAPIService.GetMaxUncompressedFileSizeInMegaBytes().then(function (response) {
                            sizeInMegaBytes = response;
                        });
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'CDRComparison.Business.FileCDRSource, CDRComparison.Business',
                        FileId: $scope.scopeModel.file.fileId,
                        IsCompressed: isCompressed,
                        FileReader: fileReaderSelectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('cdrcomparisonFilecdrsource', FileCDRSourceDirective);

})(app);