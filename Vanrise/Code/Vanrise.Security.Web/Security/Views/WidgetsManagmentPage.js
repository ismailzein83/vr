'use strict'
/// <reference path="ZoneMonitorSettings.html" />
/// <reference path="ZoneMonitor.html" />
WidgetsManagementController.$inject = ['$scope', 'UtilsService', 'WidgetAPIService', 'AnalyticsAPIService', 'VRModalService', 'VRNotificationService'];
function WidgetsManagementController($scope, UtilsService, WidgetAPIService, AnalyticsAPIService, VRModalService, VRNotificationService) {
    var filter = {};
    var mainGridAPI;
    var sortColumn;
    var sortDescending = true;
    var currentData;
    var currentSortedColDef;
    defineScopeObjects();
    load();
    //  defineScopeMethods();

    function defineScopeObjects() {
        $scope.widgets = [];
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };
        $scope.menuActions = [{
            name: "Edit",
            clicked: function (dataItem) {
                AddPage();
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
            AddWidget();
        };


    }
    function AddWidget() {

        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Widget";
            modalScope.onWidgetAdded = function (widget) {
                mainGridAPI.itemAdded(widget);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/WidgetsPages/WidgetEditor.html', null, settings);

    }

    function load() {
        $scope.isInitializing = true;
        UtilsService.waitMultipleAsyncOperations([loadData]).finally(function () {
            $scope.isInitializing = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
    }

    function loadData() {
        return WidgetAPIService.GetAllWidgets().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.widgets.push(itm);
            });
        });

    }
};

appControllers.controller('Security_WidgetsManagementController', WidgetsManagementController);