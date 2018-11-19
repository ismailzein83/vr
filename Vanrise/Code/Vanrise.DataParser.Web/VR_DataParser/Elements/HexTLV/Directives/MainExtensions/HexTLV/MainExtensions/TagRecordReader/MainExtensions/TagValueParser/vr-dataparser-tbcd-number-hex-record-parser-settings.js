"use strict";

app.directive("vrDataparserTbcdNumberHexRecordParserSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
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
        templateUrl: "/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/MainExtensions/TagValueParser/Templates/HexTBCDNumberParser.html"


    };

    function boolParserEditor($scope, ctrl) {

        var context;
        var dataRecordTypeFieldSelectorAPI;
        var dataRecordTypeFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.onDataRecordTypeFieldsSelectorReady = function (api) {
                dataRecordTypeFieldSelectorAPI = api;
                dataRecordTypeFieldSelectorReadyPromiseDeferred.resolve();
            };
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    console.log(payload)
                    if (payload.ValueParser != undefined)
                    {
                        $scope.scopeModel.fieldName = payload.ValueParser.FieldName;
                        $scope.scopeModel.aIsZero = payload.ValueParser.AIsZero;
                        $scope.scopeModel.removeHexa = payload.ValueParser.RemoveHexa;
                    }
                    context = payload.context;

                }


                //function loadDataRecordTypeFieldSelector() {
                //    var dataRecordTypeFieldLoadDeferred = UtilsService.createPromiseDeferred();
                //    dataRecordTypeFieldSelectorReadyPromiseDeferred.promise.then(function () {
                //        dataRecordTypeFieldSelectorReadyPromiseDeferred = undefined;
                //        var dataRecordTypeFieldPayload = {};
                //        if (payload != undefined && payload.ValueParser != undefined)
                //            dataRecordTypeFieldPayload.selectedIds = payload.ValueParser.FieldName;
                //        if (context != undefined)
                //            dataRecordTypeFieldPayload.dataRecordTypeId = context.recordTypeId();
                //        VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldSelectorAPI, dataRecordTypeFieldPayload, dataRecordTypeFieldLoadDeferred);
                //    });

                //    return dataRecordTypeFieldLoadDeferred.promise;
                //}
                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
                return {
                    $type: "Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers.TBCDNumberParser,Vanrise.DataParser.MainExtensions",
                    FieldName: $scope.scopeModel.fieldName,
                    AIsZero: $scope.scopeModel.aIsZero,
                    RemoveHexa: $scope.scopeModel.removeHexa
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