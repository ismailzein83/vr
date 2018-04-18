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

            var smsSendHandlerSelectorAPI;
            var smsSendHandlerSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var context;

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onSMSSendHandlerSelectorReady = function (api) {
                    smsSendHandlerSelectorAPI = api;
                    smsSendHandlerSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    if (payload != undefined)
                        context = payload.context;

                    promises.push(loadSMSSendHandlerSelector());

                    function loadSMSSendHandlerSelector() {
                        var smsSendHandlerSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        smsSendHandlerSelectorReadyDeferred.promise.then(function () {
                            var smsSendHandlerSelectorPayload = { context: getContext() };

                            if (payload != undefined && payload.Handler != undefined)
                                smsSendHandlerSelectorPayload.HandlerSettings = payload.Handler.Settings;

                            VRUIUtilsService.callDirectiveLoad(smsSendHandlerSelectorAPI, smsSendHandlerSelectorPayload, smsSendHandlerSelectorLoadDeferred);
                        });
                        return smsSendHandlerSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Entities.SMSSendHandler,Vanrise.Entities",
                        Settings: smsSendHandlerSelectorAPI.getData()
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

    app.directive('vrCommonSmssendhandler', SMSSendHandlerDirective);

})(app);