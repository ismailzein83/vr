"use strict";

app.directive("vrDataparserHextlvTagTypeGrid", ["UtilsService", "VR_DataParser_ParserTypeConfigService",
    function (UtilsService, VR_DataParser_ParserTypeConfigService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new HexTagTypeGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/Templates/HexTLVTagTypeGrid.html"

        };

        function HexTagTypeGrid($scope, ctrl, $attrs) {

            var gridAPI;
            var context;
            var tagTypes;
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];
                ctrl.validate = function () {
                    if (ctrl.datasource.length != 0)
                        return null;
                    return "it is empty grid";
                };
                ctrl.addHexTLVTagType = function () {
                    var onHexTLVTagTypeAdded = function (tagTypeObj) {
                        ctrl.datasource.push({ Entity: tagTypeObj });
                    };
                    VR_DataParser_ParserTypeConfigService.addHexTLVTagType(onHexTLVTagTypeAdded, getContext());
                };


                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var tagTypes = {};
                    var lengthDataSource = ctrl.datasource.length;
                    for (var i = 0; i < lengthDataSource; i++) {
                        var dataSourceItem = ctrl.datasource[i];
                        tagTypes[dataSourceItem.Entity.Key] = dataSourceItem.Entity.Value;

                    }

                    return tagTypes;
                };

                api.load = function (payload) {
                    ctrl.datasource.length = 0;
                    if (payload != undefined) {
                        tagTypes = payload.tagTypes;
                        context = payload.context;
                        if (tagTypes != undefined) {

                            for (var name in tagTypes) {
                                if (name != "$type") {
                                    var gridTagType = {
                                        Key: name,
                                        Value: tagTypes[name]
                                    };
                                    ctrl.datasource.push({ Entity: gridTagType });
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
                    clicked: editHexTLVTagType,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editHexTLVTagType(tagTypeObj) {
                var onEditHexTLVTagType = function (tag) {
                    var index = ctrl.datasource.indexOf(tagTypeObj);
                    ctrl.datasource[index] = { Entity: tag };
                };
                VR_DataParser_ParserTypeConfigService.editHexTLVTagType(tagTypeObj, onEditHexTLVTagType, getContext());
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