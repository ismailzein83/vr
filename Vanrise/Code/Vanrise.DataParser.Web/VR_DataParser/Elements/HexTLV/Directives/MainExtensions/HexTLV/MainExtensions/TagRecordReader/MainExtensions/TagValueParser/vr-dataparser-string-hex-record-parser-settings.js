"use strict";
app.directive("vrDataparserStringHexRecordParserSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new boolParserEditor($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/MainExtensions/TagValueParser/Templates/HexStringParse.html"


    };

    function boolParserEditor($scope, ctrl) {
        var context;
        var dataRecordTypeFieldSelectorAPI;
        var dataRecordTypeFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var recordParserDirectiveAPI;
        var recordParserDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        this.initializeController = initializeController;

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
                console.log(payload)
                var promises = [];
                var binaryParserTypeEntity;
                    if (payload != undefined) {
                        if (payload.ValueParser != undefined)
                        { $scope.scopeModel.fieldName = payload.ValueParser.FieldName; }
                        context = payload.context;

                    }

                promises.push(loadRecordParserDirective());



                function loadRecordParserDirective() {
                    var recordParserDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    recordParserDirectiveReadyDeferred.promise.then(function () {
                        var recordParserDirectivePayload;
                        if (payload != undefined) {
                            recordParserDirectivePayload = {
                                Parser: payload
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
                    $type: "Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers.StringParser,Vanrise.DataParser.MainExtensions",
                    FieldName: $scope.scopeModel.fieldName,
                    Parser: recordParserDirectiveAPI.getData()

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