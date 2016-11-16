'use strict';

app.directive('vrSecSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var securitySettingsEditor = new SecuritySettingsEditor(ctrl, $scope, $attrs);
                securitySettingsEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Security/Directives/Settings/Templates/SecuritySettingsTemplate.html"
        };

        function SecuritySettingsEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var mailMessageTemplateSettingsAPI;
            var mailMessageTemplateSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};

            $scope.scopeModel.onMailMessageTemplateSettingsReady = function (api) {
                mailMessageTemplateSettingsAPI = api;
                mailMessageTemplateSettingsReadyPromiseDeferred.resolve();
            };

            function initializeController() {
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var mailMessageTemplateSettingsPayload;
                    if (payload != undefined && payload.data != undefined) {
                        mailMessageTemplateSettingsPayload = payload.data.MailMessageTemplateSettings;
                    }


                    //Loading Mail Message Template Settings
                    var mailMessageTemplateSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    mailMessageTemplateSettingsReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(mailMessageTemplateSettingsAPI, mailMessageTemplateSettingsPayload, mailMessageTemplateSettingsLoadPromiseDeferred);
                        });

                    return mailMessageTemplateSettingsLoadPromiseDeferred.promise;
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Security.Entities.SecuritySettings, Vanrise.Security.Entities",
                        MailMessageTemplateSettings: mailMessageTemplateSettingsAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);