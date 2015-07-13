'use strict'
DynamicPageManagementController.$inject = ['$scope', 'ViewAPIService', 'VRModalService', 'VRNotificationService','DeleteOperationResultEnum'];
function DynamicPageManagementController($scope, ViewAPIService, VRModalService, VRNotificationService, DeleteOperationResultEnum) {
    var mainGridAPI;
    defineScope();
    load();
    function defineScope() {
        $scope.filterValue;
        $scope.dynamicViews = [];
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };
        $scope.menuActions = [
           {
               name: "Edit",
               clicked: function (dataItem) {
                   updatePage(dataItem);
               }
           },
           {
               name: "Delete",
               clicked: function (dataItem) {
                   deletePage(dataItem);
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
        $scope.searchClicked = function () {
          
            if ($scope.filterValue != undefined && $scope.filterValue) {
                $scope.isGettingData = true;
                return ViewAPIService.GetFilteredDynamicPages($scope.filterValue).then(function (response) {
                    $scope.dynamicViews.length = 0;
                    angular.forEach(response, function (itm) {
                        $scope.dynamicViews.push(itm);
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.isGettingData = false;
                });
            }
            else
            loadData();
                
        }
    }

    function addPage() {
        var settings = {};
        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Dynamic Page";
            modalScope.onPageAdded = function (page) {
                mainGridAPI.itemAdded(page);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPageEditor.html', null, settings);
    }

    function updatePage(dataItem) {
        var settings = {};
        
        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Dynamic Page: " + dataItem.Name;
            modalScope.onPageUpdated = function (page) {
                mainGridAPI.itemUpdated(page);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPageEditor.html', dataItem, settings);

    }
    function deletePage(dataItem) {
        var message = "Do you want to delete " + dataItem.Name;
        VRNotificationService.showConfirmation(message).then(function (response) {
            if (response == true) {
                return ViewAPIService.DeleteView(dataItem.ViewId).then(function (responseObject) {
                    if (responseObject.Result == DeleteOperationResultEnum.Succeeded.value)
                        mainGridAPI.itemDeleted(dataItem);
                    VRNotificationService.notifyOnItemDeleted("View", responseObject);
                    $scope.isGettingData = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                });
          }

        });
      

    }

    function load() {
        loadDynamicViews();
    }

    function loadDynamicViews() {
        $scope.isInitializing = true;
        return ViewAPIService.GetDynamicPages().then(function (response) {
            $scope.dynamicViews.length = 0;
            angular.forEach(response, function (itm) {   
                $scope.dynamicViews.push(itm);
                });
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        }).finally(function () {
            $scope.isGettingData = false;
        });
 
    }

};

appControllers.controller('Security_DynamicPageManagementController', DynamicPageManagementController);