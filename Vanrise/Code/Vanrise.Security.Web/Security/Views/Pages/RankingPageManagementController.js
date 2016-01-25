'use strict'
RankingPageManagementController.$inject = ['$scope', 'VR_Sec_ViewAPIService', 'VRNotificationService', 'UtilsService', 'VR_Sec_MenuAPIService'];
function RankingPageManagementController($scope, VR_Sec_ViewAPIService, VRNotificationService, UtilsService, VR_Sec_MenuAPIService) {
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