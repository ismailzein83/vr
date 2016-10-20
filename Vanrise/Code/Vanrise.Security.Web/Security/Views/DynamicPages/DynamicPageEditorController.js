DynamicPageEditorController.$inject = ['$scope', 'VR_Sec_MenuAPIService', 'VR_Sec_WidgetAPIService', 'VR_Sec_ViewAPIService',
    'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VR_Sec_WidgetSectionEnum', 'PeriodEnum', 'TimeDimensionTypeEnum',
    'ColumnWidthEnum', 'VRModalService', 'VRUIUtilsService', 'VR_Sec_ViewTypeEnum'];

function DynamicPageEditorController($scope, VR_Sec_MenuAPIService, VR_Sec_WidgetAPIService, VR_Sec_ViewAPIService,
    UtilsService, VRNotificationService, VRNavigationService, VR_Sec_WidgetSectionEnum, PeriodEnum, TimeDimensionTypeEnum,
    ColumnWidthEnum, VRModalService, VRUIUtilsService, VR_Sec_ViewTypeEnum) {
    $scope.scopeModal = {};
    var viewId;
    var viewTypeName = "VR_Sec_BI";
    var viewEntity;
    var isEditMode;
    var previewDirectiveAPI;
    var periodDirectiveAPI;
    var periodReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var userDirectiveAPI;
    var userReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var groupDirectiveAPI;
    var groupReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var timeDimentionDirectiveAPI;
    var timeDimentionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    loadParameters();
    defineScope();
    load();
    var treeAPI;

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != undefined && parameters != null)
            viewId = parameters.viewId;
        isEditMode = (viewId != undefined);

    }

    function defineScope() {

        $scope.scopeModal.onPreviewAPIReady = function (api) {
            previewDirectiveAPI = api;
            buildContentsFromScope();
            var payload = buildPreviewObjFromScope();
            var setLoader = function (value) {
                $scope.scopeModal.isLoadingPreviewDirective = value;
            };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, previewDirectiveAPI, payload, setLoader);
        }

        $scope.scopeModal.onPeriodDirectiveReady = function (api) {
            periodDirectiveAPI = api;
            var payload = {
                selectedIds: viewEntity != undefined ? viewEntity.ViewContent.DefaultPeriod : PeriodEnum.CurrentMonth.value
            };
            var setLoader = function (value) {
                $scope.isLoadingPeriodSection = value
            };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, periodDirectiveAPI, payload, setLoader, periodReadyPromiseDeferred);
        }

        $scope.scopeModal.onTimeDimentionDirectiveReady = function (api) {
            timeDimentionDirectiveAPI = api;
            var payload = {
                selectedIds: viewEntity != undefined ? viewEntity.ViewContent.DefaultGrouping : TimeDimensionTypeEnum.Daily.value
            };
            var setLoader = function (value) {
                $scope.isLoadingTimeDimentionSection = value
            };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, timeDimentionDirectiveAPI, payload, setLoader, timeDimentionReadyPromiseDeferred);
        }

        $scope.scopeModal.onUserDirectiveReady = function (api) {
            userDirectiveAPI = api;
            userReadyPromiseDeferred.resolve();
        }

        $scope.scopeModal.onGroupDirectiveReady = function (api) {
            groupDirectiveAPI = api;
            groupReadyPromiseDeferred.resolve();
        }

        $scope.scopeModal.tabObject;

        $scope.scopeModal.widgets = [];

        $scope.scopeModal.selectedWidget;

        $scope.scopeModal.menuList = [];

        $scope.scopeModal.pageName;

        $scope.scopeModal.sectionTitle;

        $scope.scopeModal.selectedMenuNode;

        $scope.scopeModal.moduleId;

        $scope.scopeModal.selectedPeriod;

        $scope.scopeModal.selectedTimeDimensionType;

        $scope.scopeModal.summaryContents = [];

        $scope.scopeModal.bodyContents = [];

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

        $scope.scopeModal.addedBodyWidgets = [];

        $scope.scopeModal.onSelectionChanged = function () {
            buildContentsFromScope();
        }

        $scope.scopeModal.periodSelectionChanged = function () {
            if ($scope.scopeModal.selectedWidget != undefined && $scope.scopeModal.selectedPeriod != undefined) {
                {
                    var defaultPeriod = $scope.scopeModal.selectedPeriod.description;
                    var title = defaultPeriod + "-" + $scope.scopeModal.selectedWidget.Name;
                    $scope.scopeModal.sectionTitle = title;
                }

            }
        }

        $scope.scopeModal.onWidgetSelectionChanged = function () {
            if ($scope.scopeModal.selectedWidget != undefined && $scope.scopeModal.selectedPeriod != undefined) {
                {
                    var defaultPeriod = $scope.scopeModal.selectedPeriod.description;
                    var title = defaultPeriod + "-" + $scope.scopeModal.selectedWidget.Name;
                    $scope.scopeModal.sectionTitle = title;
                }

            }
        }

        $scope.scopeModal.validateWidgetError = function (addedWidget) {
            return validateWidgetError(addedWidget);
        }

        $scope.scopeModal.save = function () {
            $scope.scopeModal.isLoading = true;
            buildContentsFromScope();
            buildViewObjFromScope();

            if (isEditMode)
                return updateView();
            else
                return addView();

        };

        $scope.scopeModal.validateMenu = function () {
            if ($scope.scopeModal.selectedMenuNode == undefined)
                return "You Should Select Menu Location.";
            return null;
        }

        $scope.scopeModal.validateWidget = function () {
            if ($scope.scopeModal.addedSummaryWidgets.length == 0 && $scope.scopeModal.addedBodyWidgets.length == 0)
                return "You should add at least one Widget.";
            return null;
        }

        $scope.scopeModal.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.scopeModal.onSectionChanged = function () {
            if ($scope.scopeModal.selectedSection != undefined) {
                switch ($scope.scopeModal.selectedSection.value) {
                    case VR_Sec_WidgetSectionEnum.Summary.value:
                        $scope.scopeModal.widgets = $scope.scopeModal.summaryWidgets;
                        $scope.scopeModal.columnWidth = $scope.scopeModal.summaryColumnWidth;
                        $scope.scopeModal.selectedColumnWidth = $scope.scopeModal.columnWidth[0];
                        break;
                    case VR_Sec_WidgetSectionEnum.Body.value:
                        $scope.scopeModal.widgets = $scope.scopeModal.bodyWidgets;
                        $scope.scopeModal.columnWidth = $scope.scopeModal.bodyColumnWidth;
                        $scope.scopeModal.selectedColumnWidth = $scope.scopeModal.columnWidth[0];
                        break;
                }
                $scope.scopeModal.selectedWidget = null;
            }

        }

        $scope.scopeModal.validateObjectBeforePreview = function ()
        {
            return validateObjectBeforePreview();
        }
        $scope.scopeModal.addViewContent = function () {

            var viewWidget = {
                Widget: $scope.scopeModal.selectedWidget,
                NumberOfColumns: $scope.scopeModal.selectedColumnWidth,
                SectionTitle: $scope.scopeModal.sectionTitle
            }
            viewWidget.Widget.onElementReady = function (api) {
                viewWidget.Widget.API = api;
                var payload = {
                    previewMode: true,
                    title: viewWidget.SectionTitle,
                    settings: viewWidget.Widget.Setting.Settings
                }
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingSummaryDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, payload, setLoader);
            }
            if ($scope.scopeModal.nonSearchable) {
                viewWidget.DefaultPeriod = $scope.scopeModal.selectedPeriod.value
                viewWidget.DefaultGrouping = $scope.scopeModal.selectedTimeDimensionType.value
            }
            switch ($scope.scopeModal.selectedSection.value) {
                case VR_Sec_WidgetSectionEnum.Summary.value:
                    $scope.scopeModal.addedSummaryWidgets.push(viewWidget);
                    $scope.scopeModal.widgets.splice($scope.scopeModal.widgets.indexOf($scope.scopeModal.selectedWidget), 1);
                    break;
                case VR_Sec_WidgetSectionEnum.Body.value:
                    $scope.scopeModal.addedBodyWidgets.push(viewWidget);
                    $scope.scopeModal.widgets.splice($scope.scopeModal.widgets.indexOf($scope.scopeModal.selectedWidget), 1);
                    break;
            }
            $scope.scopeModal.selectedWidget = null;

        };

        $scope.scopeModal.itemsSortable = {
            handle: '.handeldrag',
            animation: 150
        };

        $scope.scopeModal.removeViewContent = function (viewContent) {
            var sections = viewContent.Widget.WidgetDefinitionSetting.Sections;
            for (var i = 0; i < sections.length; i++) {
                switch (sections[i]) {
                    case VR_Sec_WidgetSectionEnum.Summary.value:
                        $scope.scopeModal.addedSummaryWidgets.splice($scope.scopeModal.addedSummaryWidgets.indexOf(viewContent), 1);
                        $scope.scopeModal.summaryWidgets.push(viewContent.Widget);
                        break;
                    case VR_Sec_WidgetSectionEnum.Body.value:
                        $scope.scopeModal.addedBodyWidgets.splice($scope.scopeModal.addedBodyWidgets.indexOf(viewContent), 1);
                        $scope.scopeModal.bodyWidgets.push(viewContent.Widget);
                        break;
                }
            }

        };

        $scope.scopeModal.hasSaveViewPermission = function()
        {
            if (isEditMode) {
                return VR_Sec_ViewAPIService.HasUpdateViewPermission();
            }
            else {
                return VR_Sec_ViewAPIService.HasAddViewPermission();
            }

        }


    }
    
    function validateObjectBeforePreview()
    {
        if ($scope.scopeModal.addedSummaryWidgets.length == 0 && $scope.scopeModal.addedBodyWidgets.length == 0)
            return false;

        buildContentsFromScope();
        if ($scope.scopeModal.nonSearchable)
        {
            for (var i = 0; i < $scope.scopeModal.bodyContents.length; i++)
                if ($scope.scopeModal.bodyContents[i].DefaultGrouping == undefined || $scope.scopeModal.bodyContents[i].DefaultPeriod == undefined) {
                    return false;
                }
            for (var j = 0; j < $scope.scopeModal.summaryContents.length; j++)
                if ($scope.scopeModal.summaryContents[j].DefaultGrouping == undefined || $scope.scopeModal.summaryContents[j].DefaultPeriod == undefined) {
                    return false;
                }
        }
        else
        {
            if ($scope.scopeModal.selectedPeriod == undefined || $scope.scopeModal.selectedTimeDimensionType == undefined)
                return false;
        }
        return true;
    }

    function buildPreviewObjFromScope() {
        var payload = {
            selectedPeriod: !$scope.scopeModal.nonSearchable ? $scope.scopeModal.selectedPeriod.value : undefined,
            selectedTimeDimensionType: !$scope.scopeModal.nonSearchable ? $scope.scopeModal.selectedTimeDimensionType : undefined,
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
        var Audiences = {
            Users: userDirectiveAPI.getSelectedIds(),
            Groups: groupDirectiveAPI.getSelectedIds(),
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

                return "The Widget should have default period and default grouping.";
            }

        }
        return null;
    }

    function defineTimeDimensionTypes() {
        $scope.scopeModal.timeDimensionTypes = [];
        for (var td in TimeDimensionTypeEnum)
            $scope.scopeModal.timeDimensionTypes.push(TimeDimensionTypeEnum[td]);
        $scope.scopeModal.selectedTimeDimensionType = TimeDimensionTypeEnum.Daily;
    }

    function addView() {
        $scope.scopeModal.View.ViewTypeName = viewTypeName;
        return VR_Sec_ViewAPIService.AddView($scope.scopeModal.View)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("View", response, "Name")) {
                    if ($scope.onViewAdded != undefined)
                        $scope.onViewAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });
    }

    function updateView() {

        return VR_Sec_ViewAPIService.UpdateView($scope.scopeModal.View)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("View", response, "Name")) {

                    if ($scope.onPageUpdated != undefined)
                        $scope.onPageUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }

            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function(){
                $scope.scopeModal.isLoading = false;
            });
    }

    function buildContentsFromScope() {
        $scope.scopeModal.summaryContents.length = 0;
        $scope.scopeModal.bodyContents.length = 0;
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

        var Audiences = {
            Users: userDirectiveAPI.getSelectedIds(),
            Groups: groupDirectiveAPI.getSelectedIds(),
        };

        var ViewContent = {
            SummaryContents: $scope.scopeModal.summaryContents,
            BodyContents: $scope.scopeModal.bodyContents,
        }
        if (!$scope.scopeModal.nonSearchable) {
            ViewContent.DefaultPeriod = $scope.scopeModal.selectedPeriod.value;
            ViewContent.DefaultGrouping = $scope.scopeModal.selectedTimeDimensionType.value;
        }
        $scope.scopeModal.View = {
            Name: $scope.scopeModal.pageName,
            Title: $scope.scopeModal.pageName,
            Type: viewEntity !=undefined?viewEntity.Type:undefined,
            Url: "#/viewwithparams/Security/Views/DynamicPages/DynamicPagePreview",
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
            getView()
                .then(function () {
                    loadAllControls()
                        .finally(function () {
                            viewEntity = undefined;
                        });
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
        } else {
            $scope.title = UtilsService.buildTitleForAddEditor("View");
            loadAllControls();
        }
    }

    function loadAllControls() {

        return UtilsService.waitMultipleAsyncOperations([loadTree, loadWidgets, loadUsers, loadGroups, defineWidgetSections, defineColumnWidth, loadPeriodSelector, loadTimeDimentionSelector])
            .then(function () {
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

    function getView() {
        return VR_Sec_ViewAPIService.GetView(viewId)
            .then(function (view) {
                viewEntity = view;
                $scope.title = UtilsService.buildTitleForUpdateEditor(viewEntity.Name, "View");
            });
    }

    function loadPeriodSelector() {
        var periodLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        periodReadyPromiseDeferred.promise
            .then(function () {

                var directivePayload = {
                    selectedIds: viewEntity != undefined ? viewEntity.ViewContent.DefaultPeriod : PeriodEnum.CurrentMonth.value
                };
                periodReadyPromiseDeferred = undefined;
                VRUIUtilsService.callDirectiveLoad(periodDirectiveAPI, directivePayload, periodLoadPromiseDeferred);
            });
        return periodLoadPromiseDeferred.promise;
    }

    function loadTimeDimentionSelector() {
        var timeDimentionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        timeDimentionReadyPromiseDeferred.promise
            .then(function () {
                var directivePayload = {
                    selectedIds: viewEntity != undefined ? viewEntity.ViewContent.DefaultGrouping : TimeDimensionTypeEnum.Daily.value
                };
                timeDimentionReadyPromiseDeferred = undefined;
                VRUIUtilsService.callDirectiveLoad(timeDimentionDirectiveAPI, directivePayload, timeDimentionLoadPromiseDeferred);
            });
        return timeDimentionLoadPromiseDeferred.promise;
    }

    function fillEditModeData() {
        if (viewEntity != undefined) {
            $scope.scopeModal.pageName = viewEntity.Name;

            for (var i = 0; i < viewEntity.ViewContent.BodyContents.length; i++) {
                var bodyContent = viewEntity.ViewContent.BodyContents[i];
                var bodyValue = UtilsService.getItemByVal($scope.scopeModal.bodyWidgets, bodyContent.WidgetId, 'Id');
                var numberOfColumns;
                for (var j = 0; j < $scope.scopeModal.columnWidth.length; j++)
                    if (bodyContent.NumberOfColumns == $scope.scopeModal.columnWidth[j].value)
                        numberOfColumns = $scope.scopeModal.columnWidth[j];
                if (bodyValue != null) {
                    addBodyContentAPI(bodyValue, numberOfColumns, bodyContent);
                   
                }

            }

            for (var i = 0; i < viewEntity.ViewContent.SummaryContents.length; i++) {
                var summaryContent = viewEntity.ViewContent.SummaryContents[i];
                var summaryValue = UtilsService.getItemByVal($scope.scopeModal.summaryWidgets, summaryContent.WidgetId, 'Id');
                var numberOfColumns;
                for (var j = 0; j < $scope.scopeModal.columnWidth.length; j++)
                    if (summaryContent.NumberOfColumns == $scope.scopeModal.columnWidth[j].value)
                        numberOfColumns = $scope.scopeModal.columnWidth[j];
                if (summaryValue != null) {
                    addSummaryContentAPI(summaryValue, numberOfColumns, summaryContent)
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
            } else {
                $scope.scopeModal.nonSearchable = true;
            }
        }

    }

    function addBodyContentAPI(bodyValue, numberOfColumns, bodyContent)
    {
        bodyValue.onElementReady = function (api) {
            bodyValue.API = api;
            var bodyPayload = {
                previewMode: true,
                title: bodyContent.SectionTitle,
                settings: bodyValue.Setting.Settings
            }
            var setLoader = function (value) {
                $scope.scopeModal.isLoadingBodyDirective = value;
            };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, bodyValue.API, bodyPayload, setLoader);
        }
        var viewWidget = {
            Widget: bodyValue,
            NumberOfColumns: numberOfColumns,
            SectionTitle: bodyContent.SectionTitle
        }
        if (bodyContent.DefaultPeriod != undefined && bodyContent.DefaultGrouping != undefined) {
            viewWidget.DefaultPeriod = bodyContent.DefaultPeriod
            viewWidget.DefaultGrouping = bodyContent.DefaultGrouping
        }
        $scope.scopeModal.addedBodyWidgets.push(viewWidget);
        $scope.scopeModal.bodyWidgets.splice($scope.scopeModal.bodyWidgets.indexOf(bodyValue), 1);
    }
    function addSummaryContentAPI(summaryValue, numberOfColumns, summaryContent)
    {
        summaryValue.onElementReady = function (api) {
            summaryValue.API = api;
            var summaryPayload = {
                previewMode: true,
                title: summaryContent.SectionTitle,
                settings: summaryValue.Setting.Settings
            }
            var setLoader = function (value) {
                $scope.scopeModal.isLoadingSummaryDirective = value;
            };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, summaryValue.API, summaryPayload, setLoader);
        }
        var viewWidget = {
            Widget: summaryValue,
            NumberOfColumns: numberOfColumns,
            SectionTitle: summaryContent.SectionTitle
        }
        if (summaryContent.DefaultPeriod != undefined && summaryContent.DefaultGrouping != undefined) {
            viewWidget.DefaultPeriod = summaryContent.DefaultPeriod
            viewWidget.DefaultGrouping = summaryContent.DefaultGrouping
        }
        $scope.scopeModal.addedSummaryWidgets.push(viewWidget);
        $scope.scopeModal.summaryWidgets.splice($scope.scopeModal.summaryWidgets.indexOf(summaryValue), 1);
    }

    function loadTree() {
        return VR_Sec_MenuAPIService.GetAllMenuItems(true, true)
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
                addIsSelected(menuList[i].Childs, Id);
        }

    }

    function checkAllowDynamic(response) {
        for (var i = 0; i < response.length; i++) {
            response[i].isOpened = true;
            if (response[i].Childs != undefined)
                checkAllowDynamic(response[i].Childs);
            if (!response[i].AllowDynamic)
                response[i].isDisabled = true;
        }

    }

    function loadWidgets() {
        return VR_Sec_WidgetAPIService.GetAllWidgets()
            .then(function (response) {
                angular.forEach(response, function (itm) {
                    for (var i = 0; i < itm.WidgetDefinitionSetting.Sections.length; i++) {
                        var value = itm.WidgetDefinitionSetting.Sections[i];
                        var obj = itm.Entity;
                        obj.WidgetDefinitionSetting = itm.WidgetDefinitionSetting;
                        if (value == VR_Sec_WidgetSectionEnum.Summary.value)
                            $scope.scopeModal.summaryWidgets.push(obj);
                        else if (value == VR_Sec_WidgetSectionEnum.Body.value)
                            $scope.scopeModal.bodyWidgets.push(obj);
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
        var userLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        userReadyPromiseDeferred.promise.then(function () {
            var directivePayload = {
                selectedIds: (viewEntity != undefined && viewEntity.Audience != null && viewEntity.Audience.Users != undefined) ? viewEntity.Audience.Users : undefined
            };
            VRUIUtilsService.callDirectiveLoad(userDirectiveAPI, directivePayload, userLoadPromiseDeferred);
        });
        return userLoadPromiseDeferred.promise;

    }

    function loadGroups() {
        var groupLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        groupReadyPromiseDeferred.promise.then(function () {
            var directivePayload = {
                selectedIds: (viewEntity != undefined && viewEntity.Audience != null && viewEntity.Audience.Groups != undefined) ? viewEntity.Audience.Groups : undefined
            };
            VRUIUtilsService.callDirectiveLoad(groupDirectiveAPI, directivePayload, groupLoadPromiseDeferred);
        });
        return groupLoadPromiseDeferred.promise;
    }

    function defineWidgetSections() {
        $scope.scopeModal.sections = [];
        for (var m in VR_Sec_WidgetSectionEnum) {
            $scope.scopeModal.sections.push(VR_Sec_WidgetSectionEnum[m]);
        }

        $scope.scopeModal.selectedSection = $scope.scopeModal.sections[0];
    }

}
appControllers.controller('VR_Sec_DynamicPageEditorController', DynamicPageEditorController);