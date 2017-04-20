"use strict";

app.directive("vrDataparserTagvalueparserSequence", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new sequenceParserEditor($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/MainExtensions/TagValueParser/Templates/TagValueParserSequence.html"


    };

    function sequenceParserEditor($scope, ctrl) {
        var context;
        var valueParserEntity;
        var hexTLVTagTypeGridAPI;
        var hexTLVTagTypeGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onHexTLVTagTypeGridReady = function (api) {
                hexTLVTagTypeGridAPI = api;
                hexTLVTagTypeGridReadyPromiseDeferred.resolve();
            };

            UtilsService.waitMultiplePromises([hexTLVTagTypeGridReadyPromiseDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    valueParserEntity = payload.ValueParser;
                    context = payload.context;
                }

                var promises = [];
                promises.push(loadHexTLVTagTypeGridDirective());

                function loadHexTLVTagTypeGridDirective() {
                    var hexTLVTagTypePayload = { context: getContext() };
                    if (valueParserEntity != undefined)
                        hexTLVTagTypePayload.tagTypes = valueParserEntity.TagTypes;
                    return hexTLVTagTypeGridAPI.load(hexTLVTagTypePayload);
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.DataParser.MainExtensions.HexTLV.TagValueParsers.SequenceParser ,Vanrise.DataParser.MainExtensions",
                    TagTypes: hexTLVTagTypeGridAPI.getData()
                }
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }

    }

    return directiveDefinitionObject;

}]);