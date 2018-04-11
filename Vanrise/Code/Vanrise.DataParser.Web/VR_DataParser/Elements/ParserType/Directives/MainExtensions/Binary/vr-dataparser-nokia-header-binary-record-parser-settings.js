(function (app) {
    'use strict';

    NokiaHeaderBinaryRecordParserDirective.$inject = ["UtilsService", "VRNotificationService", "VRUIUtilsService"];

    function NokiaHeaderBinaryRecordParserDirective(UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new NokiaHeaderBinaryRecordParser($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/NokiaHeaderBinaryRecordParser.html"

        };

        function NokiaHeaderBinaryRecordParser($scope, ctrl, $attrs) {
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
                        $scope.headerLength = payload.extendedSettings.HeaderLength;
                        $scope.recordLengthIndex = payload.extendedSettings.RecordLengthIndex;
                        $scope.recordLengthByteLength = payload.extendedSettings.RecordLengthByteLength;
                    }

                    promises.push(loadRecordParserDirective());

                    function loadRecordParserDirective() {
                        var recordParserDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                        recordParserDirectiveReadyDeferred.promise.then(function () {
                            var recordParserDirectivePayload;
                            if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.PackageRecordParser != null) {
                                recordParserDirectivePayload = {
                                    RecordParser: payload.extendedSettings.PackageRecordParser.Settings
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(recordParserDirectiveAPI, recordParserDirectivePayload, recordParserDirectiveLoadDeferred);
                        });
                        return recordParserDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.RecordParsers.HeaderRecordParser, Vanrise.DataParser.MainExtensions",
                        HeaderLength: $scope.headerLength,
                        RecordLengthIndex: $scope.recordLengthIndex,
                        RecordLengthByteLength: $scope.recordLengthByteLength,
                        PackageRecordParser: {
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
    app.directive('vrDataparserNokiaHeaderBinaryRecordParserSettings', NokiaHeaderBinaryRecordParserDirective);

})(app);