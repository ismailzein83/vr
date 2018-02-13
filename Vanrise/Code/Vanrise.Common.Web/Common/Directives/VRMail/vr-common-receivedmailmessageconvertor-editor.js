'use strict';

app.directive('vrCommonReceivedmailmessageconvertorEditor', ['VRUIUtilsService', 'UtilsService',
    function (VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var receivedMessageConvertor = new ReceivedMessageConvertor($scope, ctrl, $attrs);
                receivedMessageConvertor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRMail/Templates/ReceivedMailMessageConvertorEditorTemplate.html'
        };

        function ReceivedMessageConvertor($scope, ctrl, $attrs) {
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
                        $type: "Vanrise.Common.MainExtensions.ReceivedMailMessageConvertor, Vanrise.Common.MainExtensions",
                        Name: "Received Mail Message Converter",
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
