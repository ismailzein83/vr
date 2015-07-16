DynamicPageEditorController.$inject = ['$scope', 'MenuAPIService', 'WidgetAPIService', 'RoleAPIService', 'UsersAPIService', 'ViewAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WidgetSectionEnum', 'PeriodEnum', 'TimeDimensionTypeEnum', 'ColumnWidthEnum'];

function DynamicPageEditorController($scope, MenuAPIService, WidgetAPIService, RoleAPIService, UsersAPIService, ViewAPIService, UtilsService, VRNotificationService, VRNavigationService, WidgetSectionEnum, PeriodEnum, TimeDimensionTypeEnum, ColumnWidthEnum) {
    loadParameters();
    defineScope();
    load();
    var treeAPI;
    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        
        if (parameters != null) {
            $scope.filter = {
                Name: parameters.Name,
                ModuleId: parameters.ModuleId,
                Audience: parameters.Audience,
                ViewId: parameters.ViewId,
                BodyContents: parameters.ViewContent.BodyContents,
                SummaryContents: parameters.ViewContent.SummaryContents,
                DefaultPeriod: parameters.ViewContent.DefaultPeriod,
                DefaultGrouping: parameters.ViewContent.DefaultGrouping
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
        $scope.sectionTitle;
        $scope.selectedMenuNode;
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
        $scope.menuReady = function (api) {
            treeAPI = api;
            if ($scope.menuList.length > 0)
                treeAPI.refreshTree();
        }
        $scope.addedBodyWidgets = [];
        $scope.onSelectionChanged = function () {
            buildContentsFromScope();
        }
        $scope.onWidgetSelectionChanged = function () {
            if ($scope.selectedWidget != undefined) {
                var title = $scope.selectedWidget.Name;
                $scope.sectionTitle = title;
            }
           
            
        }
        $scope.save = function () {
            buildContentsFromScope();
            buildViewObjFromScope(); 
            if ($scope.summaryContents.length == 0 && $scope.bodyContents.length == 0)
                return VRNotificationService.showWarning("You Should Add Widgets Before Saving!!");
            if ($scope.selectedMenuNode==undefined)
                    return VRNotificationService.showWarning("You Should Select Menu Location Before Saving!!");
            if ($scope.isEditMode)  
                return updateView(); 
            else  
                return saveView();
              
        };
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.onSectionChanged = function () {

            switch ($scope.selectedSection.value) {
                case WidgetSectionEnum.Summary.value: $scope.widgets = $scope.summaryWidgets; $scope.columnWidth = $scope.summaryColumnWidth; $scope.selectedColumnWidth = $scope.columnWidth[0]; break;
                case WidgetSectionEnum.Body.value: $scope.widgets = $scope.bodyWidgets; $scope.columnWidth = $scope.bodyColumnWidth; $scope.selectedColumnWidth = $scope.columnWidth[0]; break;
            }
            $scope.selectedWidget = null;
        }
        $scope.addViewContent = function () {

            var viewWidget = {
                Widget: $scope.selectedWidget,
                NumberOfColumns: $scope.selectedColumnWidth.value,
                SectionTitle:$scope.sectionTitle
            }
            switch($scope.selectedSection.value)
            {
                case WidgetSectionEnum.Summary.value: $scope.addedSummaryWidgets.push(viewWidget); $scope.widgets.splice($scope.widgets.indexOf($scope.selectedWidget), 1); break;
                case WidgetSectionEnum.Body.value: $scope.addedBodyWidgets.push(viewWidget);  $scope.widgets.splice($scope.widgets.indexOf($scope.selectedWidget), 1); break;
            }
            $scope.selectedWidget = null;
            
        };
        $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };
        $scope.removeViewContent = function (viewContent) {
            var sections=viewContent.Widget.WidgetDefinitionSetting.Sections;
            for (var i = 0; i < sections.length; i++) {
                switch (sections[i]) {
                    case WidgetSectionEnum.Summary.value: $scope.addedSummaryWidgets.splice($scope.addedSummaryWidgets.indexOf(viewContent), 1); 
                        $scope.summaryWidgets.push(viewContent.Widget); break;
                    case WidgetSectionEnum.Body.value: $scope.addedBodyWidgets.splice($scope.addedBodyWidgets.indexOf(viewContent), 1);
                        $scope.bodyWidgets.push(viewContent.Widget); break;
                }
            }

        };
    }
    function definePeriods() {
        $scope.periods = [];
        for (var p in PeriodEnum)
            $scope.periods.push(PeriodEnum[p]);
        $scope.selectedPeriod = PeriodEnum.CurrentMonth;


    }

    function defineTimeDimensionTypes() {
        $scope.timeDimensionTypes = [];
        for (var td in TimeDimensionTypeEnum)
            $scope.timeDimensionTypes.push(TimeDimensionTypeEnum[td]);

        $scope.selectedTimeDimensionType = TimeDimensionTypeEnum.Daily;
    }
    function saveView() {
       
        return ViewAPIService.SaveView($scope.View).then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("View", response)) {
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
            if (VRNotificationService.notifyOnItemUpdated("View", response)) {
                if ($scope.onPageUpdated != undefined)
                    $scope.onPageUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }
    function buildContentsFromScope() {
        $scope.summaryContents = [];
        $scope.bodyContents = [];
        for (var i = 0; i < $scope.addedSummaryWidgets.length; i++) {
            var Widget = $scope.addedSummaryWidgets[i];
            var viewSummaryContent = {
                WidgetId: Widget.Widget.Id,
                NumberOfColumns: Widget.NumberOfColumns,
                SectionTitle: Widget.SectionTitle
            }
            $scope.summaryContents.push(viewSummaryContent);
        }
        for (var i = 0; i < $scope.addedBodyWidgets.length; i++) {
            var Widget = $scope.addedBodyWidgets[i];
            var viewBodyContent = {
                WidgetId: Widget.Widget.Id,
                NumberOfColumns: Widget.NumberOfColumns,
                SectionTitle: Widget.SectionTitle
            }
            $scope.bodyContents.push(viewBodyContent);
        }

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
            BodyContents: $scope.bodyContents,
            DefaultPeriod: $scope.selectedPeriod.value,
            DefaultGrouping: $scope.selectedTimeDimensionType.value
        }
        $scope.View = {
            Name: $scope.pageName,
            ModuleId: $scope.selectedMenuNode.Id,
            Audience: Audiences,
            ViewContent: ViewContent
        };
        if($scope.isEditMode )
            $scope.View.ViewId = $scope.filter.ViewId;
    }

    function load() {
        definePeriods();
        defineTimeDimensionTypes();
        defineColumnWidth();
        defineWidgetSections();
        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadWidgets, loadUsers, loadRoles, loadTree]).finally(function () {
            if (treeAPI!=undefined && !$scope.isEditMode)
             treeAPI.refreshTree();
            
            if ($scope.isEditMode) {
               
                fillEditModeData();
            }

          
            $scope.isGettingData = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
        
    }

    function fillEditModeData() {
        
        $scope.pageName = $scope.filter.Name;
        if ($scope.filter.Audience.Users != undefined || $scope.filter.Audience.Groups != undefined ) {
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
                var viewWidget = {
                    Widget: value,
                    NumberOfColumns: bodyContent.NumberOfColumns,
                    SectionTitle: bodyContent.SectionTitle
                }

                $scope.addedBodyWidgets.push(viewWidget);
                $scope.bodyWidgets.splice($scope.bodyWidgets.indexOf(value), 1);
            }
                
        }
        
        for (var i = 0; i < $scope.filter.SummaryContents.length; i++) {
            var summaryContent = $scope.filter.SummaryContents[i];
            var value = UtilsService.getItemByVal($scope.summaryWidgets, summaryContent.WidgetId, 'Id');
            if (value != null) {
                var viewWidget = {
                    Widget: value,
                    NumberOfColumns: summaryContent.NumberOfColumns,
                    SectionTitle: summaryContent.SectionTitle
                }
               
                $scope.addedSummaryWidgets.push(viewWidget);
                $scope.summaryWidgets.splice($scope.summaryWidgets.indexOf(value), 1);
            }

        }
        $scope.selectedMenuNode = UtilsService.getItemByVal($scope.menuList, $scope.filter.ModuleId, 'Id');
        addIsSelected($scope.menuList, $scope.filter.ModuleId);
        treeAPI.refreshTree();
        $scope.selectedPeriod = getPeriod($scope.filter.DefaultPeriod);
        $scope.selectedTimeDimensionType = getTimeDimentionType($scope.filter.DefaultGrouping);
    }
    function getTimeDimentionType(defaultGrouping) {
        return UtilsService.getItemByVal($scope.timeDimensionTypes, defaultGrouping, 'value');
    }
    function getPeriod(defaultPeriod) {
       return UtilsService.getItemByVal($scope.periods, defaultPeriod, 'value');
    }

    function loadTree() {
        return MenuAPIService.GetAllMenuItems()
           .then(function (response) {
               checkAllowDynamic(response);
               $scope.menuList = response;
              
           });
    }
    function addIsSelected(menuList, Id) {
        for (var i = 0; i < menuList.length; i++) {
            if (menuList[i].Id == Id) {
                menuList[i].isSelected = true;
               
                return;
            }
            if (menuList[i].Childs != undefined)
                addIsSelected(menuList[i].Childs);
        }

    }
    function checkAllowDynamic(response){
        for (var i = 0; i < response.length; i++) {
            response[i].isOpened = true;
            if(response[i].Childs!=undefined)
                checkAllowDynamic(response[i].Childs);
            if(!response[i].AllowDynamic)
                response[i].isDisabled=true;
        }
           
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
        $scope.columnWidth = [];
        for (var td in ColumnWidthEnum)
            $scope.columnWidth.push(ColumnWidthEnum[td]);

        $scope.summaryColumnWidth = $scope.columnWidth;
        $scope.bodyColumnWidth = [];
        for (var i = 0; i < $scope.columnWidth.length; i++) {
            if ($scope.columnWidth[i].value != ColumnWidthEnum.QuarterRow.value)
             $scope.bodyColumnWidth.push($scope.columnWidth[i])
        }
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
