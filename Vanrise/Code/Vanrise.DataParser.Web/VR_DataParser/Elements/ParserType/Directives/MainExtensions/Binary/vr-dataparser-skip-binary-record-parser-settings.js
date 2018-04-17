(function (app) {
    'use strict';

    SkipBinaryRecordParserDirective.$inject = ["UtilsService", "VRNotificationService", "VRUIUtilsService"];

    function SkipBinaryRecordParserDirective(UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SkipBinaryRecordParser($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/SkipBinaryRecordParser.html"

        };

        function SkipBinaryRecordParser($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var recordParserDirectiveAPI;
            var recordParserDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onRecordParserDirective = function (api) {
                    recordParserDirectiveAPI = api;
                    recordParserDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload.extendedSettings != undefined) {
                        $scope.recordToSkipBytesLength = payload.extendedSettings.RecordToSkipBytesLength;
                        $scope.recordStartingTag = payload.extendedSettings.RecordStartingTag;
                    }

                    promises.push(loadRecordParserDirective());

                    function loadRecordParserDirective() {
                        var recordParserDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                        recordParserDirectiveReadyDeferred.promise.then(function () {
                            var recordParserDirectivePayload;
                            if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.RecordParser != null)
                                recordParserDirectivePayload = {
                                    RecordParser: payload.extendedSettings.RecordParser.Settings
                                };
                            VRUIUtilsService.callDirectiveLoad(recordParserDirectiveAPI, recordParserDirectivePayload, recordParserDirectiveLoadDeferred);
                        });
                        return recordParserDirectiveLoadDeferred.promise;
                    }

                    UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.DataParser.MainExtensions.BinaryParsers.Common.RecordParsers.SkipRecordParser, Vanrise.DataParser.MainExtensions",
                        RecordToSkipBytesLength: $scope.recordToSkipBytesLength,
                        RecordStartingTag: $scope.recordStartingTag,
                        RecordParser: {
                            Settings: recordParserDirectiveAPI.getData()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

            }
        }
        return directiveDefinitionObject;
    }
    app.directive('vrDataparserSkipBinaryRecordParserSettings', SkipBinaryRecordParserDirective);

})(app);