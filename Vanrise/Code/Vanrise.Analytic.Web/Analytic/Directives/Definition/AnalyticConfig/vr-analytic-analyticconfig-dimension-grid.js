﻿"use strict";

app.directive("vrAnalyticAnalyticconfigDimensionGrid", ['VRNotificationService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigService', 'UtilsService', 'VR_Analytic_AnalyticItemConfigAPIService', 'VR_Analytic__AnalyticTypeEnum', function (VRNotificationService, VRModalService, VR_Analytic_AnalyticItemConfigService, UtilsService, VR_Analytic_AnalyticItemConfigAPIService, VR_Analytic__AnalyticTypeEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var analyticDimensionGrid = new AnalyticDimensionGrid($scope, ctrl, $attrs);
            analyticDimensionGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticConfig/Templates/AnalyticDimensionGridTemplate.html"

    };

    function AnalyticDimensionGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.dimensions = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onAnalyticDimensionAdded = function (dimensionObj) {
                        gridAPI.itemAdded(dimensionObj);
                    }
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Analytic_AnalyticItemConfigAPIService.GetFilteredDimensions(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editDimension,
            }];
        }

        function editDimension(dataItem) {
            var onEditDimension = function (dimensionObj) {
                gridAPI.itemUpdated(dimensionObj);
            }

            VR_Analytic_AnalyticItemConfigService.editItemConfig(dataItem.Entity.AnalyticItemConfigId, onEditDimension);

        }


    }

    return directiveDefinitionObject;

}]);