DynamicPageEditorController.$inject = ['$scope', 'MenuAPIService', 'WidgetAPIService', 'RoleAPIService', 'UsersAPIService', 'DynamicPageAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

function DynamicPageEditorController($scope, MenuAPIService, WidgetAPIService, RoleAPIService, UsersAPIService, DynamicPageAPIService, UtilsService, VRNotificationService, VRNavigationService) {
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
        $scope.subViewConnector = {};
        $scope.moduleId;
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.viewContents = [];
        $scope.addedwidgets = [];
        $scope.selectedNumberOfColumns;
        $scope.save = function () {
            buildViewObjFromScope();
            if ($scope.isEditMode) 
                updateView();
            else
                saveView();
        };
        $scope.addViewContent = function () {
            var viewContent = {
                WidgetId: $scope.selectedWidget.Id,
                NumberOfColumns: $scope.selectedNumberOfColumns.value
            }
            $scope.viewContents.push(viewContent);
            $scope.addedwidgets.push($scope.selectedWidget);
            $scope.selectedWidget = null;
        };
        $scope.removeViewContent = function (viewContent) {
            $scope.viewContents.splice($scope.viewContents.indexOf(viewContent), 1);
        };
        $scope.$watch('beTree.currentNode', function (newObj, oldObj) {
            if ($scope.beTree && angular.isObject($scope.beTree.currentNode)) {
                $scope.moduleId = $scope.beTree.currentNode.Id;
            }
        }, false);
    }

    function saveView() {
        return DynamicPageAPIService.SaveView($scope.View).then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Page", response)) {
                if ($scope.onPageAdded != undefined)
                    $scope.onPageAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }

    function updateView() {
       
        return DynamicPageAPIService.UpdateView($scope.View).then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Page", response)) {
                if ($scope.onPageUpdated != undefined)
                    $scope.onPageUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }

    function buildViewObjFromScope() {
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
            Content: $scope.viewContents
        };
        if($scope.isEditMode )
            $scope.View.ViewId = $scope.filter.ViewId;
    }

    function load() {
        defineNumberOfColumns();
        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadWidgets, loadUsers, loadRoles, loadTree]).finally(function () {
            if ($scope.isEditMode) {
                fillEditModeData();
            }
            $scope.isInitializing = false;
            $scope.isGettingData = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
        
    }

    function fillEditModeData() {
        $scope.pageName = $scope.filter.Name;
        
        for (var i = 0; i < $scope.filter.Audience.Users.length; i++)
        {
            var value = UtilsService.getItemByVal($scope.users, $scope.filter.Audience.Users[i], 'UserId');
            if(value!=null)
                $scope.selectedUsers.push(value);
        }
        for (var i = 0; i < $scope.filter.Audience.Groups.length; i++)
        {
            var value = UtilsService.getItemByVal($scope.roles, $scope.filter.Audience.Groups[i], 'RoleId');
            if (value != null)
                $scope.selectedRoles.push(value);
        }
        for (var i = 0; i < $scope.filter.Content.length; i++)
        {
            var content = $scope.filter.Content[i];
            var value = UtilsService.getItemByVal($scope.widgets, content.WidgetId, 'Id');
            if (value != null)
            {
                $scope.selectedWidget = value;
                var viewContent = {
                    WidgetId: $scope.selectedWidget.Id,
                    NumberOfColumns: content.NumberOfColumns
                }
                $scope.viewContents.push(viewContent);
            }
                
        }
        $scope.beTree.currentNode = UtilsService.getItemByVal($scope.menuList, $scope.filter.ModuleId, 'Id');   
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
appControllers.controller('Security_DynamicPageEditorController', DynamicPageEditorController);
