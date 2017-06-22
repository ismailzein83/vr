﻿(function (app) {

    'use strict';

    ActionsViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_ActionDefinitionService', 'Retail_BE_EntityTypeEnum'];

    function ActionsViewDirective(UtilsService, VRNotificationService, Retail_BE_ActionDefinitionService, Retail_BE_EntityTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ActionsViewCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountViews/MainExtensions/Templates/ActionsViewTemplate.html'
        };

        function ActionsViewCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var parentAccountId;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        parentAccountId = payload.parentAccountId;
                    }

                    return gridAPI.loadGrid(buildGridPayload(payload));
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildGridPayload(loadPayload) {
                var entityIds = [];
                var entityId = 'Retail_BE_' + accountBEDefinitionId + '_' + parentAccountId;
                entityIds.push(entityId);
                var actionGridPayload = {
                    accountBEDefinitionId: accountBEDefinitionId,
                    EntityIds: entityIds
                };
                return actionGridPayload;
            }
        }
    }

    app.directive('retailBeActionsView', ActionsViewDirective);

})(app);