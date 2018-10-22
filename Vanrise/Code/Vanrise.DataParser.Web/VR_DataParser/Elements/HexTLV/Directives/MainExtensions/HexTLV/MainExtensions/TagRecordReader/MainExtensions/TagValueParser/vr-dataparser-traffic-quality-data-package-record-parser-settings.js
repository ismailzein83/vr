"use strict";

app.directive("vrDataparserTrafficQualityDataPackageRecordParserSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new trafficQualityDataParserEditor($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/MainExtensions/TagValueParser/Templates/HexTrafficQualityDataFieldParser.html"


    };

    function trafficQualityDataParserEditor($scope, ctrl) {

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
                    {
                        $scope.scopeModel.causeValueFieldName = payload.ValueParser.CauseValueFieldName;
                        $scope.scopeModel.codingStandardFieldName = payload.ValueParser.CodingStandardFieldName;
                        $scope.scopeModel.locationFieldName = payload.ValueParser.LocationFieldName;
                    }
                    context = payload.context;

                }


                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
                return {
                    $type: "Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.FieldParsers.TrafficQualityDataPackageParser,Vanrise.DataParser.MainExtensions",
                    CauseValueFieldName: $scope.scopeModel.causeValueFieldName,
                    CodingStandardFieldName: $scope.scopeModel.codingStandardFieldName,
                    LocationFieldName: $scope.scopeModel.locationFieldName

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