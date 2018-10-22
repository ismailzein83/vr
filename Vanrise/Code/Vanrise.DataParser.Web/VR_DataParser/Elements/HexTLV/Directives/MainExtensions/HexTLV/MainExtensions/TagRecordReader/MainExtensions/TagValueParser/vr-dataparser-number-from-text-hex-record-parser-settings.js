"use strict";

app.directive("vrDataparserNumberFromTextHexRecordParserSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService","VR_DataParser_NumberType",
function (UtilsService, VRNotificationService, VRUIUtilsService, VR_DataParser_NumberType) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new timeFromTextEditor($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/MainExtensions/TagValueParser/Templates/NumberFromTextHexParser.html"


    };

    function timeFromTextEditor($scope, ctrl) {
        var recordTypeFieldTypeDirectiveAPI;

        var context;
        var dataRecordTypeFieldSelectorAPI;
        var recordTypeFieldTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        var dataRecordTypeFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.recordTypeFieldType = UtilsService.getArrayEnum(VR_DataParser_NumberType);
            $scope.scopeModel.onDataRecordTypeFieldsSelectorReady = function (api) {
                dataRecordTypeFieldSelectorAPI = api;
                dataRecordTypeFieldSelectorReadyPromiseDeferred.resolve();
            };




            $scope.scopeModel.onRecordTypeFieldTypeDirectiveReady = function (api) {
                recordTypeFieldTypeDirectiveAPI = api;
                recordTypeFieldTypeDirectiveReadyDeferred.resolve();





                defineAPI();
            }
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                
                if (payload != undefined) {
                    if (payload.ValueParser != undefined) {
                        $scope.scopeModel.selectedValue = UtilsService.getItemByVal($scope.scopeModel.recordTypeFieldType, payload.ValueParser.NumberType, "value");
                        $scope.scopeModel.fieldName = payload.ValueParser.FieldName;
                    }
                    context = payload.context;

                }

                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
                return {
                    $type: "Vanrise.DataParser.MainExtensions.StringFieldParsers.NumberFromTextParser,Vanrise.DataParser.MainExtensions",
                    FieldName: $scope.scopeModel.fieldName,
                    NumberType: $scope.scopeModel.selectedValue != undefined ? $scope.scopeModel.selectedValue.value : undefined
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