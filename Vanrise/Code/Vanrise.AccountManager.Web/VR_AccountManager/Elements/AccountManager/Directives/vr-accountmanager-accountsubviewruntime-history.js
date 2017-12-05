"use strict";

app.directive("vrAccountmanagerAccountsubviewruntimeHistory", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Retail_BE_AccountManagerAssignmentService", "Retail_BE_AccountManagerAssignmentAPIService",
function (UtilsService, VRNotificationService, VRUIUtilsService, Retail_BE_AccountManagerAssignmentService, Retail_BE_AccountManagerAssignmentAPIService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var subViewRuntimeHistory = new SubViewRuntimeHistory($scope, ctrl, $attrs);
            subViewRuntimeHistory.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_AccountManager/Elements/AccountManager/Directives/Template/AccountManagerSubViewRuntimeHistoryTemplate.html",
    };

    function SubViewRuntimeHistory($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        var historyGridAPI;
        var accountManagerId;
        var accountManagerDefinitionId
        function initializeController() {
            $scope.onHistoryGridReady = function (api) {
                historyGridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveAPI());
                }
            };
            function getDirectiveAPI() {
                var directiveAPI = {};
                directiveAPI.load = function (payload) {
                    if (payload != undefined) {
                        accountManagerDefinitionId = payload.accountManagerDefinitionId;
                        accountManagerId = payload.accountManagerId;
                    }
                    var query = {
                        ObjectId: accountManagerId,
                        EntityUniqueName: "VR_AccountManager_AccountManager_" + accountManagerDefinitionId,
                    };
                    historyGridAPI.load(query);
                };
                directiveAPI.getData = function () {

                };
                return directiveAPI;
            }
        }
    }
    return directiveDefinitionObject;
}]);