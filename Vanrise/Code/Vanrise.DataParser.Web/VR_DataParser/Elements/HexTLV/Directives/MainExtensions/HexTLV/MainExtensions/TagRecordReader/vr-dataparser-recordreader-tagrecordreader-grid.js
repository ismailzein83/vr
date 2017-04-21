"use strict";

app.directive("vrDataparserRecordreaderTagrecordreaderGrid", ["UtilsService", "VR_DataParser_ParserTypeConfigService",
    function (UtilsService, VR_DataParser_ParserTypeConfigService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new TagRecordReader($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/Templates/RecordReaderTagRecordReaderGrid.html"

        };

        function TagRecordReader($scope, ctrl, $attrs) {

            var gridAPI;
            var context;
            var recordTypesByTag;
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];
                ctrl.validate = function () {
                    if (ctrl.datasource.length != 0)
                        return null;
                    return "it is empty grid";
                };
                ctrl.addTagRecordReader = function () {
                    var onTagRecordReaderAdded = function (gridRecord) {
                        ctrl.datasource.push({ Entity: gridRecord });
                    };
                    VR_DataParser_ParserTypeConfigService.addTagRecordReader(onTagRecordReaderAdded, getContext());
                };


                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var recordtypesByTag = {};
                    var lengthDataSource = ctrl.datasource.length;
                    for (var i = 0; i < lengthDataSource; i++) {
                        var dataSourceItem = ctrl.datasource[i];
                        recordtypesByTag[dataSourceItem.Entity.Key] = dataSourceItem.Entity.Value;

                    }

                    return recordtypesByTag;
                };
                api.load = function (payload) {
                    ctrl.datasource.length = 0;
                    if (payload != undefined) {
                        recordTypesByTag = payload.recordTypesByTag;
                        context = payload.context;
                        if (recordTypesByTag != undefined) {
                            for (var tag in recordTypesByTag) {
                                if (tag != "$type") {
                                    var gridRecord = {
                                        Key: tag,
                                        Value: recordTypesByTag[tag]
                                    };
                                    ctrl.datasource.push({ Entity: gridRecord });
                                }

                            }
                        }
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editTagRecordReader,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editTagRecordReader(recordObj) {
                var onEditTagRecordReader = function (tag) {
                    var index = ctrl.datasource.indexOf(recordObj);
                    ctrl.datasource[index] = { Entity: tag };
                };
                VR_DataParser_ParserTypeConfigService.editTagRecordReader(recordObj, onEditTagRecordReader, getContext());
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);