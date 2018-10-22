(function (app) {
    'use strict';

    TagvalueparserSequenceDirective.$inject = ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_DataParser_RecordTypeFieldType"];

    function TagvalueparserSequenceDirective(UtilsService, VRNotificationService, VRUIUtilsService, VR_DataParser_RecordTypeFieldType) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "="
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new TagvalueparserSequence($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/MainExtensions/TagValueParser/Templates/TagValueParserSequence.html"

        };

        function TagvalueparserSequence($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var recordTypeFieldTypeDirectiveAPI;
            var recordTypeFieldTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var tagRecordParserDirectiveAPI;
            var tagRecordParserDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            var tagRecordEntity;

            $scope.tagNames = [];

            function initializeController() {


                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [{ Type: "Int", Id: VR_DataParser_RecordTypeFieldType.Int },
                                                { Type: "String", Id: VR_DataParser_RecordTypeFieldType.String }];

                $scope.scopeModel.onRecordTypeFieldTypeDirectiveReady = function (api) {
                    recordTypeFieldTypeDirectiveAPI = api;
                    recordTypeFieldTypeDirectiveReadyDeferred.resolve();
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
    

                    function loadGrid() {
                        if (payload != undefined && payload.ValueParser != undefined && payload.ValueParser.FieldParsers != undefined && payload.ValueParser.FieldParsers.FieldParsersByTag != undefined) {
                            console.log(payload)
                            for (var tag in payload.ValueParser.FieldParsers.FieldParsersByTag) {
                                if (tag != "$type") {
                                    var gridItem = {
                                        payload: {
                                            key: tag,
                                            value: payload.ValueParser.FieldParsers.FieldParsersByTag[tag]
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
                        $type: "Vanrise.DataParser.MainExtensions.BinaryParsers.HexTLV.FieldParsers.SequenceFieldParser,Vanrise.DataParser.MainExtensions",

                        FieldParsers: {FieldParsersByTag: getGridData()}
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
    app.directive('vrDataparserTagvalueparserSequence', TagvalueparserSequenceDirective);

})(app);