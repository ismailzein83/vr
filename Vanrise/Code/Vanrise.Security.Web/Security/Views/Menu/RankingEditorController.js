'use strict'
RankingEditorController.$inject = ['$scope', 'VR_Sec_ViewAPIService', 'VRNotificationService', 'UtilsService', 'VR_Sec_MenuAPIService'];
function RankingEditorController($scope, VR_Sec_ViewAPIService, VRNotificationService, UtilsService, VR_Sec_MenuAPIService) {
    var treeAPI;
    defineScope();
    load();
    function defineScope() {
        $scope.menu = [];

        $scope.selectedMenuNode;
        $scope.menuReady = function (api) {
            treeAPI = api;
            if ($scope.menu.length > 0) {
                treeAPI.refreshTree($scope.menu);
            }
        }
        $scope.save = function () {
            if (treeAPI.getTree != undefined) {
                var menu = treeAPI.getTree();
                return VR_Sec_ViewAPIService.UpdateViewsRank(menu[0].Childs).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("MenuItems", response)) {
                        if ($scope.onPageUpdated != undefined)
                            $scope.onPageUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                })
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal();
        };
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls().finally(function () {
            $scope.isLoading = false;
        });
    }
    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([setTitle, loadViews])
           .catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           })
          .finally(function () {
              $scope.scopeModal.isLoading = false;
          });
    }
    function setTitle() {

        $scope.title = UtilsService.buildTitleForAddEditor('Ranking MenuItems');
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

appControllers.controller('VR_Sec_RankingEditorController', RankingEditorController);