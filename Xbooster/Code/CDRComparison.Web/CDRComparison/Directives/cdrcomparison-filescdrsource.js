(function (app) {

    'use strict';

    FileCDRSourceDirective.$inject = ['CDRComparison_CDRComparisonAPIService', 'VRCommon_FileService', 'UtilsService', 'VRUIUtilsService'];

    function FileCDRSourceDirective(CDRComparison_CDRComparisonAPIService, VRCommon_FileService, UtilsService, VRUIUtilsService) {
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

            function initializeController() {
                $scope.scopeModel = {};

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
                };

                api.getData = function () {
                    return {
                        $type: 'CDRComparison.Business.FileCDRSource, CDRComparison.Business',
                        FileId: $scope.scopeModel.file.fileId,
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