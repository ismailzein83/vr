RankPageEditorController.$inject = ['$scope', 'MenuAPIService', 'WidgetAPIService', 'GroupAPIService', 'UsersAPIService', 'ViewAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WidgetSectionEnum', 'PeriodEnum', 'TimeDimensionTypeEnum', 'ColumnWidthEnum', 'VRModalService'];

function RankPageEditorController($scope, MenuAPIService, WidgetAPIService, GroupAPIService, UsersAPIService, ViewAPIService, UtilsService, VRNotificationService, VRNavigationService, WidgetSectionEnum, PeriodEnum, TimeDimensionTypeEnum, ColumnWidthEnum, VRModalService) {
    loadParameters();
    var treeAPI;
    defineScope();
    load();
    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != null) {
            $scope.menu = {
                Name: parameters.Name,
                Childs: parameters.Childs,
                isOpened: true

            }
        }
    }
    function defineScope() {
        $scope.selectedMenuNode;
        $scope.menuReady = function (api) {
            treeAPI = api;
            if ($scope.menu.Childs.length > 0) {
                var menu = [];
                menu.push($scope.menu);
                treeAPI.refreshTree(menu);
            }
        }
        $scope.save = function () {
            return ViewAPIService.UpdateViewsRank($scope.menu.Childs).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("MenuItems", response)) {
                    if ($scope.onPageUpdated != undefined)
                        $scope.onPageUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            })

        };
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };
    
    } 
    function load() {
        if (treeAPI != undefined) {
            var menu = [];
            menu.push($scope.menu);
            treeAPI.refreshTree(menu);
        }
           
    }
    function loadTree() {
        return MenuAPIService.GetAllMenuItems()
           .then(function (response) {
               $scope.menuList = response;

           });
    }
}
appControllers.controller('Security_RankPageEditorController', RankPageEditorController);
