"use strict";

app.directive("vrDataparserTagvalueparserString", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
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
        templateUrl: "/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/MainExtensions/TagValueParser/Templates/TagValueParserString.html"


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
                    if (payload.ValueParser != undefined)
                    { $scope.scopeModel.fieldName = payload.ValueParser.FieldName; }
                    context = payload.context;

                    if (context != undefined) {

                        $scope.scopeModel.useRecordType = context.useRecordType();
                    }
                }
                if ($scope.scopeModel.useRecordType) {
                    promises.push(loadDataRecordTypeFieldSelector());
                }

                function loadDataRecordTypeFieldSelector() {
                    var dataRecordTypeFieldLoadDeferred = UtilsService.createPromiseDeferred();
                    dataRecordTypeFieldSelectorReadyPromiseDeferred.promise.then(function () {
                        dataRecordTypeFieldSelectorReadyPromiseDeferred = undefined;
                        var dataRecordTypeFieldPayload = {};
                        if (payload != undefined && payload.ValueParser != undefined)
                            dataRecordTypeFieldPayload.selectedIds = payload.ValueParser.FieldName;
                        if (context != undefined)
                            dataRecordTypeFieldPayload.dataRecordTypeId = context.recordTypeId();
                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldSelectorAPI, dataRecordTypeFieldPayload, dataRecordTypeFieldLoadDeferred);
                    });

                    return dataRecordTypeFieldLoadDeferred.promise;
                }
                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
                var fieldName;
                if ($scope.scopeModel.useRecordType == true) {
                    fieldName = dataRecordTypeFieldSelectorAPI.getSelectedIds();
                }
                else { fieldName = $scope.scopeModel.fieldName; }
                return {
                    $type: "Vanrise.DataParser.MainExtensions.HexTLV.TagValueParsers.BoolParser ,Vanrise.DataParser.MainExtensions",
                    FieldName: fieldName
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