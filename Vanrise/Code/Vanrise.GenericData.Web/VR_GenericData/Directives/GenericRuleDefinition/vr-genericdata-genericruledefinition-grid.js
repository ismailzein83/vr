﻿(function (app) {

    'use strict';

    GenericRuleDefinitionGridDirective.$inject = ['VR_GenericData_GenericRuleAPIService', 'VR_GenericData_GenericRule', 'VRNotificationService'];

    function GenericRuleDefinitionGridDirective(VR_GenericData_GenericRuleAPIService, VR_GenericData_GenericRule, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var obj = new GenericRuleGrid($scope, ctrl, $attrs);
                obj.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericRule/Templates/GenericRuleGridTemplate.html'
        };

        function GenericRuleGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.genericRules = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_GenericRuleAPIService.GetFilteredGenericRules(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onGenericRuleDefinitionAdded = function (addedGenericRuleDefinition) {
                    gridAPI.itemAdded(addedGenericRuleDefinition);
                };

                return api;
            }

            function defineMenuActions() {
                $scope.gridMenuActions = [{
                    name: 'Edit',
                    clicked: editGenericRule,
                }];
            }

            function editGenericRule(genericRule) {
                var onGenericRuleUpdated = function (updatedGenericRule) {
                    gridAPI.itemUpdated(updatedGenericRule);
                };

                VR_GenericData_GenericRule.editGenericRule(genericRule.Entity.RuleId, genericRule.Entity.DefinitionId, onGenericRuleUpdated);
            }
        }
    }

    app.directive('vrGenericdataGenericruledefinitionGrid', GenericRuleDefinitionGridDirective);

})(app);