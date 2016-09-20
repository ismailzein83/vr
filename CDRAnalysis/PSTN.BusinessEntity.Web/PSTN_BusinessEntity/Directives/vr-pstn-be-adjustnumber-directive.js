"use strict";

app.directive("vrPstnBeAdjustnumber", ["NormalizationRuleAPIService", "UtilsService", "VRNotificationService", "VRUIUtilsService", function (NormalizationRuleAPIService, UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObj = {
        restrict: "E",
        scope: {
            onReady: "="
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
            defineAPI();
        }

        // private members

        var actionSettingsDirectiveAPI;

        function defineScope() {

            $scope.actionTemplates = [];
            $scope.selectedActionTemplate = undefined;
            $scope.disableAddButton = true;
            $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };
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

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                $scope.loadingActionTemplates = true;

                return NormalizationRuleAPIService.GetNormalizationRuleAdjustNumberActionSettingsTemplates()
                    .then(function (response) {
                        angular.forEach(response, function (item) {
                            $scope.actionTemplates.push(item);
                        });

                        var promises = [];
                        if (payload != undefined) {
                            angular.forEach(payload.Actions, function (item) {
                                var action = getActionItem(item);
                                action.DirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                                promises.push(action.DirectiveLoadPromiseDeferred.promise);
                                action.ReadyPromiseDeferred.promise.then(function () {
                                    VRUIUtilsService.callDirectiveLoad(action.ActionDirectiveAPI, action.Data, action.DirectiveLoadPromiseDeferred);
                                });

                                $scope.actions.push(action);
                            });


                            UtilsService.waitMultiplePromises(promises).then(function () {
                                $scope.loadingActionTemplates = false;
                            }).catch(function (error) {
                                $scope.loadingActionTemplates = false;
                                VRNotificationService.notifyExceptionWithClose(error, $scope);
                            });
                        }

                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    })
                    .finally(function () {
                        $scope.loadingActionTemplates = false;
                    });
            };


            api.getData = function () {
                return {
                    $type: "PSTN.BusinessEntity.Entities.NormalizationRuleAdjustNumberSettings, PSTN.BusinessEntity.Entities",
                    Actions: ($scope.actions.length > 0) ? getActions() : null
                };
            };

            api.validateData = function () {

                if ($scope.actions.length == 0)
                    return false;

                for (var i = 0; i < $scope.actions.length; i++) {
                    var action = $scope.actions[i];

                    if (action.ActionDirectiveAPI == undefined ||!action.ActionDirectiveAPI.validateData() )
                        return false;
                }

                return true;
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }


            function getActions() {
                var actions = [];

                angular.forEach($scope.actions, function (item) {

                    var action = item.ActionDirectiveAPI.getData();
                    action.ConfigId = item.ConfigId;

                    actions.push(action);
                });

                return actions;
            }
        }


        function getActionItem(dbAction) {

            var actionItem = {
                ActionId: $scope.actions.length + 1,

                ConfigId: (dbAction != null) ? dbAction.ConfigId : $scope.selectedActionTemplate.ExtensionConfigurationId,

                Editor: (dbAction != null) ?
                    UtilsService.getItemByVal($scope.actionTemplates, dbAction.ConfigId, "ExtensionConfigurationId").Editor :
                    $scope.selectedActionTemplate.Editor,

                Data: (dbAction != null) ? dbAction : {},

                ReadyPromiseDeferred: UtilsService.createPromiseDeferred(),

                DirectiveLoadPromiseDeferred: null
            };



            actionItem.onActionDirectiveAPIReady = function (api) {
                actionItem.ActionDirectiveAPI = api;
                actionItem.ReadyPromiseDeferred.resolve();
            }

            return actionItem;
        }
    }

    return directiveDefinitionObj;

}]);
