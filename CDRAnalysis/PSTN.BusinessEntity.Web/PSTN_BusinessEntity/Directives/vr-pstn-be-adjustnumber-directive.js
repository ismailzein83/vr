"use strict";

app.directive("vrPstnBeAdjustnumber", ["NormalizationRuleAPIService", "UtilsService", "VRNotificationService", function (NormalizationRuleAPIService, UtilsService, VRNotificationService) {

    var directiveDefinitionObj = {
        restrict: "E",
        scope: {
            configid: "=",
            onloaded: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: "/Client/Modules/PSTN_BusinessEntity/Directives/Templates/AdjustNumberDirectiveTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        // public members

        this.initializeController = initializeController;

        function initializeController() {
            defineScope();

            loadActionTemplates().then(function () {
                defineAPI();
            });
        }

        // private members

        var actionSettingsDirectiveAPI;

        function defineScope() {

            $scope.actionTemplates = [];
            $scope.selectedActionTemplate = undefined;
            $scope.disableAddButton = true;

            $scope.actions = [];

            $scope.onActionTemplateChanged = function () {
                $scope.disableAddButton = ($scope.selectedActionTemplate == undefined);
            };

            $scope.addAction = function () {
                var action = getActionItem(null);
                $scope.actions.push(action);
            };

            $scope.removeAction = function ($event, action) {
                $event.preventDefault();
                $event.stopPropagation();

                var index = UtilsService.getItemIndexByVal($scope.actions, action.ActionId, 'ActionId');
                $scope.actions.splice(index, 1);
            };
        }

        function loadActionTemplates() {
            $scope.loadingActionTemplates = true;

            return NormalizationRuleAPIService.GetNormalizationRuleAdjustNumberActionSettingsTemplates()
                .then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.actionTemplates.push(item);
                    });
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.loadingActionTemplates = false;
                });
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return {
                    $type: "PSTN.BusinessEntity.Entities.NormalizationRuleAdjustNumberSettings, PSTN.BusinessEntity.Entities",
                    Actions: ($scope.actions.length > 0) ? getActions() : null
                };
            }

            api.setData = function (adjustNumberSettings) {
                if (adjustNumberSettings == undefined || adjustNumberSettings == null)
                    return;

                angular.forEach(adjustNumberSettings.Actions, function (item) {
                    var action = getActionItem(item);
                    $scope.actions.push(action);
                });
            }

            if (ctrl.onloaded != null)
                ctrl.onloaded(api);

            function getActions() {
                var actionList = [];

                angular.forEach($scope.actions, function (item) {
                    actionList.push(item.ActionDirectiveAPI.getData());
                });

                return actionList;
            }
        }

        function getActionItem(dbAction) {

            var actionItem = {
                ActionId: $scope.actions.length + 1,

                ConfigId: (dbAction != null) ? dbAction.ConfigId : $scope.selectedActionTemplate.TemplateConfigID,

                Editor: (dbAction != null) ?
                    UtilsService.getItemByVal($scope.actionTemplates, dbAction.ConfigId, "TemplateConfigID").Editor :
                    $scope.selectedActionTemplate.Editor,

                Data: (dbAction != null) ? dbAction : {}
            };

            actionItem.onActionDirectiveAPILoaded = function (api) {
                actionItem.ActionDirectiveAPI = api;
                actionItem.ActionDirectiveAPI.setData(actionItem.Data);

                actionItem.Data = undefined;
                actionItem.onActionDirectiveAPILoaded = undefined;
            }

            return actionItem;
        }
    }

    return directiveDefinitionObj;
    
}]);
