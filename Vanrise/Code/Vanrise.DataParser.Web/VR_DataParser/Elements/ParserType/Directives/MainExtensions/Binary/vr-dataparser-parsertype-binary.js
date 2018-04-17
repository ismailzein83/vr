(function (app) {
    'use strict';

    BinaryParserTypeDirective.$inject = ["UtilsService", "VRNotificationService", "VRUIUtilsService"];

    function BinaryParserTypeDirective(UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new BinaryParserType($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_DataParser/Elements/ParserType/Directives/MainExtensions/Binary/templates/BinaryRecordParser.html"

        };

        function BinaryParserType($scope, ctrl, $attrs) {

            var recordParserDirectiveAPI;
            var recordParserDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var context;

            this.initializeController = initializeController;
           
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onRecordParserDirective = function (api) {
                    recordParserDirectiveAPI = api;
                    recordParserDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var binaryParserTypeEntity;

                    if (payload != undefined) {
                        binaryParserTypeEntity = payload.extendedSettings;
                        context = payload.context;
                    }

                    promises.push(loadRecordParserDirective());

                    function loadRecordParserDirective() {
                        var recordParserDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                        recordParserDirectiveReadyDeferred.promise.then(function () {
                            var recordParserDirectivePayload = { context: getContext() };
                            
                            if (binaryParserTypeEntity != undefined) {
                                recordParserDirectivePayload.RecordParser = binaryParserTypeEntity.RecordParser;
                            }
                            VRUIUtilsService.callDirectiveLoad(recordParserDirectiveAPI, recordParserDirectivePayload, recordParserDirectiveLoadDeferred);
                        });
                        return recordParserDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.DataParser.Business.BinaryParserType,Vanrise.DataParser.Business",
                        RecordParser: recordParserDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }

        }


        return directiveDefinitionObject;
            }

    app.directive('vrDataparserParsertypeBinary', BinaryParserTypeDirective);

})(app);