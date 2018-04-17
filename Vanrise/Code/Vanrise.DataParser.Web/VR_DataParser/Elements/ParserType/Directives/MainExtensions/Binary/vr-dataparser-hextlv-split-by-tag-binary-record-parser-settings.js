(function (app) {
    'use strict';

    HexTLVSplitByTagBinaryRecordParserDirective.$inject = ["UtilsService", "VRNotificationService", "VRUIUtilsService"];

    function HexTLVSplitByTagBinaryRecordParserDirective(UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "="
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new HexTLVSplitByTagBinaryRecordParser($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/HexTLVSplitByTagBinaryRecordParser.html"

        };

        function HexTLVSplitByTagBinaryRecordParser($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var recordParserDirectiveAPI;
            var recordParserDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var tagRecordParserDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
                       
            $scope.tagNames = [];

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.onRecordParserDirective = function (api) {
                    recordParserDirectiveAPI = api;
                    recordParserDirectiveReadyDeferred.resolve();
                };

                ctrl.addTagName = function () {
                    var dataItem = { Name: $scope.tagName };
                    dataItem.onTagRecordParserDirective = function (api) {
                        dataItem.tagRecordParserDirectiveAPI = api;
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingDirective = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.tagRecordParserDirectiveAPI, undefined, setLoader);
                    };
                    $scope.tagNames.push(dataItem);
                    $scope.tagName = "";
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    promises.push(loadRecordParserDirective());

                    function loadRecordParserDirective() {
                        var recordParserDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                        recordParserDirectiveReadyDeferred.promise.then(function () {
                            var recordParserDirectivePayload;
                            if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.DefaultSubRecordParser != null) {
                                recordParserDirectivePayload = {
                                    RecordParser: payload.extendedSettings.DefaultSubRecordParser.Settings
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(recordParserDirectiveAPI, recordParserDirectivePayload, recordParserDirectiveLoadDeferred);
                        });
                        return recordParserDirectiveLoadDeferred.promise;
                    }

                    function loadGrid() {
                        if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.SubRecordsParsersByTag != undefined) {

                            for (var tag in payload.extendedSettings.SubRecordsParsersByTag) {
                                if (tag != "$type") {
                                    var gridItem = {
                                        payload: {
                                            key: tag,
                                            value: payload.extendedSettings.SubRecordsParsersByTag[tag]
                                        },
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    //   promises.push(gridItem.loadPromiseDeferred.promise);
                                    addGridItem(gridItem);
                                }
                            }
                        }

                        function addGridItem(gridItem) {
                            var dataItem = {
                                Name: gridItem.payload.key
                            };

                            dataItem.onTagRecordParserDirective = function (api) {
                                dataItem.tagRecordParserDirectiveAPI = api;
                                gridItem.readyPromiseDeferred.resolve();
                            };

                            gridItem.readyPromiseDeferred.promise.then(function () {
                                var tagRecordParserDirectivePayload = {
                                    RecordParser: gridItem.payload.value.Settings
                                };
                                VRUIUtilsService.callDirectiveLoad(dataItem.tagRecordParserDirectiveAPI, tagRecordParserDirectivePayload, gridItem.loadPromiseDeferred);
                            });
                            $scope.tagNames.push(dataItem);
                        }
                    }

                    loadGrid();

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.DataParser.MainExtensions.BinaryParsers.HexTLV.RecordParsers.SplitByTagRecordParser, Vanrise.DataParser.MainExtensions",
                        DefaultSubRecordParser: {
                            Settings: recordParserDirectiveAPI.getData()
                        },
                        SubRecordsParsersByTag: getGridData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getGridData() {
                var tagsByValue;
                if ($scope.tagNames.length > 0) {
                    tagsByValue = {};
                    for (var i = 0; i < $scope.tagNames.length; i++) {
                        var tagName = $scope.tagNames[i];
                        tagsByValue[tagName.Name] = { Settings: tagName.tagRecordParserDirectiveAPI.getData() };
                    }
                }
                return tagsByValue;
            }
        }

        return directiveDefinitionObject;
    }
    app.directive('vrDataparserHextlvSplitByTagBinaryRecordParserSettings', HexTLVSplitByTagBinaryRecordParserDirective);

})(app);