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
            var cdrSourceContext;

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

                        VRUIUtilsService.callDirectiveLoad(fileReaderSelectiveAPI, payload, fileReaderSelectiveLoadDeferred);
                        return fileReaderSelectiveLoadDeferred.promise;
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'CDRComparison.Business.FileCDRSource, CDRComparison.Business',
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