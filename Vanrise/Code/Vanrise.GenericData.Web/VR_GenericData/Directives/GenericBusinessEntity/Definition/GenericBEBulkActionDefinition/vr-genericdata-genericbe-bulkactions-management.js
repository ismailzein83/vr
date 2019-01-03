"use strict";

app.directive("vrGenericdataGenericbeBulkactionsManagement", ["UtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService",
    function (UtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBEBulkActions($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEBulkActionDefinition/Templates/GenericBEBulkActionsManagementTemplate.html'

        };

        function GenericBEBulkActions($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var genericBEDefinitionId;
            var gridAPI;
            var context;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.addGenericBEBulkAction = function () {
                    var onGenericBEBulkActionAdded = function (genericBEBulkAction) {
                        ctrl.datasource.push({ Entity: genericBEBulkAction });
                    };
                    VR_GenericData_GenericBEDefinitionService.addGenericBEBulkActionDefinition(genericBEDefinitionId, onGenericBEBulkActionAdded, context);
                };

                ctrl.removeGenericBEBulkAction = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };

                defineMenuActions();
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                        genericBEDefinitionId = payload.genericBEDefinitionId;
                        if (payload.genericBEBulkActions != undefined) {
                            for (var i = 0; i < payload.genericBEBulkActions.length; i++) {
                                var genericBEBulkAction = payload.genericBEBulkActions[i];
                                ctrl.datasource.push({ Entity: genericBEBulkAction });
                            }
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    var genericBEBulkActions;
                    if (ctrl.datasource != undefined) {
                        genericBEBulkActions = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            genericBEBulkActions.push(currentItem.Entity);
                        }
                    }
                    return genericBEBulkActions;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                        clicked: editGenericBEBulkActionDefinition,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }
            function editGenericBEBulkActionDefinition(genericBEBulkActionObj) {
                var onGenericBEBulkActionUpdated = function (genericBEBulkAction) {
                    var index = ctrl.datasource.indexOf(genericBEBulkActionObj);
                    ctrl.datasource[index] = { Entity: genericBEBulkAction };
                };

                VR_GenericData_GenericBEDefinitionService.editGenericBEBulkActionDefinition(genericBEBulkActionObj.Entity, genericBEDefinitionId, onGenericBEBulkActionUpdated, context);
            }
        }

        return directiveDefinitionObject;

    }
]);