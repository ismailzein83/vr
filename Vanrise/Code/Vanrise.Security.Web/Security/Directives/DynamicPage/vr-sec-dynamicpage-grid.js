"use strict";

app.directive("vrSecDynamicpageGrid", ["VRNotificationService", "ViewAPIService", "VR_ViewService","UsersAPIService","GroupAPIService","TimeDimensionTypeEnum","PeriodEnum","UtilsService",
function (VRNotificationService, ViewAPIService, VR_ViewService, UsersAPIService, GroupAPIService, TimeDimensionTypeEnum, PeriodEnum, UtilsService) {

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
                    }
                    directiveAPI.onDaynamicPageAdded = function (dynamicPageObject) {
                        fillNeededData(dynamicPageObject);
                        gridAPI.itemAdded(dynamicPageObject);
                    }

                    return directiveAPI;
                }
            };

            ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return ViewAPIService.GetFilteredDynamicPages(dataRetrievalInput)
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
                    permissions: "Root/Administration Module/Dynamic Pages:Edit",
                    clicked: function (dataItem) {
                        editDynamicPage(dataItem);
                    }
                },
                {
                    name: "Delete",
                    permissions: "Root/Administration Module/Dynamic Pages:Delete",
                    clicked: function (dataItem) {
                        deleteDynamicPage(dataItem);
                    }
                },
                {
                    name: "Validate",
                    permissions: "Root/Administration Module/Dynamic Pages:Validate",
                    clicked: function (dataItem) {
                        validate(dataItem);
                    }
                }];

        }

        function validate(dataItem) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Validate Dynamic Page: " + dataItem.Name;
            };
            VRModalService.showModal('/Client/Modules/BI/Views/DynamicPageValidator.html', dataItem, settings);

        }

        function fillNeededData(itm) {
            if (itm.ViewContent.DefaultPeriod != null)
                itm.ViewContent.DefaultPeriodDescription = UtilsService.getItemByVal(ctrl.periods, itm.ViewContent.DefaultPeriod, 'value')
                .description;
            if (itm.ViewContent.DefaultGrouping != null)
                itm.ViewContent.DefaultGroupingDescription = UtilsService.getItemByVal(ctrl.timeDimensionTypes, itm.ViewContent.DefaultGrouping, 'value')
                .description;
            if (itm.Audience != null && itm.Audience.Users != undefined) {
                itm.Audience.UsersName = [];
                var usersArray = [];
                var value;
                for (var i = 0; i < itm.Audience.Users.length; i++) {

                    value = UtilsService.getItemByVal(ctrl.users, itm.Audience.Users[i], 'UserId');
                    if (value != null)
                        usersArray.push(value.Name);

                }
                itm.Audience.UsersName = usersArray.toString();
            }
            if (itm.Audience != null && itm.Audience.Groups != undefined) {

                itm.Audience.GroupsName = "";
                var groupsArray = [];
                for (var j = 0; j < itm.Audience.Groups.length; j++) {
                    if (itm.Audience.GroupsName != "")
                        itm.Audience.GroupsName += ",";
                    value = UtilsService.getItemByVal(ctrl.groups, itm.Audience.Groups[j], 'GroupId');
                    if (value != null)
                        groupsArray.push(value.Name);

                }
                itm.Audience.GroupsName = groupsArray.toString();

            } else {
                itm.Audience = {
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
            GroupAPIService.GetGroups()
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
            VR_ViewService.updateWidget(dataItem.Entity.Id, onDynamicPageUpdated);
        }

        function deleteDynamicPage(dataItem) {
            var onDynamicPageDeleted = function (deletedItem) {
                fillNeededData(deletedItem);
                gridAPI.itemDeleted(deletedItem);
            }

            VR_ViewService.deleteWidget($scope, dataItem, onDynamicPageDeleted);
        }

    }

    return directiveDefinitionObject;

}]);