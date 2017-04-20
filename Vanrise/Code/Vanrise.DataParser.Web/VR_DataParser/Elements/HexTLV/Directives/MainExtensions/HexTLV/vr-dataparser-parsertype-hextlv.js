"use strict";

app.directive("vrDataparserParsertypeHextlv", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new ParserTypeDirective($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/Templates/ParserTypeHexTLV.html"

    };

    function ParserTypeDirective($scope, ctrl) {
        var context;
        var recordReaderSelectorAPI;
        var recordReaderSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.onRecordReaderSelectorReady = function (api) {
                recordReaderSelectorAPI = api;
                recordReaderSelectorReadyPromiseDeferred.resolve();
            };
            UtilsService.waitMultiplePromises([recordReaderSelectorReadyPromiseDeferred.promise]).then(function () {
                defineAPI();
            });
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                var recordReaderEntity;
                if (payload != undefined) {
                    if (payload.extendedSettings != undefined)
                    { recordReaderEntity = payload.extendedSettings.RecordReader; }
                    context = payload.context;
                }

                var loadSelectorRecordReaderPromise = loadSelectorrecordReader();
                promises.push(loadSelectorRecordReaderPromise);

                function loadSelectorrecordReader() {

                    var selectorPayload = { context: getContext() };
                    if (recordReaderEntity != undefined) {
                        selectorPayload.recordReaderEntity = recordReaderEntity;
                    }

                    return recordReaderSelectorAPI.load(selectorPayload);;
                }

                return UtilsService.waitMultiplePromises(promises);
            };
            api.getData = function () {
                return {
                    $type: "Vanrise.DataParser.Business.HexTLV.HexTLVParserType ,Vanrise.DataParser.Business",
                    RecordReader: recordReaderSelectorAPI.getData(),
                };
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