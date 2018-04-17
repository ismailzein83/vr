(function (app) {
    'use strict';

    SMSSendHandlerDirective.$inject = ["UtilsService", "VRNotificationService", "VRUIUtilsService"];

    function SMSSendHandlerDirective(UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SMSSendHandler($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/SMS/Templates/SMSSendHandler.html"

        };

        function SMSSendHandler($scope, ctrl, $attrs) {

            var smsSendHandlerDirectiveAPI;
            var smsSendHandlerDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var context;

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onSMSSendHandlerDirectiveReady = function (api) {
                    smsSendHandlerDirectiveAPI = api;
                    smsSendHandlerDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};
                
                api.load = function (payload) {
                    console.log("payload: " + JSON.stringify(payload));
                    var promises = [];
                   
                    if (payload != undefined)
                        context = payload.context;
                    
                    promises.push(loadSMSSendHandlerDirective());

                    function loadSMSSendHandlerDirective() {
                        var smsSendHandlerDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                        smsSendHandlerDirectiveReadyDeferred.promise.then(function () {
                             var smsSendHandlerDirectivePayload = { context: getContext() };
                            
                          /*  if (binaryParserTypeEntity != undefined) {
                                recordParserDirectivePayload.RecordParser = binaryParserTypeEntity.RecordParser;
                            }*/
                            VRUIUtilsService.callDirectiveLoad(smsSendHandlerDirectiveAPI, smsSendHandlerDirectivePayload, smsSendHandlerDirectiveLoadDeferred);
                        });
                        return smsSendHandlerDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Entities.SMSSendHandler,Vanrise.Entities",
                        Settings: smsSendHandlerDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }

        }


        return directiveDefinitionObject;
    }

    app.directive('vrCommonSmssendhandler', SMSSendHandlerDirective);

})(app);