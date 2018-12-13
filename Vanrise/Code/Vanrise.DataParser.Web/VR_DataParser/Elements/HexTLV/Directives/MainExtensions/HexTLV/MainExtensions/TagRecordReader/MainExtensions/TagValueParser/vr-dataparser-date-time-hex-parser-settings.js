"use strict";

app.directive("vrDataparserDateTimeHexParserSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_DataParser_DateTimeParsingType",
function (UtilsService, VRNotificationService, VRUIUtilsService, VR_DataParser_DateTimeParsingType) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new dateTimeParserEditor($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/MainExtensions/TagValueParser/Templates/HexDateTimeParser.html"


    };

    function dateTimeParserEditor($scope, ctrl) {
        var recordTypeFieldTypeDirectiveAPI;
        var context;
        var dataRecordTypeFieldSelectorAPI;
        var recordTypeFieldTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        var dataRecordTypeFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        this.initializeController = initializeController;




        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.recordTypeFieldType = UtilsService.getArrayEnum(VR_DataParser_DateTimeParsingType);
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
                        $scope.scopeModel.selectedValue = UtilsService.getItemByVal($scope.scopeModel.recordTypeFieldType, payload.ValueParser.DateTimeParsingType, "value");
                        $scope.scopeModel.fieldName = payload.ValueParser.FieldName;
                        //$scope.scopeModel.fieldBytesLength = payload.ValueParser.FieldBytesLength;
                        //$scope.scopeModel.fieldIndex = payload.ValueParser.FieldIndex;
                        //$scope.scopeModel.reverse = payload.ValueParser.Reverse
                        $scope.scopeModel.dayIndex=payload.ValueParser.DayIndex,
                        $scope.scopeModel.monthIndex=payload.ValueParser.MonthIndex,
                        $scope.scopeModel.yearIndex=payload.ValueParser.YearIndex,
                        $scope.scopeModel.secondsIndex=payload.ValueParser.SecondsIndex,
                        $scope.scopeModel.minutesIndex=payload.ValueParser.MinutesIndex,
                        $scope.scopeModel.hoursIndex=payload.ValueParser.HoursIndex,
                        $scope.scopeModel.hoursTimeShiftIndex=payload.ValueParser.HoursTimeShiftIndex,
                        $scope.scopeModel.minutesTimeShiftIndex=payload.ValueParser.MinutesTimeShiftIndex,
                        $scope.scopeModel.timeShiftIndicatorIndex=payload.ValueParser.TimeShiftIndicatorIndex,
                        $scope.scopeModel.withOffset=payload.ValueParser.WithOffset,
                        $scope.scopeModel.isBCD = payload.ValueParser.IsBCD
                    }
                    context = payload.context;

                }

                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
                return {
                    $type: "Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers.DateTimeParser,Vanrise.DataParser.MainExtensions",
                    FieldName: $scope.scopeModel.fieldName,
                    DateTimeParsingType: $scope.scopeModel.selectedValue != undefined ? $scope.scopeModel.selectedValue.value : undefined,
                    //Reverse: $scope.scopeModel.reverse,
                    //FieldIndex: $scope.scopeModel.fieldIndex,
                    //FieldBytesLength: $scope.scopeModel.fieldBytesLength
                    DayIndex: $scope.scopeModel.dayIndex,
                    MonthIndex: $scope.scopeModel.monthIndex,
                    YearIndex: $scope.scopeModel.yearIndex,
                    SecondsIndex: $scope.scopeModel.secondsIndex,
                    MinutesIndex: $scope.scopeModel.minutesIndex,
                    HoursIndex: $scope.scopeModel.hoursIndex,
                    HoursTimeShiftIndex: $scope.scopeModel.hoursTimeShiftIndex,
                    MinutesTimeShiftIndex: $scope.scopeModel.minutesTimeShiftIndex,
                    TimeShiftIndicatorIndex: $scope.scopeModel.timeShiftIndicatorIndex,
                    WithOffset: $scope.scopeModel.withOffset,
                    IsBCD: $scope.scopeModel.isBCD 

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