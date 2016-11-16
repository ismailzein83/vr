'use strict';
app.directive('vrNotificationActionEmail', ['UtilsService', 'VRUIUtilsService', 'Retail_BE_ActionDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, Retail_BE_ActionDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new NotificationActionEmailCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_Notification/Directives/MainExtensions/VRActions/Templates/EmailActionTemplate.html';
            }

        };

        function NotificationActionEmailCtor(ctrl, $scope) {

            var vrActionEntity;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        vrActionEntity = payload.vrActionEntity;
                    }
                };

                api.getData = function () {

                    return {
                        $type: "Vanrise.Notification.MainExtensions.VRActions.EmailAction, Vanrise.Notification.MainExtensions",
                        ActionName: "Email"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);