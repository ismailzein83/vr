'use strict'
/// <reference path="ZoneMonitorSettings.html" />
/// <reference path="ZoneMonitor.html" />
DynamicPagesManagementController.$inject = ['$scope', 'UtilsService', 'DynamicPagesManagementAPIService', 'AnalyticsAPIService', 'VRModalService', 'VRNotificationService'];
function DynamicPagesManagementController($scope, UtilsService, DynamicPagesManagementAPIService, AnalyticsAPIService, VRModalService, VRNotificationService) {
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
                AddPage();
                //var modalSettings = {
                //    useModalTemplate: false,
                //    width: "80%",
                //    maxHeight: "800px"
                //};
                //VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPagesEditor.html', null, modalSettings);
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

            AddPage()

            //var modalSettings = {
            //    useModalTemplate: false,
            //    width: "80%",
            //    maxHeight: "800px"
            //};
            //VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPagesEditor.html', null, modalSettings);
        };


    }
    function AddPage() {

        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Dynamic Page";
            modalScope.onPageAdded = function (page) {
                mainGridAPI.itemAdded(page);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPagesEditor.html', null, settings);

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
        return DynamicPagesManagementAPIService.GetDynamicPages().then(function (response) { 
            angular.forEach(response, function (itm) {   
                $scope.data.push(itm);
                });
            });
 
    }
};

appControllers.controller('Security_DynamicPagesManagementController', DynamicPagesManagementController);