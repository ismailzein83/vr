(function (app) {
    'use strict';

    ExecuteDatabaseCommandSMSHandlerDirective.$inject = ["UtilsService", "VRNotificationService", "VRUIUtilsService"];

    function ExecuteDatabaseCommandSMSHandlerDirective(UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ExecuteDatabaseCommandSMSHandlerDirective($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/SMS/MainExtensions/Templates/ExecuteDatabaseCommandSMSHandlerTemplate.html"

        };

        function ExecuteDatabaseCommandSMSHandlerDirective($scope, ctrl, $attrs) {
        
            var connectionSelectorAPI;
            var connectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            ctrl.datasource = [{ Element: "#MobileNumber#", Description: "The mobile number to send the SMS message to" },
                                { Element: "#Message#", Description: "SMS Message" }];

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onConnectionSelectorReady = function (api) {
                    connectionSelectorAPI = api;
                    connectionSelectorReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    
                    if (payload != undefined && payload.Settings != undefined)
                        $scope.commandQuery = payload.Settings.CommandQuery;

                    promises.push(loadConnectionSelector());

                    function loadConnectionSelector() {
                        var connectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        connectionSelectorReadyDeferred.promise.then(function () {
                            var connectionSelectorPayload = {};
                              if (payload != undefined && payload.Settings != undefined) {
                                  connectionSelectorPayload.selectedIds = payload.Settings.VRConnectionId;
                              }
                            VRUIUtilsService.callDirectiveLoad(connectionSelectorAPI, connectionSelectorPayload, connectionSelectorLoadDeferred);
                        });
                        return connectionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Common.MainExtensions.SMSSendHandler.ExecuteDatabaseCommandSMSHandler,Vanrise.Common.MainExtensions",
                        VRConnectionId: connectionSelectorAPI.getSelectedIds(),
                        CommandQuery: $scope.commandQuery
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            
        }

        return directiveDefinitionObject;
    }

    app.directive('vrCommonExecutedatabasecommandSmshandler', ExecuteDatabaseCommandSMSHandlerDirective);

})(app);