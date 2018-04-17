(function (app) {
    'use strict';

    SplitByOffsetBinaryRecordParserDirective.$inject = ["UtilsService", "VRNotificationService", "VRUIUtilsService"];

    function SplitByOffsetBinaryRecordParserDirective(UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SplitByOffsetkBinaryRecordParser($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/SplitByOffsetBinaryRecordParser.html"

        };

        function SplitByOffsetkBinaryRecordParser($scope, ctrl, $attrs) {
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
                var promises = [];
                api.load = function (payload) {

                    if (payload.extendedSettings != undefined) {
                        $scope.lengthNbOfBytes = payload.extendedSettings.LengthNbOfBytes;
                        $scope.reverseLengthBytes = payload.extendedSettings.ReverseLengthBytes;
                    }

                    promises.push(loadRecordParserDirective());

                    function loadRecordParserDirective() {
                        var recordParserDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                        recordParserDirectiveReadyDeferred.promise.then(function () {
                            var recordParserDirectivePayload;
                            if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.RecordParser != null) {
                                recordParserDirectivePayload = {
                                    RecordParser: payload.extendedSettings.RecordParser.Settings
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
                        $type: "Vanrise.DataParser.MainExtensions.BinaryParsers.Common.RecordParsers.SplitByOffSetRecordParser,Vanrise.DataParser.MainExtensions",
                        LengthNbOfBytes: $scope.lengthNbOfBytes,
                        ReverseLengthBytes: $scope.reverseLengthBytes,
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

    app.directive('vrDataparserSplitByOffsetBinaryRecordParserSettings', SplitByOffsetBinaryRecordParserDirective);

})(app);