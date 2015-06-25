DynamicPagesEditorController.$inject = ['$scope', 'MenuAPIService', 'WidgetAPIService', 'RoleAPIService', 'UsersAPIService', 'BIVisualElementService1', 'BIConfigurationAPIService', 'ChartSeriesTypeEnum', 'DynamicPagesAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function DynamicPagesEditorController($scope, MenuAPIService, WidgetAPIService, RoleAPIService, UsersAPIService, BIVisualElementService1, BIConfigurationAPIService, ChartSeriesTypeEnum, DynamicPagesAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {
    //var mainGridAPI;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
    }
    function defineScope() {
        $scope.widgets = [];
        $scope.selectedWidget;
        $scope.Measures = [];
        $scope.Entities = [];
        $scope.Users = [];
        $scope.menuList = [];
        $scope.selectedUsers = [];
        $scope.Roles = [];
        $scope.PageName;
        $scope.selectedRoles = [];
        $scope.selectedEntityType;
        $scope.selectedMeasureTypes=[];
        $scope.visualElements = [];
        $scope.subViewValue = {};
        $scope.ModuleId;
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.selectedNumberOfColumns=3;
        $scope.save = function () {
            console.log($scope.selectedUsers);
            console.log($scope.selectedRoles);
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
                Name: $scope.PageName,
                ModuleId: $scope.ModuleId,
                Audience: Audiences,
                Content: $scope.visualElements
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
        //$scope.chartReady = function (api) {
        //    $scope.chartAPI = api;

        //};

        //$scope.chartTopReady = function (api) {
        //    chartTopAPI = api;
        //    // updateChart();
        //};
        $scope.addVisualElement = function () {
            $scope.subViewValue = $scope.subViewValue.getValue();
            var visualElement = {
                settings: $scope.subViewValue,
                directive: $scope.selectedWidget.directiveName,
                numberOfColumns: $scope.selectedNumberOfColumns.value
            };

            visualElement.onElementReady = function (api) {
                visualElement.API = api;
            };
            $scope.visualElements.push(visualElement);
           // console.log(visualElement.settings.timedimensiontype);
            $scope.selectedWidget = null;
           
        };
        $scope.removeVisualElement = function (visualElement) {
            $scope.visualElements.splice($scope.visualElements.indexOf(visualElement), 1);
        };
        $scope.$watch('beTree.currentNode', function (newObj, oldObj) {
            if ($scope.beTree && angular.isObject($scope.beTree.currentNode)) {
                $scope.ModuleId = $scope.beTree.currentNode.Id;
                //$scope.showBreakInheritance = !$scope.beTree.currentNode.BreakInheritance;
                //refreshGrid();
            }
        }, false);
        }

    function load() {
        defineNumberOfColumns();
     
        defineChartSeriesTypes();
        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadWidgets, loadMeasures, loadEntities, loadUsers, loadRoles, loadTree]).finally(function () {
            $scope.isInitializing = false;
            $scope.isGettingData = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
        
    }

    function loadTree() {
        console.log("test");
        return MenuAPIService.GetAllMenuItems()
           .then(function (response) {
               console.log(response);
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

    function loadMeasures() {
        return BIConfigurationAPIService.GetMeasures().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.Measures.push(itm);
                console.log(itm);
            });
        });
    }
    function loadEntities() {
        return BIConfigurationAPIService.GetEntities().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.Entities.push(itm);
                console.log($scope.Entities[0].Id);
            });
        });
    }
    function loadUsers() {
        UsersAPIService.GetUsers().then(function (response) {
            angular.forEach(response, function (users) {
                $scope.Users.push(users);
                }) 
            });
      
    }
    function loadRoles() {
        RoleAPIService.GetRoles().then(function (response) {
            //Remove existing roles
            angular.forEach(response, function (role) {
                $scope.Roles.push(role);
                }
)});
    }

}
appControllers.controller('Security_DynamicPagesEditorController', DynamicPagesEditorController);
