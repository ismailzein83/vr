﻿DynamicPageEditorController.$inject = ['$scope', 'MenuAPIService', 'WidgetAPIService', 'RoleAPIService', 'UsersAPIService', 'ViewAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService','WidgetSectionEnum'];

function DynamicPageEditorController($scope, MenuAPIService, WidgetAPIService, RoleAPIService, UsersAPIService, ViewAPIService, UtilsService, VRNotificationService, VRNavigationService, WidgetSectionEnum) {
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
                BodyContents: parameters.ViewContent.BodyContents,
                SummaryContents: parameters.ViewContent.SummaryContents
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
        $scope.summaryContents = [];
        $scope.bodyContents = [];
        $scope.addedwidgets = [];
        $scope.summaryWidgets = [];
        $scope.bodyWidgets = [];
        $scope.selectedColumnWidth;
        $scope.addedSummaryWidgets = [];
        $scope.addedBodyWidgets=[];
        $scope.save = function () {
            buildViewObjFromScope();
            if ($scope.isEditMode) 
                updateView();
            else
                saveView();
        };
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.onSectionChanged = function () {
            switch ($scope.selectedSection.value) {
                case WidgetSectionEnum.Summary.value: $scope.widgets = $scope.summaryWidgets; break;
                case WidgetSectionEnum.Body.value: $scope.widgets = $scope.bodyWidgets; break;
            }
            $scope.selectedWidget = null;
        }
        $scope.addViewContent = function () {
            var viewContent = {
                WidgetId: $scope.selectedWidget.Id,
                NumberOfColumns: $scope.selectedColumnWidth.value
            }
            var viewWidget = {
                Widget: $scope.selectedWidget,
                NumberOfColumns: $scope.selectedColumnWidth.value
            }
            switch($scope.selectedSection.value)
            {
                case WidgetSectionEnum.Summary.value: $scope.addedSummaryWidgets.push(viewWidget); $scope.summaryContents.push(viewContent); $scope.widgets.splice($scope.widgets.indexOf($scope.selectedWidget), 1); break;
                case WidgetSectionEnum.Body.value: $scope.addedBodyWidgets.push(viewWidget); $scope.bodyContents.push(viewContent); $scope.widgets.splice($scope.widgets.indexOf($scope.selectedWidget), 1); break;
            }
            $scope.selectedWidget = null;
            
        };
        $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };
        $scope.removeViewContent = function (viewContent) {
            switch ($scope.selectedSection.value) {
                case WidgetSectionEnum.Summary.value: $scope.addedSummaryWidgets.splice($scope.addedSummaryWidgets.indexOf(viewContent), 1);
                    $scope.summaryContents.splice($scope.summaryContents.indexOf(viewContent), 1);
                    $scope.widgets.push(viewContent.Widget); break;
                case WidgetSectionEnum.Body.value: $scope.addedBodyWidgets.splice($scope.addedBodyWidgets.indexOf(viewContent), 1);
                    $scope.bodyContents.splice($scope.bodyContents.indexOf(viewContent), 1);
                    $scope.widgets.push(viewContent.Widget); break;
            }
            

        };
        $scope.$watch('beTree.currentNode', function (newObj, oldObj) {
            if ($scope.beTree && angular.isObject($scope.beTree.currentNode)) {
                $scope.moduleId = $scope.beTree.currentNode.Id;
            }
        }, false);
    }

    function saveView() {
        return ViewAPIService.SaveView($scope.View).then(function (response) {
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
       
        return ViewAPIService.UpdateView($scope.View).then(function (response) {
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
        var ViewContent = {
            SummaryContents: $scope.summaryContents,
            BodyContents: $scope.bodyContents
        }
        $scope.View = {
            Name: $scope.pageName,
            ModuleId: $scope.moduleId,
            Audience: Audiences,
            ViewContent: ViewContent
        };
        if($scope.isEditMode )
            $scope.View.ViewId = $scope.filter.ViewId;
    }

    function load() {
        defineColumnWidth();
        defineWidgetSections();
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
        if ($scope.filter.Audience != null || $scope.filter.Audience != undefined) {
            for (var i = 0; i < $scope.filter.Audience.Users.length; i++) {
                var value = UtilsService.getItemByVal($scope.users, $scope.filter.Audience.Users[i], 'UserId');
                if (value != null)
                    $scope.selectedUsers.push(value);
            }

            for (var i = 0; i < $scope.filter.Audience.Groups.length; i++) {
                var value = UtilsService.getItemByVal($scope.roles, $scope.filter.Audience.Groups[i], 'RoleId');
                if (value != null)
                    $scope.selectedRoles.push(value);
            }
        }
        for (var i = 0; i < $scope.filter.BodyContents.length; i++)
        {
            
            var bodyContent = $scope.filter.BodyContents[i];
            var value = UtilsService.getItemByVal($scope.bodyWidgets, bodyContent.WidgetId, 'Id');
            if (value != null)
            {
                $scope.selectedWidget = value;
                var viewContent = {
                    WidgetId: $scope.selectedWidget.Id,
                    NumberOfColumns: bodyContent.NumberOfColumns
                }
                $scope.bodyContents.push(viewContent);
                $scope.addedBodyWidgets.push(bodyContent);
            }
                
        }
        
        for (var i = 0; i < $scope.filter.SummaryContents.length; i++) {
            var summaryContent = $scope.filter.SummaryContents[i];
            var value = UtilsService.getItemByVal($scope.summaryWidgets, summaryContent.WidgetId, 'Id');
            if (value != null) {
                $scope.selectedWidget = value;
                var viewContent = {
                    WidgetId: $scope.selectedWidget.Id,
                    NumberOfColumns: summaryContent.NumberOfColumns
                }
                $scope.summaryContents.push(viewContent);
                $scope.addedSummaryWidgets.push(summaryContent);
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
                for(var i=0;i<itm.WidgetDefinitionSetting.Sections.length;i++)
                {
                    var value=itm.WidgetDefinitionSetting.Sections[i];
                    if(value==WidgetSectionEnum.Summary.value)
                        $scope.summaryWidgets.push(itm);
                    else if(value==WidgetSectionEnum.Body.value)
                        $scope.bodyWidgets.push(itm);
                }
            });
        });

    }

    function defineColumnWidth() {
        $scope.columnWidth = [
            {
                value: "12",
                description: "Full Row"
            },
            {
                value: "6",
                description: "Half Row"
            },
            {
                value: "4",
                description: "1/3 Row"
            },
            {
                value: "2",
                description: "Quarter Row"
            }
        ];

        $scope.selectedColumnWidth = $scope.columnWidth[0];
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


    function defineWidgetSections() {
        $scope.sections = [];
        for (var m in WidgetSectionEnum) {
            $scope.sections.push(WidgetSectionEnum[m]);
        }

        $scope.selectedSection = $scope.sections[0];
    }

}
appControllers.controller('Security_DynamicPageEditorController', DynamicPageEditorController);
