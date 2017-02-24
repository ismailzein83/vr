"use strict";

app.directive("vrAccountbalanceNotificationBodyeditor", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
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
            templateUrl: "/Client/Modules/VR_AccountBalance/Directives/AccountBalanceNotification/Templates/AccountBalanceNotificationBodyEditor.html"
        };

        function ViewEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    gridAPI.loadGrid(payload);

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