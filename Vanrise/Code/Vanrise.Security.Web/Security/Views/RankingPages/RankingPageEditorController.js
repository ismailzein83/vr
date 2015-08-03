RankPageEditorController.$inject = ['$scope', 'MenuAPIService', 'WidgetAPIService', 'GroupAPIService', 'UsersAPIService', 'ViewAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WidgetSectionEnum', 'PeriodEnum', 'TimeDimensionTypeEnum', 'ColumnWidthEnum', 'VRModalService'];

function RankPageEditorController($scope, MenuAPIService, WidgetAPIService, GroupAPIService, UsersAPIService, ViewAPIService, UtilsService, VRNotificationService, VRNavigationService, WidgetSectionEnum, PeriodEnum, TimeDimensionTypeEnum, ColumnWidthEnum, VRModalService) {
    loadParameters();
    defineScope();
    load();
    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != null) {
            $scope.menu = {
                Name: parameters.Name,
                Childs: parameters.Childs
            }
        }
    }
    function defineScope() {
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
      
    }
    function loadTree() {
        return MenuAPIService.GetAllMenuItems()
           .then(function (response) {
               $scope.menuList = response;

           });
    }
}
appControllers.controller('Security_RankPageEditorController', RankPageEditorController);
