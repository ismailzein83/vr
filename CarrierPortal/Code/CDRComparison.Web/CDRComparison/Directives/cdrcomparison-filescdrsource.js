(function (app) {

    'use strict';

    FileCDRSourceDirective.$inject = ['CDRComparison_CDRComparisonAPIService', 'UtilsService', 'VRUIUtilsService'];

    function FileCDRSourceDirective(CDRComparison_CDRComparisonAPIService, UtilsService, VRUIUtilsService) {
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

            function initializeController() {
                $scope.scopeModel = {};
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
                        fileReader = payload.FileReader;
                    }

                    var loadFileReaderSelectivePromise = loadFileReaderSelective();
                    promises.push(loadFileReaderSelectivePromise);

                    return UtilsService.waitMultiplePromises(promises);

                    function loadFileReaderSelective() {
                        var fileReaderSelectiveLoadDeferred = UtilsService.createPromiseDeferred();
                        VRUIUtilsService.callDirectiveLoad(fileReaderSelectiveAPI, undefined, fileReaderSelectiveLoadDeferred);
                        return fileReaderSelectiveLoadDeferred.promise;
                    }
                };

                api.getData = function () {
                    return {
                        FileId: $scope.file.fileId,
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