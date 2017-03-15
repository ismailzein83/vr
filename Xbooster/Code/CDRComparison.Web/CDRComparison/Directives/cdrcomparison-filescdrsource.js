(function (app) {

    'use strict';

    FileCDRSourceDirective.$inject = ['CDRComparison_CDRComparisonAPIService', 'VRCommon_FileService', 'CDRComparison_FileCDRSourceAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function FileCDRSourceDirective(CDRComparison_CDRComparisonAPIService, VRCommon_FileService, CDRComparison_FileCDRSourceAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {
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
            var sizeInMegaBytes;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.validateFile = function (fileName, fileSizeInBytes) {

                    var fileSizeInMegaBytes = fileSizeInBytes * 0.000001;
                    var maxSize = (sizeInMegaBytes != null) ? sizeInMegaBytes : 5;
                    
                    if (fileSizeInMegaBytes <= maxSize)
                        return true;
                    else if (isCompressedFormat(fileName))
                        return true;
                    else {
                        VRNotificationService.showWarning("File '" + fileName + "' is > " + maxSize + " MB. Please upload a compressed version <= " + maxSize + " MB");
                        return false;
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
                        return CDRComparison_FileCDRSourceAPIService.GetMaxUncompressedFileSizeInMegaBytes().then(function (response) {
                            sizeInMegaBytes = response;
                        });
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'CDRComparison.Business.FileCDRSource, CDRComparison.Business',
                        FileId: $scope.scopeModel.file.fileId,
                        IsCompressed: isCompressedFormat(),
                        FileReader: fileReaderSelectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function isCompressedFormat(fileName)
            {
                var finalFileName = (fileName != undefined) ? fileName : $scope.scopeModel.file.fileName;
                var nameParts = finalFileName.split('.');
                var extension = nameParts[nameParts.length - 1];
                return (extension == 'zip' || extension == 'rar');
            }
        }
    }

    app.directive('cdrcomparisonFilecdrsource', FileCDRSourceDirective);

})(app);