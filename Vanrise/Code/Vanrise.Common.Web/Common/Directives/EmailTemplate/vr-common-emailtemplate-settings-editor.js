'use strict';

app.directive('vrCommonEmailtemplateSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/EmailTemplate/Templates/EmailTemplateSettings.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.data != undefined) {
                        $scope.senderEmail = payload.data.SenderEmail;
                        $scope.senderPassword = payload.data.SenderPassword;
                        $scope.host = payload.data.Host;
                        $scope.port = payload.data.Port;
                        $scope.timeout = payload.data.Timeout;
                        $scope.ssl = payload.data.EnabelSsl;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Entities.EmailSettingData, Vanrise.Entities",
                        SenderEmail: $scope.senderEmail,
                        SenderPassword: $scope.senderPassword,
                        Host: $scope.host,
                        Port: $scope.port,
                        Timeout: $scope.timeout,
                        EnabelSsl: $scope.ssl
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);