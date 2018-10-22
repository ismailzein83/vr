(function (app) {
    'use strict';

    HuaweiBinaryRecordParserDirective.$inject = ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_DataParser_RecordTypeFieldType"];

    function HuaweiBinaryRecordParserDirective(UtilsService, VRNotificationService, VRUIUtilsService, VR_DataParser_RecordTypeFieldType) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "="
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new HuaweiBinaryRecordParser($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/HuaweiBinaryRecordParser.html"

        };

        function HuaweiBinaryRecordParser($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            var tagRecordParserDirectiveAPI;
            var tagRecordParserDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            var tagRecordEntity;

            $scope.tagNames = [];

            function initializeController() {


                $scope.scopeModel = {};
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
                    if (payload.extendedSettings != undefined) {
                        $scope.headerLength = payload.extendedSettings.HeaderLength;
                        $scope.recordTypeByteLength = payload.extendedSettings.RecordTypeByteLength;
                        $scope.recordLengthPosition = payload.extendedSettings.RecordLengthPosition;
                        $scope.recordByteLength = payload.extendedSettings.RecordByteLength;
                        $scope.recordTypePosition = payload.extendedSettings.RecordTypePosition;
                    }
                    function loadGrid() {
                        if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.SubRecordsParsersByRecordType != undefined) {

                            for (var tag in payload.extendedSettings.SubRecordsParsersByRecordType) {
                                if (tag != "$type") {
                                    var gridItem = {
                                        payload: {
                                            key: tag,
                                            value: payload.extendedSettings.SubRecordsParsersByRecordType[tag]
                                        },
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
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
                        $type: "Vanrise.DataParser.MainExtensions.BinaryParsers.HuaweiParser.RecordParsers.HuaweiRecordParser,Vanrise.DataParser.MainExtensions",
                        RecordLengthPosition: $scope.recordLengthPosition,
                        RecordByteLength: $scope.recordByteLength,
                        RecordTypePosition: $scope.recordTypePosition,
                        RecordTypeByteLength: $scope.recordTypeByteLength,
                        HeaderLength: $scope.headerLength,
                        SubRecordsParsersByRecordType: getGridData()

                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

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
        }
        return directiveDefinitionObject;
    }
    app.directive('vrDataparserHuaweiBinaryRecordParserSettings', HuaweiBinaryRecordParserDirective);

})(app);