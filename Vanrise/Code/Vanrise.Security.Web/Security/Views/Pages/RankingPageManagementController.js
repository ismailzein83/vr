﻿'use strict'
RankingPageManagementController.$inject = ['$scope', 'VR_Sec_ViewAPIService', 'VRModalService', 'VRNotificationService', 'DeleteOperationResultEnum', 'PeriodEnum', 'UtilsService', 'TimeDimensionTypeEnum', 'UsersAPIService', 'VR_Sec_GroupAPIService', 'DataRetrievalResultTypeEnum', 'VR_Sec_MenuAPIService'];
function RankingPageManagementController($scope, VR_Sec_ViewAPIService, VRModalService, VRNotificationService, DeleteOperationResultEnum, PeriodEnum, UtilsService, TimeDimensionTypeEnum, UsersAPIService, VR_Sec_GroupAPIService, DataRetrievalResultTypeEnum, VR_Sec_MenuAPIService) {
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
            if (treeAPI.getTree!= undefined) {
                var menu = treeAPI.getTree();
                console.log(menu);
                console.log(menu[0].Childs);
                return VR_Sec_ViewAPIService.UpdateViewsRank(menu[0].Childs).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("MenuItems", response)) {
                        if ($scope.onPageUpdated != undefined)
                            $scope.onPageUpdated(response.UpdatedObject);
                    }
                })
            }
            

        };
    }




    function load() {
        loadViews();
     
    }


    function loadViews() {
        return VR_Sec_MenuAPIService.GetMenuItems().then(function (response) {
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