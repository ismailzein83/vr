"use strict";

app.directive("vrDataparserTimesTimestampRecordParserSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService","VRDateTimeService",
function (UtilsService, VRNotificationService, VRUIUtilsService, VRDateTimeService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new timesTimestampRecordParserEditor($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/TimesTimestampRecordParser.html"


    };

    function timesTimestampRecordParserEditor($scope, ctrl) {

        var context;
        var dataRecordTypeFieldSelectorAPI;
        var dataRecordTypeFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.dateTimeShift = VRDateTimeService.getNowDateTime();

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
                        $scope.scopeModel.dateTimeShift = payload.ValueParser.DateTimeShift;
                        $scope.scopeModel.dateTimeFieldName = payload.ValueParser.DateTimeFieldName;


                    }
                    context = payload.context;

                }


                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
				return {
					$type: "Vanrise.DataParser.MainExtensions.CompositeFieldParsers.TimestampDateTimeCompositeParser,Vanrise.DataParser.MainExtensions",
					FieldName: $scope.scopeModel.fieldName,
					DateTimeShift: $scope.scopeModel.dateTimeShift,
					DateTimeFieldName: $scope.scopeModel.dateTimeFieldName

				};
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }

    }




    return directiveDefinitionObject;

}]);