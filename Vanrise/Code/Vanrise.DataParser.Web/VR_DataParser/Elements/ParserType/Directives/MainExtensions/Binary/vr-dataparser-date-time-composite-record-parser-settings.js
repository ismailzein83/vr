"use strict";

app.directive("vrDataparserDateTimeCompositeRecordParserSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new dateTimeCompositeParserEditor($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/DateTimeCompositeRecordParser.html"


    };

    function dateTimeCompositeParserEditor($scope, ctrl) {

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
                        $scope.scopeModel.fieldName = payload.ValueParser.FieldName;
                        $scope.scopeModel.dateFieldName = payload.ValueParser.DateFieldName;
                        $scope.scopeModel.timeFieldName = payload.ValueParser.TimeFieldName;
                        $scope.scopeModel.secondsToAddFieldName = payload.ValueParser.SecondsToAddFieldName;
                       $scope.scopeModel.subtractTime = payload.ValueParser.SubtractTime;
                    }
                    context = payload.context;

                }
            

                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
				return {
					$type: "Vanrise.DataParser.MainExtensions.CompositeFieldParsers.DateTimeCompositeParser,Vanrise.DataParser.MainExtensions",
					FieldName: $scope.scopeModel.fieldName,
					DateFieldName: $scope.scopeModel.dateFieldName,
					TimeFieldName: $scope.scopeModel.timeFieldName,
					SecondsToAddFieldName: $scope.scopeModel.secondsToAddFieldName,
					SubtractTime: $scope.scopeModel.subtractTime


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