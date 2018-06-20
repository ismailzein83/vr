"use strict";

app.directive("vrSecSecurityproviderBeforeinserthandler", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new Handler($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: ""
        };

        function Handler($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {

                    return {
                        $type: "Vanrise.Security.MainExtensions.GenericBEOnBeforeInsertHandlers.SecurityProviderOnBeforeInsertHandler, Vanrise.Security.MainExtensions"
                    };
                };

                api.load = function (payload) {
                    if (payload != undefined) {

                    }
                };
                
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);