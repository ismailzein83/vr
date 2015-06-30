'use strict'
WidgetManagementController.$inject = ['$scope', 'UtilsService', 'WidgetAPIService', 'VRModalService', 'VRNotificationService'];
function WidgetManagementController($scope, UtilsService, WidgetAPIService, VRModalService, VRNotificationService) {
    var mainGridAPI;
    defineScope();
    load();

    function defineScope() {
        $scope.widgets = [];
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };
        $scope.menuActions = [{
                name: "Edit",
                clicked: function (dataItem) {
                 updateWidget(dataItem);
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


    }

    function addNewWidget() {
        var settings = {};
        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Widget";
            modalScope.onWidgetAdded = function (widget) {
                console.log(widget);
                mainGridAPI.itemAdded(widget);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/WidgetsPages/WidgetEditor.html', null, settings);

    }

    function updateWidget(dataItem) {
        var settings = {};
        console.log(dataItem);
        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Update Widget:" + dataItem.Name;
            modalScope.onWidgetUpdated = function (widget) {
                mainGridAPI.itemUpdated(widget);
            };
        };

        VRModalService.showModal('/Client/Modules/Security/Views/WidgetsPages/WidgetEditor.html', dataItem, settings);
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
        $scope.isGettingData = true;
        return WidgetAPIService.GetAllWidgets().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.widgets.push(itm);
            });
        }).finally(function () {
            $scope.isGettingData = false;
        });

    }
    
};

appControllers.controller('Security_WidgetManagementController', WidgetManagementController);