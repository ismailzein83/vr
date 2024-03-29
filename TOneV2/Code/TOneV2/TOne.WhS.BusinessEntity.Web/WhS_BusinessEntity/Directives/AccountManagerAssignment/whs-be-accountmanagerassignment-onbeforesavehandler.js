﻿"use strict";
app.directive("whsBeAccountmanagerassignmentOnbeforesavehandler", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {
        return {

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
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/AccountManagerAssignment/Templates/AccountManagerAssignmentOnBeforeSaveHandler.html"
        };

        function Handler($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    return;
                };

                api.getData = function () {

                    return {
                        $type: "TOne.WhS.BusinessEntity.Business.AccountManagerAssignmentOnBeforeSaveHandler, TOne.WhS.BusinessEntity.Business"
                    };
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }
    }
]);