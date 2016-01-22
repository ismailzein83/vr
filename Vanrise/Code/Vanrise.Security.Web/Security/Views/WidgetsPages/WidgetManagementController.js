﻿'use strict'
widgetManagementController.$inject = ['$scope', 'UtilsService', 'VR_Sec_WidgetDefinitionAPIService', 'VRNotificationService', 'VR_Sec_WidgetService'];

function widgetManagementController($scope, UtilsService, VR_Sec_WidgetDefinitionAPIService, VRNotificationService, VR_Sec_WidgetService) {
    var mainGridAPI;
    defineScope();
    load();

    function defineScope() {
        $scope.widgetsTypes = [];
        $scope.selectedWidgetsTypes = [];
        $scope.onGridReady = function (api) {
            mainGridAPI = api;
            var filter = {};
            api.loadGrid(filter);
        };

        $scope.Add = addNewWidget;

        $scope.searchClicked = function () {
            if (mainGridAPI != undefined)
                return mainGridAPI.loadGrid(getFilterObject());
        }

    }

    function getFilterObject() {

        var query = {
            WidgetName: $scope.widgetName,
            WidgetTypes: UtilsService.getPropValuesFromArray($scope.selectedWidgetsTypes, "ID")
        }
        return query;
    }

    function addNewWidget() {
        var onWidgetAdded = function (widgetObj) {
            if (mainGridAPI != undefined)
                mainGridAPI.onWidgetAdded(widgetObj);
        };
        VR_Sec_WidgetService.addWidget(onWidgetAdded);
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {

        return loadWidgets()
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            })
            .finally(function () {
                $scope.isLoading = false;
            });
    }

    function loadWidgets() {
        return VR_Sec_WidgetDefinitionAPIService.GetWidgetsDefinition()
            .then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.widgetsTypes.push(itm);
                });
            });

    }

};

appControllers.controller('VR_Sec_WidgetManagementController', widgetManagementController);