﻿'use strict'
WidgetManagementController.$inject = ['$scope', 'UtilsService', 'WidgetAPIService', 'VRModalService', 'VRNotificationService','DeleteOperationResultEnum'];
function WidgetManagementController($scope, UtilsService, WidgetAPIService, VRModalService, VRNotificationService, DeleteOperationResultEnum) {
    var mainGridAPI;
    defineScope();
    load();

    function defineScope() {
        $scope.filterValue="";
        $scope.widgets = [];
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };
        $scope.menuActions = [{
                name: "Edit",
                clicked: function (dataItem) {
                 updateWidget(dataItem);
                }
        }, {
            name: "Delete",
            clicked: function (dataItem) {
                deleteWidget(dataItem);
            }
        }];

        $scope.mainGridPagerSettings = {
            currentPage: 1,
            totalDataCount: 0,
            pageChanged: function () {
                return getData();
            }
        };

        $scope.Add = function () {
            addNewWidget();
        };
        $scope.searchClicked=function(){
            if ($scope.filterValue != undefined && $scope.filterValue) {
                $scope.isGettingData = true;
                return WidgetAPIService.GetFilteredWidgets($scope.filterValue).then(function (response) {
                    $scope.widgets.length = 0;
                    angular.forEach(response, function (itm) {
                        $scope.widgets.push(itm);
                    });
                }).finally(function () {
                    $scope.isGettingData = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope)
                });

            }
            else
                loadData();

        }

    }
    function deleteWidget(dataItem) {
        var message = "Do you want to delete " + dataItem.Name;
        VRNotificationService.showConfirmation(message).then(function (response) {
            if (response == true) {
                return WidgetAPIService.DeleteWidget(dataItem.Id).then(function (responseObject) {
                    if (responseObject.Result == DeleteOperationResultEnum.Succeeded.value)
                        mainGridAPI.itemDeleted(dataItem);
                    VRNotificationService.notifyOnItemDeleted("Widget", responseObject);
                    $scope.isGettingData = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    
                   
                });
            }

        });


    }
    function addNewWidget() {
        var settings = {};
        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Widget";
            modalScope.onWidgetAdded = function (widget) {
                mainGridAPI.itemAdded(widget);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/WidgetsPages/WidgetEditor.html', null, settings);

    }

    function updateWidget(dataItem) {
        var settings = {};
        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Update Widget:" + dataItem.Name;
            modalScope.onWidgetUpdated = function (widget) {
                mainGridAPI.itemUpdated(widget);
            };
        };

        VRModalService.showModal('/Client/Modules/Security/Views/WidgetsPages/WidgetEditor.html', dataItem, settings);
    }

    function load() {
        loadData();
    }
    function loadData() {
        $scope.isGettingData = true;
        return WidgetAPIService.GetAllWidgets().then(function (response) {
            $scope.widgets.length = 0;
            angular.forEach(response, function (itm) {
                $scope.widgets.push(itm);
            });
        }).finally(function () {
            $scope.isGettingData = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope)});

    }
    
};

appControllers.controller('Security_WidgetManagementController', WidgetManagementController);