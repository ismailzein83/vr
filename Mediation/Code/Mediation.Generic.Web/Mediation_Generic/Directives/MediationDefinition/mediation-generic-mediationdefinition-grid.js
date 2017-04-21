"use strict";

app.directive("mediationGenericMediationdefinitionGrid", ["UtilsService", "VRNotificationService", "Mediation_Generic_MediationDefinitionAPIService", "Mediation_Generic_MediationDefinitionService",
    function (UtilsService, VRNotificationService, Mediation_Generic_MediationDefinitionAPIService, Mediation_Generic_MediationDefinitionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var mediationDefinitionGrid = new MediationDefinitionGrid($scope, ctrl, $attrs);
                mediationDefinitionGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Mediation_Generic/Directives/MediationDefinition/Templates/MediationDefinitionGrid.html"

        };

        function MediationDefinitionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            function initializeController() {

                $scope.mediationDefinitions = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        directiveAPI.onMediationDefinitionAdded = function (onMediationDefinitionObj) {
                            gridAPI.itemAdded(onMediationDefinitionObj);
                        };

                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Mediation_Generic_MediationDefinitionAPIService.GetFilteredMediationDefinitions(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined) {
                            }
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };

                defineMenuActions();
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editMediationDefinition,
                    haspermission: hasEditMediationDefinitionPermission
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function hasEditMediationDefinitionPermission() {
                return Mediation_Generic_MediationDefinitionAPIService.HasUpdateMediationDefinition()
            }
            function editMediationDefinition(dataItem) {
                var onMediationDefinitionUpdated = function (mediationDefinitionObj) {
                    gridAPI.itemUpdated(mediationDefinitionObj);
                };

                Mediation_Generic_MediationDefinitionService.editMediationDefinition(dataItem.Entity.MediationDefinitionId, onMediationDefinitionUpdated);
            }
        }

        return directiveDefinitionObject;

    }
]);