"use strict";

app.directive("vrIntegrationAdapterDb", ['UtilsService',
function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: "/Client/Modules/Integration/Directives/Adapter/Templates/AdapterDBTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;


        function initializeController() {
            $scope.lastImportedId = 0;
            $scope.cmdTimeoutInSec = 600;
            $scope.validateTimeOffset = function (value) {

                if (value == undefined) return null;

                var offset = value.split(".");

                if (offset.length == 1) {
                    var time = offset[0].split("-");

                    if (time.length == 1 && validateTime(time[0]))
                        return null;

                    else if (time.length == 2 && time[0].length == 0 && validateTime(time[1]))
                        return null;
                }
                else if (offset.length == 2) {
                    var days = offset[0].split("-");

                    if (days.length == 1 && validateInteger(days[0], 99) && validateTime(offset[1]))
                        return null;

                    else if (days.length == 2 && days[0].length == 0 && validateInteger(days[1], 99) && validateTime(offset[1]))
                        return null;
                }

                return "Format: DD.HH:MM:SS";
            }

            defineAPI();
        }

        function defineAPI() {
           
          
            var api = {};
                        
            api.getData = function () {
                var obj = {
                    $type: "Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments.DBAdapterArgument, Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments",
                    ConnectionString: $scope.connectionString,
                    Query: $scope.query,
                    IdentifierColumnName: $scope.identifierColumnName,
                    NumberOfParallelReader: $scope.numberOfParallelReader,
                    CommandTimeoutInSeconds: $scope.cmdTimeoutInSec
                };
                if ($scope.isTimeRange)
                    obj.TimeOffset = $scope.timeOffSet;
                else
                    obj.NumberOffSet = $scope.numberOffSet;
                return obj;
            };
            api.getStateData = function () {
                return {
                    $type: "Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments.DBAdapterState, Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments",
                    LastImportedId: $scope.lastImportedId
                };
            };


            api.load = function (payload) {
                
                if (payload != undefined) {
                    var argumentData = payload.adapterArgument;
                    if (argumentData != undefined) {
                        $scope.cmdTimeoutInSec = argumentData.CommandTimeoutInSeconds;
                        $scope.connectionString = argumentData.ConnectionString;
                        $scope.query = argumentData.Query;
                        $scope.identifierColumnName = argumentData.IdentifierColumnName;
                        $scope.numberOfParallelReader = argumentData.NumberOfParallelReader;
                        if (argumentData.TimeOffset != undefined && argumentData.TimeOffset != null) {
                            $scope.isTimeRange = true;
                            $scope.timeOffSet = argumentData.TimeOffset;
                        }
                        else
                            $scope.numberOffSet = argumentData.NumberOffSet;
                    }

                    var adapterState = payload.adapterState;
                    if (adapterState != undefined) {
                        $scope.lastImportedId = adapterState.LastImportedId;
                    }
                }
              
                
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function validateInteger(integer, maxValue) {
            var parsedInt = parseInt(integer);

            if (isNaN(parsedInt) || parsedInt < 0 || parsedInt > maxValue) return false;

            return true;
        }

        function validateTime(time) { // the valid time format is HH:MM:SS

            if (time.length != 8) return false;

            var timeArray = time.split(":");

            if (timeArray.length != 3 || timeArray[0].length != 2 || timeArray[1].length != 2 || timeArray[2].length != 2)
                return false;

            if (validateInteger(timeArray[0], 23) && validateInteger(timeArray[1], 59) && validateInteger(timeArray[2], 59))
                return true;

            return false;
        }
    }

    return directiveDefinitionObject;
}]);
