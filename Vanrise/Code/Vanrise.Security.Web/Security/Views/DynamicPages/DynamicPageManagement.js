'use strict'
DynamicPageManagementController.$inject = ['$scope', 'DynamicPageAPIService', 'VRModalService', 'VRNotificationService'];
function DynamicPageManagementController($scope, DynamicPageAPIService, VRModalService, VRNotificationService) {
    var mainGridAPI;
    defineScope();
    load();
    function defineScope() {
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

    function load() {
        loadDynamicViews();
    }

    function loadDynamicViews() {
         $scope.isInitializing = true;
        return DynamicPageAPIService.GetDynamicPages().then(function (response) {
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