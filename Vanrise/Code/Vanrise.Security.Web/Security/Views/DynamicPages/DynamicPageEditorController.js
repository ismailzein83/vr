DynamicPageEditorController.$inject = ['$scope', 'MenuAPIService', 'WidgetAPIService', 'GroupAPIService', 'UsersAPIService', 'VR_Sec_ViewAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WidgetSectionEnum', 'PeriodEnum', 'TimeDimensionTypeEnum', 'ColumnWidthEnum', 'VRModalService','VRUIUtilsService'];

function DynamicPageEditorController($scope, MenuAPIService, WidgetAPIService, GroupAPIService, UsersAPIService, VR_Sec_ViewAPIService, UtilsService, VRNotificationService, VRNavigationService, WidgetSectionEnum, PeriodEnum, TimeDimensionTypeEnum, ColumnWidthEnum, VRModalService, VRUIUtilsService) {
    $scope.scopeModal = {};
    var viewId;
    var viewEntity;
    var isEditMode;
    var previewDirectiveAPI;
    loadParameters();
    defineScope();
    load();
    var treeAPI;

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters !=undefined && parameters !=null)
            viewId = parameters.ViewId;
        isEditMode=(viewId != undefined);
      
    }

    function defineScope() {


        $scope.scopeModal.onPreviewAPIReady = function (api) {
                previewDirectiveAPI = api;
                buildContentsFromScope();
                var payload = buildPreviewObjFromScope();
                var setLoader = function (value) { $scope.scopeModal.isLoadingPreviewDirective = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, previewDirectiveAPI, payload, setLoader);
        }

        $scope.scopeModal.tabObject;
        $scope.scopeModal.widgets = [];
        $scope.scopeModal.selectedWidget;
        $scope.scopeModal.users = [];
        $scope.scopeModal.menuList = [];
        $scope.scopeModal.selectedUsers = [];
        $scope.scopeModal.groups = [];
        $scope.scopeModal.pageName;
        $scope.scopeModal.sectionTitle;
        $scope.scopeModal.selectedMenuNode;
        $scope.scopeModal.selectedGroups = [];
        $scope.scopeModal.subViewConnector = {};
        $scope.scopeModal.moduleId;
        $scope.scopeModal.selectedViewTimeDimensionType;
        $scope.scopeModal.selectedViewPeriod;
        $scope.scopeModal.selectedWidgetPeriod;
        $scope.scopeModal.selectedWidgetTimeDimensionType;
        $scope.scopeModal.summaryContents = [];
        $scope.scopeModal.bodyContents = [];
        $scope.scopeModal.addedwidgets = [];
        $scope.scopeModal.summaryWidgets = [];
        $scope.scopeModal.bodyWidgets = [];
        $scope.scopeModal.validate = function () {
            validate();
        }
        $scope.scopeModal.selectedColumnWidth;
        $scope.scopeModal.addedSummaryWidgets = [];
        $scope.scopeModal.menuReady = function (api) {
            treeAPI = api;
            if ($scope.scopeModal.menuList.length > 0) {
                treeAPI.refreshTree($scope.scopeModal.menuList);
            } 
        }
        $scope.scopeModal.nonSearchable = false;
        $scope.scopeModal.nonSearchableSelectionChanged = function () {
            $scope.scopeModal.nonSearchable = true;

        }
        $scope.scopeModal.addedBodyWidgets = [];
        $scope.scopeModal.onSelectionChanged = function () {
            buildContentsFromScope();
        }
        $scope.scopeModal.periodSelectionChanged = function () {
            if ($scope.scopeModal.selectedWidget != undefined) {
                {
                    var defaultPeriod;
                    if (!$scope.scopeModal.nonSearchable)
                        defaultPeriod = $scope.selectedViewPeriod.description;
                    else
                        defaultPeriod = $scope.scopeModal.selectedWidgetPeriod.description;
                    var title = defaultPeriod + "-" + $scope.scopeModal.selectedWidget.Name;
                    $scope.scopeModal.sectionTitle = title;
                }

            }
        }
        $scope.scopeModal.onWidgetSelectionChanged = function () {
            if ($scope.scopeModal.selectedWidget != undefined) {
                {
                    var defaultPeriod;
                    if (!$scope.scopeModal.nonSearchable)
                        defaultPeriod = $scope.scopeModal.selectedViewPeriod.description;
                    else
                        defaultPeriod = $scope.scopeModal.selectedWidgetPeriod.description;
                    var title = defaultPeriod + "-" + $scope.scopeModal.selectedWidget.Name;
                    $scope.scopeModal.sectionTitle = title;
                }
               
            }
        }
        $scope.scopeModal.validateWidgetError = function (addedWidget) {
            return validateWidgetError(addedWidget);
        }
        $scope.scopeModal.save = function () {
            if (!checkWidgetValidator()) {
                return; 
            }
            if ($scope.scopeModal.selectedMenuNode == undefined)
                return VRNotificationService.showWarning("You Should Select Menu Location Before Saving!!");
            buildContentsFromScope();
            buildViewObjFromScope(); 
            if ($scope.scopeModal.summaryContents.length == 0 && $scope.scopeModal.bodyContents.length == 0)
                return VRNotificationService.showWarning("You Should Add Widgets Before Saving!!");
          
            if (isEditMode)
                return updateView(); 
            else  
                return addView();
              
        };
        $scope.scopeModal.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.scopeModal.onSectionChanged = function () {
            if ($scope.scopeModal.selectedSection != undefined)
            {
                switch ($scope.scopeModal.selectedSection.value) {
                    case WidgetSectionEnum.Summary.value: $scope.scopeModal.widgets = $scope.scopeModal.summaryWidgets; $scope.scopeModal.columnWidth = $scope.scopeModal.summaryColumnWidth; $scope.scopeModal.selectedColumnWidth = $scope.scopeModal.columnWidth[0]; break;
                    case WidgetSectionEnum.Body.value: $scope.scopeModal.widgets = $scope.scopeModal.bodyWidgets; $scope.scopeModal.columnWidth = $scope.scopeModal.bodyColumnWidth; $scope.scopeModal.selectedColumnWidth = $scope.scopeModal.columnWidth[0]; break;
                }
                $scope.scopeModal.selectedWidget = null;
            }
            
        }
        $scope.scopeModal.addViewContent = function () {

            var viewWidget = {
                Widget: $scope.scopeModal.selectedWidget,
                NumberOfColumns: $scope.scopeModal.selectedColumnWidth,
                SectionTitle: $scope.scopeModal.sectionTitle
            }
            if ($scope.scopeModal.nonSearchable) {
                viewWidget.DefaultPeriod = $scope.scopeModal.selectedWidgetPeriod.value
                viewWidget.DefaultGrouping = $scope.scopeModal.selectedWidgetTimeDimensionType.value
            }
            switch ($scope.scopeModal.selectedSection.value)
            {
                case WidgetSectionEnum.Summary.value: $scope.scopeModal.addedSummaryWidgets.push(viewWidget); $scope.scopeModal.widgets.splice($scope.scopeModal.widgets.indexOf($scope.scopeModal.selectedWidget), 1); break;
                case WidgetSectionEnum.Body.value: $scope.scopeModal.addedBodyWidgets.push(viewWidget); $scope.scopeModal.widgets.splice($scope.scopeModal.widgets.indexOf($scope.scopeModal.selectedWidget), 1); break;
            }
            $scope.scopeModal.selectedWidget = null;
            
        };
        $scope.scopeModal.itemsSortable = { handle: '.handeldrag', animation: 150 };
        $scope.scopeModal.removeViewContent = function (viewContent) {
            var sections=viewContent.Widget.WidgetDefinitionSetting.Sections;
            for (var i = 0; i < sections.length; i++) {
                switch (sections[i]) {
                    case WidgetSectionEnum.Summary.value: $scope.scopeModal.addedSummaryWidgets.splice($scope.scopeModal.addedSummaryWidgets.indexOf(viewContent), 1);
                        $scope.scopeModal.summaryWidgets.push(viewContent.Widget); break;
                    case WidgetSectionEnum.Body.value: $scope.scopeModal.addedBodyWidgets.splice($scope.scopeModal.addedBodyWidgets.indexOf(viewContent), 1);
                        $scope.scopeModal.bodyWidgets.push(viewContent.Widget); break;
                }
            }

        };
    }

    function buildPreviewObjFromScope()
    {
        var payload=
            {
                selectedViewPeriod:$scope.scopeModal.selectedViewPeriod.value,
                selectedViewTimeDimensionType: $scope.scopeModal.selectedViewTimeDimensionType,
                bodyContents: $scope.scopeModal.bodyContents,
                summaryContents: $scope.scopeModal.summaryContents,
            }
        return payload;
    }

    function validate() {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Validate Dynamic Page: " + $scope.scopeModal.pageName;
        };
            
        var selectedUsersIDs = [];
        for (var i = 0; i < $scope.scopeModal.selectedUsers.length; i++)
            selectedUsersIDs.push($scope.scopeModal.selectedUsers[i].UserId);
        var selectedGroupsIDs = [];
        for (var i = 0; i < $scope.scopeModal.selectedGroups.length; i++)
            selectedGroupsIDs.push($scope.scopeModal.selectedGroups[i].GroupId);
        var Audiences = {
            Users: selectedUsersIDs,
            Groups: selectedGroupsIDs
        };
        buildContentsFromScope();
        var ViewContent = {
            BodyContents: $scope.scopeModal.bodyContents,
            SummaryContents: $scope.scopeModal.summaryContents
        }
        var parameter = {
            Audience: Audiences,
            ViewContent: ViewContent
        }
        VRModalService.showModal('/Client/Modules/BI/Views/DynamicPageValidator.html', parameter, settings);

    }
    function validateWidgetError(addedWidget) {
        if ($scope.scopeModal.nonSearchable) {
            if (addedWidget.DefaultGrouping == undefined || addedWidget.DefaultPeriod == undefined) {
                  
                return "ng-valid ng-valid-requiredvalue required-inpute";
                }

            }
    }

    function checkWidgetValidator() {
        if ($scope.scopeModal.nonSearchable) {
            for (var i = 0; i < $scope.scopeModal.addedBodyWidgets.length; i++) {
                var widget = $scope.scopeModal.addedBodyWidgets[i];
                if (widget.DefaultGrouping == undefined || widget.DefaultPeriod == undefined) {
                    VRNotificationService.showWarning("You should select period and grouping for all body widgets.");
                    $scope.scopeModal.addedBodyWidgets[i].isValid = false;
                    return false;
                }

            }
            for (var i = 0; i < $scope.scopeModal.addedSummaryWidgets.length; i++) {
                var widget = $scope.scopeModal.addedSummaryWidgets[i];
                if (widget.DefaultGrouping == undefined || widget.DefaultPeriod == undefined) {
                    VRNotificationService.showWarning("You should select period and grouping for all summary widgets.");
                    $scope.scopeModal.addedSummaryWidgets[i].isValid = false;
                    return false;
                }

            }   
        }
        return true;
    }

    function definePeriods() {
        $scope.scopeModal.periods = [];
        for (var p in PeriodEnum)
            $scope.scopeModal.periods.push(PeriodEnum[p]);
        $scope.scopeModal.selectedViewPeriod = PeriodEnum.CurrentMonth;
        $scope.scopeModal.selectedWidgetPeriod = PeriodEnum.CurrentMonth;

    }

    function defineTimeDimensionTypes() {
        $scope.scopeModal.timeDimensionTypes = [];
        for (var td in TimeDimensionTypeEnum)
            $scope.scopeModal.timeDimensionTypes.push(TimeDimensionTypeEnum[td]);
        $scope.scopeModal.selectedViewTimeDimensionType = TimeDimensionTypeEnum.Daily;
        $scope.scopeModal.selectedWidgetTimeDimensionType = TimeDimensionTypeEnum.Daily;
    }
    function addView() {
       
        return VR_Sec_ViewAPIService.AddView($scope.scopeModal.View).then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("View", response, "Name")) {
                if ($scope.onPageAdded != undefined)
                    $scope.onPageAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }

    function updateView() {
       
        return VR_Sec_ViewAPIService.UpdateView($scope.scopeModal.View).then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("View", response, "Name")) {

                if ($scope.onPageUpdated != undefined)
                    $scope.onPageUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }
    function buildContentsFromScope() {
        $scope.scopeModal.summaryContents = [];
        $scope.scopeModal.bodyContents = [];
        for (var i = 0; i < $scope.scopeModal.addedSummaryWidgets.length; i++) {
            var Widget = $scope.scopeModal.addedSummaryWidgets[i];
            var viewSummaryContent = {
                WidgetId: Widget.Widget.Id,
                NumberOfColumns: Widget.NumberOfColumns.value,
                SectionTitle: Widget.SectionTitle
            }
            if (Widget.DefaultPeriod != undefined && Widget.DefaultGrouping != undefined) {
                viewSummaryContent.DefaultPeriod = Widget.DefaultPeriod
                viewSummaryContent.DefaultGrouping = Widget.DefaultGrouping
            }
            $scope.scopeModal.summaryContents.push(viewSummaryContent);
        }
        for (var i = 0; i < $scope.scopeModal.addedBodyWidgets.length; i++) {
            var Widget = $scope.scopeModal.addedBodyWidgets[i];
            var viewBodyContent = {
                WidgetId: Widget.Widget.Id,
                NumberOfColumns: Widget.NumberOfColumns.value,
                SectionTitle: Widget.SectionTitle
            }
            if (Widget.DefaultPeriod != undefined && Widget.DefaultGrouping != undefined) {
                viewBodyContent.DefaultPeriod = Widget.DefaultPeriod
                viewBodyContent.DefaultGrouping = Widget.DefaultGrouping
            }
            $scope.scopeModal.bodyContents.push(viewBodyContent);
        }

    }
    function buildViewObjFromScope() {
        
        var selectedUsersIDs = [];
        for (var i = 0; i < $scope.scopeModal.selectedUsers.length; i++)
            selectedUsersIDs.push($scope.scopeModal.selectedUsers[i].UserId);
        var selectedGroupsIDs = [];
        for (var i = 0; i < $scope.scopeModal.selectedGroups.length; i++)
            selectedGroupsIDs.push($scope.scopeModal.selectedGroups[i].GroupId);
        var Audiences = {
            Users: selectedUsersIDs,
            Groups: selectedGroupsIDs
        };
       
        var ViewContent = {
            SummaryContents: $scope.scopeModal.summaryContents,
            BodyContents: $scope.scopeModal.bodyContents,
        }
        if (!$scope.scopeModal.nonSearchable) {
            ViewContent.DefaultPeriod = $scope.scopeModal.selectedViewPeriod.value;
            ViewContent.DefaultGrouping = $scope.scopeModal.selectedViewTimeDimensionType.value;
        }
        $scope.scopeModal.View = {
            Name: $scope.scopeModal.pageName,
            ModuleId: $scope.scopeModal.selectedMenuNode.Id,
            Audience: Audiences,
            ViewContent: ViewContent
        };
        if (isEditMode)
            $scope.scopeModal.View.ViewId = viewId;
    }

    function load() {
        $scope.scopeModal.isLoading = true;
        if (isEditMode) {
            getView().then(function () {
                loadAllControls()
                    .finally(function () {
                    });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModal.isLoading = false;
            });
        }
        else {
            $scope.title = UtilsService.buildTitleForAddEditor("View");
            loadAllControls();
        }  
    }

    function getView() {
        return VR_Sec_ViewAPIService.GetView(viewId).then(function (view) {
            viewEntity = view;
            $scope.title = UtilsService.buildTitleForUpdateEditor(viewEntity.Name, "View");
        });
    }

    function loadAllControls() {

        return UtilsService.waitMultipleAsyncOperations([loadTree, loadWidgets, loadUsers, loadGroups, defineWidgetSections, defineColumnWidth, defineTimeDimensionTypes, definePeriods]).then(function ()
        
        {
            if (treeAPI != undefined && !isEditMode) {
                treeAPI.refreshTree($scope.scopeModal.menuList);
            }
            fillEditModeData();
        })
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
              .finally(function () {
                  $scope.scopeModal.isLoading = false;
              });
    }

    function fillEditModeData() {
        if (viewEntity != undefined)
        {
            $scope.scopeModal.pageName = viewEntity.Name;
            if (viewEntity.Audience !=null && (viewEntity.Audience.Users != undefined || viewEntity.Audience.Groups != undefined)) {
                for (var i = 0; i < viewEntity.Audience.Users.length; i++) {
                    var value = UtilsService.getItemByVal($scope.scopeModal.users, viewEntity.Audience.Users[i], 'UserId');
                    if (value != null)
                        $scope.scopeModal.selectedUsers.push(value);
                }

                for (var i = 0; i < viewEntity.Audience.Groups.length; i++) {
                    var value = UtilsService.getItemByVal($scope.scopeModal.groups, viewEntity.Audience.Groups[i], 'GroupId');
                    if (value != null)
                        $scope.scopeModal.selectedGroups.push(value);
                }
            }
            for (var i = 0; i < viewEntity.ViewContent.BodyContents.length; i++) {
                var bodyContent = viewEntity.ViewContent.BodyContents[i];
                var value = UtilsService.getItemByVal($scope.scopeModal.bodyWidgets, bodyContent.WidgetId, 'Id');
                var numberOfColumns;
                for (var j = 0; j < $scope.scopeModal.columnWidth.length; j++)
                    if (bodyContent.NumberOfColumns == $scope.scopeModal.columnWidth[j].value)
                        numberOfColumns = $scope.scopeModal.columnWidth[j];

                if (value != null) {
                    var viewWidget = {
                        Widget: value,
                        NumberOfColumns: numberOfColumns,
                        SectionTitle: bodyContent.SectionTitle
                    }
                    if (bodyContent.DefaultPeriod != undefined && bodyContent.DefaultGrouping != undefined) {
                        viewWidget.DefaultPeriod = bodyContent.DefaultPeriod
                        viewWidget.DefaultGrouping = bodyContent.DefaultGrouping
                    }
                    $scope.scopeModal.addedBodyWidgets.push(viewWidget);
                    $scope.scopeModal.bodyWidgets.splice($scope.scopeModal.bodyWidgets.indexOf(value), 1);
                }

            }

            for (var i = 0; i < viewEntity.ViewContent.SummaryContents.length; i++) {
                var summaryContent = viewEntity.ViewContent.SummaryContents[i];
                var value = UtilsService.getItemByVal($scope.scopeModal.summaryWidgets, summaryContent.WidgetId, 'Id');
                var numberOfColumns;
                for (var j = 0; j < $scope.scopeModal.columnWidth.length; j++)
                    if (summaryContent.NumberOfColumns == $scope.scopeModal.columnWidth[j].value)
                        numberOfColumns = $scope.scopeModal.columnWidth[j];

                if (value != null) {
                    var viewWidget = {
                        Widget: value,
                        NumberOfColumns: numberOfColumns,
                        SectionTitle: summaryContent.SectionTitle
                    }
                    if (summaryContent.DefaultPeriod != undefined && summaryContent.DefaultGrouping != undefined) {
                        viewWidget.DefaultPeriod = summaryContent.DefaultPeriod
                        viewWidget.DefaultGrouping = summaryContent.DefaultGrouping
                    }
                    $scope.scopeModal.addedSummaryWidgets.push(viewWidget);
                    $scope.scopeModal.summaryWidgets.splice($scope.scopeModal.summaryWidgets.indexOf(value), 1);
                }


            }
            //$scope.selectedMenuNode = UtilsService.getItemByVal($scope.menuList, $scope.filter.ModuleId, 'Id');
            // addIsSelected($scope.menuList, $scope.filter.ModuleId);

            if ($scope.scopeModal.menuList.length > 0) {
                $scope.scopeModal.selectedMenuNode = treeAPI.setSelectedNode($scope.scopeModal.menuList, viewEntity.ModuleId, "Id", "Childs");
                treeAPI.refreshTree($scope.scopeModal.menuList);
            }

            if (viewEntity.ViewContent.DefaultPeriod != undefined && viewEntity.ViewContent.DefaultGrouping != undefined) {
                $scope.scopeModal.nonSearchable = false;
                $scope.scopeModal.selectedViewPeriod = getPeriod(viewEntity.ViewContent.DefaultPeriod);
                $scope.scopeModal.selectedViewTimeDimensionType = getTimeDimentionType(viewEntity.ViewContent.DefaultGrouping);
            }
            else {
                $scope.scopeModal.nonSearchable = true;
            }
        }

     

    }

    function getTimeDimentionType(defaultGrouping) {
        return UtilsService.getItemByVal($scope.scopeModal.timeDimensionTypes, defaultGrouping, 'value');
    }

    function getPeriod(defaultPeriod) {
        return UtilsService.getItemByVal($scope.scopeModal.periods, defaultPeriod, 'value');
    }

    function loadTree() {
        return MenuAPIService.GetAllMenuItems()
           .then(function (response) {
               checkAllowDynamic(response);
               $scope.scopeModal.menuList = response;
              
           });
    }

    function addIsSelected(menuList, Id) {
        for (var i = 0; i < menuList.length; i++) {
            if (menuList[i].Id == Id) {
                $scope.scopeModal.selectedMenuNode = menuList[i];
                menuList[i].isSelected = true;
                return;
            }
            if (menuList[i].Childs != undefined)
                addIsSelected(menuList[i].Childs,Id);
        }

    }

    function checkAllowDynamic(response) {
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
                        $scope.scopeModal.summaryWidgets.push(itm.Entity);
                    else if(value==WidgetSectionEnum.Body.value)
                        $scope.scopeModal.bodyWidgets.push(itm.Entity);
                }
            });
        });

    }

    function defineColumnWidth() {
        $scope.scopeModal.columnWidth = [];
        for (var td in ColumnWidthEnum)
            $scope.scopeModal.columnWidth.push(ColumnWidthEnum[td]);

        $scope.scopeModal.summaryColumnWidth = $scope.scopeModal.columnWidth;
        $scope.scopeModal.bodyColumnWidth = [];
        for (var i = 0; i < $scope.scopeModal.columnWidth.length; i++) {
            if ($scope.scopeModal.columnWidth[i].value != ColumnWidthEnum.QuarterRow.value)
                $scope.scopeModal.bodyColumnWidth.push($scope.scopeModal.columnWidth[i])
        }
        $scope.scopeModal.selectedColumnWidth = $scope.scopeModal.columnWidth[0];
    }

    function loadUsers() {
        UsersAPIService.GetUsers().then(function (response) {
            angular.forEach(response, function (users) {
                $scope.scopeModal.users.push(users);
                }) 
            });
      
    }

    function loadGroups() {
        GroupAPIService.GetGroups().then(function (response) {
            angular.forEach(response, function (group) {
                $scope.scopeModal.groups.push(group);
                }
)});
    }


    function defineWidgetSections() {
        $scope.scopeModal.sections = [];
        for (var m in WidgetSectionEnum) {
            $scope.scopeModal.sections.push(WidgetSectionEnum[m]);
        }

        $scope.scopeModal.selectedSection = $scope.scopeModal.sections[0];
    }

}
appControllers.controller('Security_DynamicPageEditorController', DynamicPageEditorController);
