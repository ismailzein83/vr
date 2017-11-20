﻿"use strict";

app.directive("retailBeAccountmanagerAccountsubviewruntimeSearch", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Retail_BE_AccountManagerAssignmentService",
function (UtilsService, VRNotificationService, VRUIUtilsService, Retail_BE_AccountManagerAssignmentService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var subViewsRuntimeSearch = new SubViewsRuntimeSearch($scope, ctrl, $attrs);
            subViewsRuntimeSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Extensions/AccountManager/Templates/AccountSubViewRuntimeSearchTemplate.html"
    };

    function SubViewsRuntimeSearch($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        var accountManagerAssignmentGridAPI;
        var gridPayload;
        
        function initializeController() {
            $scope.onAccountManagerAssignmentGridReady = function (api) {
                accountManagerAssignmentGridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveAPI());
                }
            };
            $scope.addAccountManagerAssignments = function () {
                var onAccountManagerAssignmentAdded = function (accountManagerAssignment) {
                    accountManagerAssignmentGridAPI.onAccountManagerAssignmentAdded(accountManagerAssignment);
                };
                Retail_BE_AccountManagerAssignmentService.addAccountManagerAssignments(gridPayload.accountManagerDefinitionId, gridPayload.accountManagerId, gridPayload.accountManagerSubViewDefinition.Settings.AccountManagerAssignementDefinitionId, onAccountManagerAssignmentAdded);
            };
            function getDirectiveAPI() {
                var directiveAPI = {};
                directiveAPI.load = function (payload) {
                    gridPayload = payload;
                    accountManagerAssignmentGridAPI.loadGrid(getGridPayload());
                };
                directiveAPI.getData = function () {

                };
                return directiveAPI;
            }
            function getGridPayload() {
                var payload = {
                    accountManagerDefinitionId: gridPayload.accountManagerDefinitionId,
                    accountManagerId: gridPayload.accountManagerId,
                    accountManagerSubViewDefinition: gridPayload.accountManagerSubViewDefinition
                };
                return payload;
            }
        }
    }
    return directiveDefinitionObject;
}]);