"use strict";

app.directive("vrDataparserDateTimePackageRecordParserSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new dateTimePackageParserEditor($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/MainExtensions/TagValueParser/Templates/HexDateTimePackageFieldParser.html"


    };

    function dateTimePackageParserEditor($scope, ctrl) {

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
                        $scope.scopeModel.beginDateTimeFieldName = payload.ValueParser.BeginDateTimeFieldName;
                        $scope.scopeModel.endDateTimeFieldName = payload.ValueParser.EndDateTimeFieldName;
                        $scope.scopeModel.durationFieldName = payload.ValueParser.DurationFieldName;
                        $scope.scopeModel.yearIndex = payload.ValueParser.YearIndex;
                        $scope.scopeModel.monthIndex = payload.ValueParser.MonthIndex;
                        $scope.scopeModel.dayIndex = payload.ValueParser.DayIndex;
                        $scope.scopeModel.hoursIndex = payload.ValueParser.HoursIndex;
                        $scope.scopeModel.minutesIndex = payload.ValueParser.MinutesIndex;
                        $scope.scopeModel.secondsIndex = payload.ValueParser.SecondsIndex;
                        $scope.scopeModel.flagsMillisecondsIndex = payload.ValueParser.FlagsMillisecondsIndex;
                        $scope.scopeModel.durationIndex = payload.ValueParser.DurationIndex;

                    }
                    context = payload.context;

                }


                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
                return {
                    $type: "Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.FieldParsers.DateTimePackageParser,Vanrise.DataParser.MainExtensions",
                    BeginDateTimeFieldName: $scope.scopeModel.beginDateTimeFieldName,
                    EndDateTimeFieldName: $scope.scopeModel.endDateTimeFieldName,
                    DurationFieldName: $scope.scopeModel.durationFieldName,
                    YearIndex: $scope.scopeModel.yearIndex,
                    MonthIndex: $scope.scopeModel.monthIndex,
                    DayIndex: $scope.scopeModel.dayIndex,
                    HoursIndex: $scope.scopeModel.hoursIndex,
                    MinutesIndex: $scope.scopeModel.minutesIndex,
                    SecondsIndex: $scope.scopeModel.secondsIndex,
                    FlagsMillisecondsIndex: $scope.scopeModel.flagsMillisecondsIndex,
                    DurationIndex: $scope.scopeModel.durationIndex

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