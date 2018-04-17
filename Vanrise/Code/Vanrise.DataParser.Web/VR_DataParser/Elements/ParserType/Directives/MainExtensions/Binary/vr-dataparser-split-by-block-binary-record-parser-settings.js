(function (app) {
    'use strict';

    SplitByBlockBinaryRecordParserDirective.$inject = ["UtilsService", "VRNotificationService", "VRUIUtilsService"];

    function SplitByBlockBinaryRecordParserDirective(UtilsService, VRNotificationService, VRUIUtilsService) {

            var directiveDefinitionObject = {

                restrict: "E",
                scope:
                {
                    onReady: "="
                },

                controller: function ($scope, $element, $attrs) {
                    var ctrl = this;

                    var ctor = new SplitByBlockBinaryRecordParser($scope, ctrl, $attrs);
                    ctor.initializeController();
                },

                controllerAs: "ctrl",
                bindToController: true,
                compile: function (element, attrs) {

                },
                templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/SplitByBlockBinaryRecordParser.html"

            };

            function SplitByBlockBinaryRecordParser($scope, ctrl, $attrs) {
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
                            $scope.blockSize = payload.extendedSettings.BlockSize;
                            $scope.lengthNbOfBytes = payload.extendedSettings.LengthNbOfBytes;
                            $scope.reverseLengthBytes = payload.extendedSettings.ReverseLengthBytes;
                            $scope.dataLengthIndex = payload.extendedSettings.DataLengthIndex;
                            $scope.dataLengthBytesLength = payload.extendedSettings.DataLengthBytesLength;

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
                            $type: "Vanrise.DataParser.MainExtensions.BinaryParsers.Common.RecordParsers.SplitByBlockRecordParser, Vanrise.DataParser.MainExtensions",
                            BlockSize: $scope.blockSize,
                            LengthNbOfBytes: $scope.lengthNbOfBytes,
                            ReverseLengthBytes: $scope.reverseLengthBytes,
                            DataLengthIndex: $scope.dataLengthIndex,
                            DataLengthBytesLength: $scope.dataLengthBytesLength,
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
    app.directive('vrDataparserSplitByBlockBinaryRecordParserSettings', SplitByBlockBinaryRecordParserDirective);

})(app);