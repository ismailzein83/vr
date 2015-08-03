'use strict'
RankingPageManagementController.$inject = ['$scope', 'ViewAPIService', 'VRModalService', 'VRNotificationService', 'DeleteOperationResultEnum', 'PeriodEnum', 'UtilsService', 'TimeDimensionTypeEnum', 'UsersAPIService', 'GroupAPIService', 'DataRetrievalResultTypeEnum','MenuAPIService'];
function RankingPageManagementController($scope, ViewAPIService, VRModalService, VRNotificationService, DeleteOperationResultEnum, PeriodEnum, UtilsService, TimeDimensionTypeEnum, UsersAPIService, GroupAPIService, DataRetrievalResultTypeEnum, MenuAPIService) {
    var treeAPI;
    defineScope();
    load();
    function defineScope() {
        $scope.menu = [];
       
        $scope.searchClicked = function () {
            $scope.searchClicked = function () {
                return retrieveData();
            };

        }
        $scope.selectedMenuNode;
        $scope.menuReady = function (api) {
            treeAPI = api;
            if ($scope.menu.length > 0) {
                treeAPI.refreshTree($scope.menu);
            }
        }
        $scope.onSelectedMenuNodechanged = function () {
            var settings = {};
            if ($scope.selectedMenuNode != undefined)
            {
                settings.onScopeReady = function (modalScope) {
                    modalScope.title = "Ranking Page Editor: " + $scope.selectedMenuNode.Name;
                    modalScope.onPageUpdated = function (menuItem) {
                        var menu =[];
                        menu.push({
                            Name: "Root",
                            Childs: menuItem,
                            isOpened: true
                        });
                        treeAPI.refreshTree(menu);
                    };
                };
                VRModalService.showModal('/Client/Modules/Security/Views/RankingPages/RankingPageEditor.html', $scope.selectedMenuNode, settings);
            }

           
        }
    }


    function updatePage(dataItem) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Dynamic Page: " + dataItem.Name;
            modalScope.onPageUpdated = function (view) {
                mainGridAPI.itemUpdated(fillNeededData(view));
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPageEditor.html', dataItem, settings);

    }
    function deletePage(dataItem) {

        var message = "Do you want to delete " + dataItem.Name;
        VRNotificationService.showConfirmation(message).then(function (response) {
            if (response == true) {
                return ViewAPIService.DeleteView(dataItem.ViewId).then(function (responseObject) {
                    if (responseObject.Result == DeleteOperationResultEnum.Succeeded.value) {
                        mainGridAPI.itemDeleted(fillNeededData(dataItem));
                    }

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
        loadViews();
     
    }


    function loadViews() {
        return MenuAPIService.GetMenuItems().then(function (response) {
            $scope.menuItems = [];
            angular.forEach(response, function (item) {

                $scope.menuItems.push(item);
            })
            console.log($scope.menuItems);
            var menu = {
                Name: "Root",
                Childs: $scope.menuItems,
                isOpened: true
            }
            $scope.menu.push(menu);
            if (treeAPI != undefined) {
                treeAPI.refreshTree($scope.menu);
            }
        });

    }

  

};

appControllers.controller('Security_RankingPageManagementController', RankingPageManagementController);