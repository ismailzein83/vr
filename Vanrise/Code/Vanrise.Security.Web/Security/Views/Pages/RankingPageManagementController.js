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
        $scope.addClicked = function () {
            if (treeAPI.getTree()!=undefined)
            console.log(treeAPI.getTree());
        }
        //$scope.onSelectedMenuNodechanged = function () {
        //    var settings = {};
        //    if ($scope.selectedMenuNode != undefined)
        //    {
        //        settings.onScopeReady = function (modalScope) {
        //            modalScope.title = "Ranking Page Editor: " + $scope.selectedMenuNode.Name;
        //            modalScope.onPageUpdated = function (menuItem) {
        //                var menu =[];
        //                menu.push({
        //                    Name: "Root",
        //                    Childs: menuItem,
        //                    isOpened: true
        //                });
        //                treeAPI.refreshTree(menu);
        //            };
        //        };
        //        VRModalService.showModal('/Client/Modules/Security/Views/Pages/RankingPageEditor.html', $scope.selectedMenuNode, settings);
        //    }

           
        //}
        $scope.save = function () {
            var menu = treeAPI.getTree();
            console.log(menu[0].Childs);
            return ViewAPIService.UpdateViewsRank(menu[0].Childs).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("MenuItems", response)) {
                    if ($scope.onPageUpdated != undefined)
                        $scope.onPageUpdated(response.UpdatedObject);
                }
            })

        };
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