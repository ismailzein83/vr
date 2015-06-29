DynamicPagesEditorController.$inject = ['$scope', 'MenuAPIService', 'WidgetAPIService', 'RoleAPIService', 'UsersAPIService', 'BIVisualElementService1', 'BIConfigurationAPIService', 'ChartSeriesTypeEnum', 'DynamicPagesAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function DynamicPagesEditorController($scope, MenuAPIService, WidgetAPIService, RoleAPIService, UsersAPIService, BIVisualElementService1, BIConfigurationAPIService, ChartSeriesTypeEnum, DynamicPagesAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {
    //var mainGridAPI;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != null) {
            $scope.filter = {
                Name: parameters.Name,
                ModuleId: parameters.ModuleId,
                Audience: parameters.Audience,
                ViewId: parameters.ViewId,
                Content: parameters.Content
            }
            $scope.isEditMode = true;
        }
        else
            $scope.isEditMode = false;
    }
    function defineScope() {
        $scope.widgets = [];
        $scope.selectedWidget;
        $scope.users = [];
        $scope.menuList = [];
        $scope.selectedUsers = [];
        $scope.roles = [];
        $scope.pageName;
        $scope.selectedRoles = [];
        $scope.subViewValue = {};
        $scope.moduleId;
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.Contents=[];
        $scope.selectedNumberOfColumns=3;
        $scope.save = function () {
            var selectedUsersIDs = [];
            for (var i = 0; i < $scope.selectedUsers.length; i++)
                selectedUsersIDs.push($scope.selectedUsers[i].UserId);
            var selectedRolesIDs = [];
            for (var i = 0; i < $scope.selectedRoles.length; i++)
                selectedRolesIDs.push($scope.selectedRoles[i].RoleId);
            var Audiences = {
                Users: selectedUsersIDs,
                Groups: selectedRolesIDs
            };
            $scope.View = {
                Name: $scope.pageName,
                ModuleId: $scope.moduleId,
                Audience: Audiences,
                Content: $scope.Contents
            };

            return DynamicPagesAPIService.SaveView($scope.View).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Page", response)) {
                    if ($scope.onPageAdded != undefined)
                        $scope.onPageAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        };
        $scope.addContent = function () {
            var Content = {
                WidgetId: $scope.selectedWidget.Id,
                NumberOfColumns: $scope.selectedNumberOfColumns.value
            }
            $scope.Contents.push(Content);
            $scope.selectedWidget = null;
        };
        $scope.removeContent = function (Content) {
            $scope.Contents.splice($scope.Contents.indexOf(Content), 1);
        };
        $scope.$watch('beTree.currentNode', function (newObj, oldObj) {
            if ($scope.beTree && angular.isObject($scope.beTree.currentNode)) {
                $scope.moduleId = $scope.beTree.currentNode.Id;
            }
        }, false);
        }
    function load() {
        defineNumberOfColumns();
        defineChartSeriesTypes();
        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadWidgets, loadUsers, loadRoles, loadTree]).finally(function () {
            if ($scope.isEditMode) {
                $scope.pageName= $scope.filter.Name;
                for (var i = 0; i < $scope.users.length; i++)
                    for (var j = 0; j < $scope.filter.Audience.Users.length; j++)
                        if ($scope.filter.Audience.Users[j] == $scope.users[i].UserId)
                            $scope.selectedUsers.push($scope.users[i]);
                for (var i = 0; i < $scope.roles.length; i++)
                    for (var j = 0; j < $scope.filter.Audience.Groups.length; j++)
                        if ($scope.filter.Audience.Groups[j] == $scope.roles[i].RoleId)
                            $scope.selectedRoles.push($scope.roles[i]);
                

            }
            $scope.isInitializing = false;
            $scope.isGettingData = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
        
    }
    function loadTree() {
        return MenuAPIService.GetAllMenuItems()
           .then(function (response) {
               $scope.menuList = response;
              
           });
    }
    function loadWidgets() {
        return WidgetAPIService.GetAllWidgets().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.widgets.push(itm);
            });
        });

    } 
    function defineNumberOfColumns() {
        $scope.numberOfColumns = [
            {
                value: "6",
                description: "Half Row"
            },
            {
                value: "12",
                description: "Full Row"
            }
        ];

        $scope.selectedNumberOfColumns = $scope.numberOfColumns[0];
    }
    function defineChartSeriesTypes() {
        $scope.chartSeriesTypes = [];
        for (var m in ChartSeriesTypeEnum) {
            $scope.chartSeriesTypes.push(ChartSeriesTypeEnum[m]);
        }
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
)});
    }
}
appControllers.controller('Security_DynamicPagesEditorController', DynamicPagesEditorController);
