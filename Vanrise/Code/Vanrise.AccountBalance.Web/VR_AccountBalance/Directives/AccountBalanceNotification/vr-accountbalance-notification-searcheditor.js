"use strict";

app.directive("vrAccountbalanceNotificationSearcheditor", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ViewEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_AccountBalance/Directives/AccountBalanceNotification/Templates/AccountBalanceNotificationSearchEditor.html"
        };
        function ViewEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            function initializeController() {
                $scope.scopeModel = {};
                
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];



                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                     
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);