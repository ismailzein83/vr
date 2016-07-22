'use strict';
app.directive('retailBeActionBalancealertaccount', ['UtilsService','VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new BalanceAlertAccountActionCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/BalanceAlertAccount/Templates/BalanceAlertAccountActionTemplate.html';
            }

        };

        function BalanceAlertAccountActionCtor(ctrl, $scope) {

            var actionDefinitionAPI;
            var actionDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onActionDefinitionSelectorReady = function(api)
                {
                    actionDefinitionAPI = api;
                    actionDefinitionSelectorReadyDeferred.resolve();
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {

                    }

                    var actionDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    actionDefinitionSelectorReadyDeferred.promise.then(function () {
                        var actionDefinitionSelectorPayload = undefined;
                        if (payload) {
                            actionDefinitionSelectorPayload = {
                                selectedIds:  payload.ActionDefinitionId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(actionDefinitionAPI, actionDefinitionSelectorPayload, actionDefinitionSelectorLoadDeferred);
                    });
                    return actionDefinitionSelectorLoadDeferred.promise;
                }

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.Business.Extensions.BalanceAlertThresholds.FixedBalanceAlertThreshold, Retail.BusinessEntity.Business",
                        ActionDefinitionId:actionDefinitionAPI.getSelectedIds()
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);