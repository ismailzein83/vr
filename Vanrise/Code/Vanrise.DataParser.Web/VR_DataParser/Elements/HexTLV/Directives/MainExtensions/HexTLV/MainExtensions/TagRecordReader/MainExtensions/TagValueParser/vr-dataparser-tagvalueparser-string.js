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
            var ctor = new stringParserEditor($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/MainExtensions/TagValueParser/Templates/TagValueParserString.html"


    };

    function stringParserEditor($scope, ctrl) {


        var context;
        this.initializeController = initializeController;

        function initializeController() {


            $scope.scopeModel = {};

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                if (payload != undefined)
                {
                    $scope.scopeModel.fieldName = payload.ValueParser.FieldName;
                    context = payload.context;
                }
                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.DataParser.MainExtensions.HexTLV.TagValueParsers.StringParser ,Vanrise.DataParser.MainExtensions",
                    FieldName: $scope.scopeModel.fieldName
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