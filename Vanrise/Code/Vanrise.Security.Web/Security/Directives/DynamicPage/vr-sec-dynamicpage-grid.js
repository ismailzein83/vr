"use strict";

app.directive("vrSecDynamicpageGrid", ["VRNotificationService", "VR_Sec_ViewAPIService", "VR_Sec_ViewService", "VR_Sec_UserAPIService", "VR_Sec_GroupAPIService", "TimeDimensionTypeEnum", "PeriodEnum", "UtilsService", "VRModalService",
function (VRNotificationService, VR_Sec_ViewAPIService, VR_Sec_ViewService, UsersAPIService, VR_Sec_GroupAPIService, TimeDimensionTypeEnum, PeriodEnum, UtilsService, VRModalService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var ctor = new ctorGrid($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Security/Directives/DynamicPage/Templates/DynamicPagesGrid.html"

    };

    function ctorGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            ctrl.dynamicViews = [];
            ctrl.defaultPeriod;
            ctrl.defaultGrouping;
            ctrl.groups = [];
            ctrl.users = [];
            defineMenuActions();
            definePeriods();
            defineTimeDimensionTypes();
            loadUsers();
            loadGroups();

            ctrl.widgets = [];

            ctrl.onMainGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onViewAdded = function (dynamicPageObject) {
                        fillNeededData(dynamicPageObject);
                        gridAPI.itemAdded(dynamicPageObject);
                    };

                    return directiveAPI;
                }
            };

            ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Sec_ViewAPIService.GetFilteredDynamicPages(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            angular.forEach(response.Data, function (itm) {
                                fillNeededData(itm);
                            });
                        }
                        onResponseReady(response);
                    });
            };

        }

        function defineMenuActions() {
            ctrl.menuActions = [
                {
                    name: "Edit",
                    clicked: editDynamicPage,
                    haspermission: hasUpdateViewPermission
                },
                {
                    name: "Delete",
                    clicked: deleteDynamicPage,
                    haspermission: hasDeleteViewPermission
                },
                {
                    name: "Validate",
                    clicked: validate,

                }];

        }
        function hasUpdateViewPermission() {
            return VR_Sec_ViewAPIService.HasUpdateViewPermission();
        }
        function hasDeleteViewPermission() {
            return VR_Sec_ViewAPIService.HasDeleteViewPermission();
        }
        function validate(dataItem) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Validate Dynamic Page: " + dataItem.Entity.Name;
            };
            VRModalService.showModal('/Client/Modules/BI/Views/DynamicPageValidator.html', dataItem.Entity, settings);

        }

        function fillNeededData(itm) {
            if (itm.Entity.ViewContent.DefaultPeriod != null)
                itm.Entity.ViewContent.DefaultPeriodDescription = UtilsService.getItemByVal(ctrl.periods, itm.Entity.ViewContent.DefaultPeriod, 'value')
                .description;
            if (itm.Entity.ViewContent.DefaultGrouping != null)
                itm.Entity.ViewContent.DefaultGroupingDescription = UtilsService.getItemByVal(ctrl.timeDimensionTypes, itm.Entity.ViewContent.DefaultGrouping, 'value')
                .description;
            if (itm.Entity.Audience != null && itm.Entity.Audience.Users != undefined) {
                itm.Entity.Audience.UsersName = [];
                var usersArray = [];
                var value;
                for (var i = 0; i < itm.Entity.Audience.Users.length; i++) {

                    value = UtilsService.getItemByVal(ctrl.users, itm.Entity.Audience.Users[i], 'UserId');
                    if (value != null)
                        usersArray.push(value.Name);

                }
                itm.Entity.Audience.UsersName = usersArray.toString();
            }
            if (itm.Entity.Audience != null && itm.Entity.Audience.Groups != undefined) {

                itm.Entity.Audience.GroupsName = "";
                var groupsArray = [];
                for (var j = 0; j < itm.Entity.Audience.Groups.length; j++) {
                    if (itm.Entity.Audience.GroupsName != "")
                        itm.Entity.Audience.GroupsName += ",";
                    value = UtilsService.getItemByVal(ctrl.groups, itm.Entity.Audience.Groups[j], 'GroupId');
                    if (value != null)
                        groupsArray.push(value.Name);

                }
                itm.Entity.Audience.GroupsName = groupsArray.toString();

            } else {
                itm.Entity.Audience = {
                    UsersName: '',
                    GroupsName: '',
                }
            }
        }

        function definePeriods() {
            ctrl.periods = [];
            for (var p in PeriodEnum)
                ctrl.periods.push(PeriodEnum[p]);
        }

        function defineTimeDimensionTypes() {
            ctrl.timeDimensionTypes = [];
            for (var td in TimeDimensionTypeEnum)
                ctrl.timeDimensionTypes.push(TimeDimensionTypeEnum[td]);
        }

        function loadUsers() {
            UsersAPIService.GetUsers()
                .then(function (response) {
                    angular.forEach(response, function (users) {

                        ctrl.users.push(users);
                    })
                });

        }

        function loadGroups() {
            VR_Sec_GroupAPIService.GetGroupInfo()
                .then(function (response) {
                    angular.forEach(response, function (role) {
                        ctrl.groups.push(role);
                    })
                });
        }

        function editDynamicPage(dataItem) {
            var onDynamicPageUpdated = function (updatedItem) {
                fillNeededData(updatedItem);
                gridAPI.itemUpdated(updatedItem);
            };
            VR_Sec_ViewService.updateDynamicPage(dataItem.Entity.ViewId, onDynamicPageUpdated);
        }

        function deleteDynamicPage(dataItem) {
            var onDynamicPageDeleted = function () {
                gridAPI.itemDeleted(deletedItem);
            };

            VR_Sec_ViewService.deleteDynamicPage($scope, dataItem, onDynamicPageDeleted);
        }

    }

    return directiveDefinitionObject;

}]);