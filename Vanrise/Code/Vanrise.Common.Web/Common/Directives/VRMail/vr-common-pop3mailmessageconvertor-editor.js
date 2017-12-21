'use strict';

app.directive('vrCommonPop3mailmessageconvertorEditor', ['VRUIUtilsService', 'UtilsService',
    function (VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var pop3MessageConvertor = new Pop3MessageConvertor($scope, ctrl, $attrs);
                pop3MessageConvertor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRMail/Templates/Pop3MailMessageConvertorEditorTemplate.html'
        };

        function Pop3MessageConvertor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Common.MainExtensions.Pop3MailMessageConvertor, Vanrise.Common.MainExtensions",
                        Name: "Received Mail Message Convertor",
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
