ValidateEditorController.$inject = ['$scope', 'MenuAPIService', 'WidgetAPIService', 'RoleAPIService', 'UsersAPIService', 'ViewAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WidgetSectionEnum', 'PeriodEnum', 'TimeDimensionTypeEnum', 'ColumnWidthEnum'];

function ValidateEditorController($scope, MenuAPIService, WidgetAPIService, RoleAPIService, UsersAPIService, ViewAPIService, UtilsService, VRNotificationService, VRNavigationService, WidgetSectionEnum, PeriodEnum, TimeDimensionTypeEnum, ColumnWidthEnum) {
    loadParameters();
    defineScope();
    load();
    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        console.log(parameters);
        if (parameters != null) {
            $scope.filter = {
                ModuleId: parameters.ModuleId,
                Audience: parameters.Audience,
                ViewId: parameters.ViewId,
                BodyContents: parameters.ViewContent.BodyContents,
                SummaryContents: parameters.ViewContent.SummaryContents,
            }
            $scope.isEditMode = true;
        }
        else
            $scope.isEditMode = false;
    }

    function defineScope() {

        $scope.menuReady = function (api) {
            treeAPI = api;
            if ($scope.menuList.length > 0) {
                treeAPI.refreshTree($scope.menuList);

            }

        }

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

    }
    function load() {

        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadWidgets, loadUsers, loadRoles]).then(function () {
            if (treeAPI != undefined && !$scope.isEditMode) {
                treeAPI.refreshTree($scope.menuList);
            }
            if ($scope.isEditMode) {

                fillEditModeData();
            }

        }).finally(function () {
            $scope.isGettingData = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });

    }
    function loadWidgets() {
        return WidgetAPIService.GetAllWidgets().then(function (response) {

            angular.forEach(response, function (itm) {
                for (var i = 0; i < itm.WidgetDefinitionSetting.Sections.length; i++) {
                    var value = itm.WidgetDefinitionSetting.Sections[i];
                    if (value == WidgetSectionEnum.Summary.value)
                        $scope.summaryWidgets.push(itm);
                    else if (value == WidgetSectionEnum.Body.value)
                        $scope.bodyWidgets.push(itm);
                }
            });
        });

    }
    function loadUsers() {
        UsersAPIService.GetUsers().then(function (response) {
            angular.forEach(response, function (users) {
                $scope.users.push(users);
            })
        });

    }

    function loadRoles() {
        RoleAPIService.GetRoles().then(function (response) {
            angular.forEach(response, function (role) {
                $scope.roles.push(role);
            }
)
        });
    }



}
appControllers.controller('Security_ValidateEditorController', ValidateEditorController);
