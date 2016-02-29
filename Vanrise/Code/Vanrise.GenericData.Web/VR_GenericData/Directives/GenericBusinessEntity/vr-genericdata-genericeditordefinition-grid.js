"use strict";

app.directive("vrGenericdataGenericeditordefinitionGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_GenericEditorAPIService", "VR_GenericData_GenericEditorService","VRUIUtilsService",
    function (UtilsService, VRNotificationService, VR_GenericData_GenericEditorAPIService, VR_GenericData_GenericEditorService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new GenericBusinessEntityDefinitionGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Templates/GenericEditorDefinitionGrid.html"

        };

        function GenericBusinessEntityDefinitionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            function initializeController() {

                $scope.genericEditorDefinitions = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        }
                        directiveAPI.onGenericEditorAdded = function (genericEditorObj) {
                            gridAPI.itemAdded(genericEditorObj);
                        }
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_GenericEditorAPIService.GetFilteredGenericEditorDefinitions(dataRetrievalInput)
                        .then(function (response) {
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
                    clicked: editGenericEditor,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editGenericEditor(dataItem) {
                var onGenericEditorUpdated = function (genericEditorObj) {
                    gridAPI.itemUpdated(genericEditorObj);
                }

                VR_GenericData_GenericEditorService.editGenericEditor(dataItem.Entity.GenericEditorDefinitionId, onGenericEditorUpdated);
            }
        }

        return directiveDefinitionObject;

    }
]);