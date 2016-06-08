﻿(function (app) {

    'use strict';

    GenericRuleGridDirective.$inject = ['VR_GenericData_GenericRuleAPIService', 'VR_GenericData_GenericRuleDefinitionAPIService', 'VR_GenericData_GenericRule', 'UtilsService', 'VRNotificationService'];

    function GenericRuleGridDirective(VR_GenericData_GenericRuleAPIService, VR_GenericData_GenericRuleDefinitionAPIService, VR_GenericData_GenericRule, UtilsService, VRNotificationService) {
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
                ctrl.criteriaFields = [];
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
                var directiveAPI = {};

                directiveAPI.loadGrid = function (query) {
                    var promises = [];

                    var getDefinitionPromise = VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(query.RuleDefinitionId);
                    promises.push(getDefinitionPromise);

                    var retrieveDataDeferred = UtilsService.createPromiseDeferred();
                    promises.push(retrieveDataDeferred.promise);

                    getDefinitionPromise.then(function (response) {
                        ctrl.criteriaFields.length = 0;
                        for (var i = 0; i < response.CriteriaDefinition.Fields.length; i++) {
                            ctrl.criteriaFields.push(response.CriteriaDefinition.Fields[i]);
                        }
                        gridAPI.retrieveData(query).then(function () { retrieveDataDeferred.resolve(); }).catch(function (error) { retrieveDataDeferred.reject(error); });
                    });

                    return UtilsService.waitMultiplePromises(promises);
                };

                directiveAPI.onGenericRuleAdded = function (addedGenericRule) {
                    gridAPI.itemAdded(addedGenericRule);
                };

                return directiveAPI;
            }
            
            //function defineMenuActions() {
            //    $scope.gridMenuActions = [{
            //        name: 'Edit',
            //        clicked: editGenericRule,
            //    },
            //    {
            //        name: 'Delete',
            //        clicked: deleteGenericRule,
            //    }];
            //}

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


            function deleteGenericRule(genericRule) {
                var onGenericRuleDeleted = function () {
                    gridAPI.itemDeleted(genericRule);
                };
                VR_GenericData_GenericRule.deleteGenericRule($scope, genericRule.Entity, onGenericRuleDeleted);
            }
        }
    }

    app.directive('vrGenericdataGenericruleGrid', GenericRuleGridDirective);

})(app);