"use strict";

app.directive("vrDataparserFixedLengthPackageFieldParserSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new fixedLengthPackageFieldParserEditor($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/FixedLengthPackageFieldParser.html"


    };

    function fixedLengthPackageFieldParserEditor($scope, ctrl) {

        var context;
        //var dataRecordTypeFieldSelectorAPI;
        //var dataRecordTypeFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var recordParserDirectiveAPI;
        var recordParserDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.onTagParserDirective = function (api) {
                recordParserDirectiveAPI = api;
                recordParserDirectiveReadyDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    if (payload.extendedSettings != undefined)
                    { $scope.scopeModel.packageLength = payload.extendedSettings.PackageLength; }
                    context = payload.context;

                }
                promises.push(loadRecordParserDirective());

                function loadRecordParserDirective() {
                    var recordParserDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    recordParserDirectiveReadyDeferred.promise.then(function () {
                        var recordParserDirectivePayload;
                        if (payload != undefined && payload.extendedSettings != undefined && payload.extendedSettings.FieldParser != null) {
                            recordParserDirectivePayload = {
                                RecordParser: payload.extendedSettings.FieldParser
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(recordParserDirectiveAPI, recordParserDirectivePayload, recordParserDirectiveLoadDeferred);
                    });
                    return recordParserDirectiveLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
				return {
					$type: "Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.FieldParsers.PackageFieldParser.FixedLengthPackageFieldParser,Vanrise.DataParser.MainExtensions",
					PackageLength: $scope.scopeModel.packageLength,
					FieldParser: recordParserDirectiveAPI.getData()
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