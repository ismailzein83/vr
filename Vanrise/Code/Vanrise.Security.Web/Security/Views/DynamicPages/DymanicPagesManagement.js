'use strict'
/// <reference path="ZoneMonitorSettings.html" />
/// <reference path="ZoneMonitor.html" />
DynamicPagesManagementController.$inject = ['$scope', 'UtilsService', 'DynamicPagesAPIService', 'AnalyticsAPIService', 'VRModalService', 'VRNotificationService'];
function DynamicPagesManagementController($scope, UtilsService, DynamicPagesAPIService, AnalyticsAPIService, VRModalService, VRNotificationService) {
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
        $scope.data = [];
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };
        $scope.menuActions = [{
            name: "Edit",
            clicked: function (dataItem) {
                updatePage(dataItem);
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
            addPage()
        };
    }
    function addPage() {

        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Dynamic Page";
            modalScope.onPageAdded = function (page) {
                mainGridAPI.itemAdded(page);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPagesEditor.html', null, settings);

    }
    function updatePage(dataItem) {
        console.log(dataItem);
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Dynamic Page: " + dataItem.Name;
            modalScope.onPageAdded = function (page) {
                mainGridAPI.itemAdded(page);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPagesEditor.html', dataItem, settings);

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
        return DynamicPagesAPIService.GetDynamicPages().then(function (response) {
            angular.forEach(response, function (itm) {   
                $scope.data.push(itm);
                });
            });
 
    }
};

appControllers.controller('Security_DynamicPagesManagementController', DynamicPagesManagementController);