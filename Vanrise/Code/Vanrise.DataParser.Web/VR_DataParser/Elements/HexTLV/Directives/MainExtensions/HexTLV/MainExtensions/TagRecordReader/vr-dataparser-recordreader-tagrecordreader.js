"use strict";

app.directive("vrDataparserRecordreaderTagrecordreader", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var hexTLVTagDirective = new HexTLVTagDirective($scope, ctrl);
            hexTLVTagDirective.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/Templates/RecordReaderTagRecordReader.html"


    };

    function HexTLVTagDirective($scope, ctrl) {

        var context;
        var tagRecordReaderGridAPI;
        var tagRecordReaderGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            
            $scope.scopeModel.onTagRecordReaderGridReady = function (api) {

                tagRecordReaderGridAPI = api;
                tagRecordReaderGridReadyPromiseDeferred.resolve();

            };
            UtilsService.waitMultiplePromises([tagRecordReaderGridReadyPromiseDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                var gridPayload;
                if (payload != undefined) {
                    $scope.scopeModel.numberOfBytesToSkip = payload.recordReaderEntity.NumberOfBytesToSkip;
                    context = payload.context;
                    gridPayload = {
                        recordTypesByTag: payload.recordReaderEntity.RecordTypesByTag,
                        context: getContext()
                    };
                }
                if (gridPayload != undefined) {
                    var loadTagRecordReaderPromise = loadRecordTypesByTagGrid();
                    promises.push(loadTagRecordReaderPromise);
                }

                function loadRecordTypesByTagGrid() {

                    return tagRecordReaderGridAPI.load(gridPayload);
                }
                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
                return {
                    $type: "Vanrise.DataParser.MainExtensions.HexTLV.RecordReaders.TagRecordReader ,Vanrise.DataParser.MainExtensions",
                    RecordTypesByTag: tagRecordReaderGridAPI.getData(),
                    NumberOfBytesToSkip: $scope.scopeModel.numberOfBytesToSkip,
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