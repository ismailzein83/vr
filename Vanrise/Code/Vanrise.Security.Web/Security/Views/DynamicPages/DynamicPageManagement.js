﻿'use strict'
DynamicPageManagementController.$inject = ['$scope', 'ViewAPIService', 'VRModalService', 'VRNotificationService','DeleteOperationResultEnum','PeriodEnum','UtilsService','TimeDimensionTypeEnum','UsersAPIService','RoleAPIService'];
function DynamicPageManagementController($scope, ViewAPIService, VRModalService, VRNotificationService, DeleteOperationResultEnum, PeriodEnum, UtilsService, TimeDimensionTypeEnum, UsersAPIService, RoleAPIService) {
    var mainGridAPI;
    defineScope();
    load();
    function defineScope() {
        definePeriods();
        defineTimeDimensionTypes();
        $scope.filterValue;
        $scope.dynamicViews = [];
        $scope.defaultPeriod;
        $scope.defaultGrouping;
        $scope.groups;

        $scope.users=[];
        $scope.roles = [];
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };
        $scope.menuActions = [
           {
               name: "Edit",
               clicked: function (dataItem) {
                   updatePage(dataItem);
               }
           },
           {
               name: "Delete",
               clicked: function (dataItem) {
                   deletePage(dataItem);
               }
           }];
        $scope.mainGridPagerSettings = {
            currentPage: 1,
            totalDataCount: 0,
            pageChanged: function () {
                return getData();
            }
        };
        $scope.Add = function () {
            addPage()
        };
        $scope.searchClicked = function () {
          
            if ($scope.filterValue != undefined && $scope.filterValue) {
                $scope.isGettingData = true;
                return ViewAPIService.GetFilteredDynamicPages($scope.filterValue).then(function (response) {
                    $scope.dynamicViews.length = 0;
                    angular.forEach(response, function (itm) {
                        $scope.dynamicViews.push(itm);
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.isGettingData = false;
                });
            }
            else
            loadData();
                
        }
    }

    function addPage() {
        var settings = {};
        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Dynamic Page";
            modalScope.onPageAdded = function (page) {
                mainGridAPI.itemAdded(page);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPageEditor.html', null, settings);
    }

    function updatePage(dataItem) {
        var settings = {};
        
        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Dynamic Page: " + dataItem.Name;
            modalScope.onPageUpdated = function (page) {
                mainGridAPI.itemUpdated(page);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPageEditor.html', dataItem, settings);

    }
    function deletePage(dataItem) {
      
        var message = "Do you want to delete " + dataItem.Name;
        VRNotificationService.showConfirmation(message).then(function (response) {
            if (response == true) {
                return ViewAPIService.DeleteView(dataItem.ViewId).then(function (responseObject) {
                    if (responseObject.Result == DeleteOperationResultEnum.Succeeded.value)
                        mainGridAPI.itemDeleted(dataItem);
                    VRNotificationService.notifyOnItemDeleted("View", responseObject);
                    $scope.isGettingData = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                });
          }

        });
      

    }

    function load() {
        UtilsService.waitMultipleAsyncOperations([loadUsers, loadRoles])
                .finally(function () {
                    loadDynamicViews();
                    $scope.isGettingData = false;
                });
     
    }

    function loadDynamicViews() {
        $scope.isInitializing = true;
        return ViewAPIService.GetDynamicPages().then(function (response) {
            $scope.dynamicViews.length = 0;
            angular.forEach(response, function (itm) {
                $scope.dynamicViews.push(fillNeededData(itm));
                });
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        }).finally(function () {
            $scope.isGettingData = false;
        });
 
    }
   
    function fillNeededData(itm) {
    
        itm.ViewContent.DefaultPeriodDescription = UtilsService.getItemByVal($scope.periods, itm.ViewContent.DefaultPeriod, 'value').description;
        itm.ViewContent.DefaultGroupingDescription = UtilsService.getItemByVal($scope.timeDimensionTypes, itm.ViewContent.DefaultGrouping, 'value').description;
        if (itm.Audience!=null) {
            itm.Audience.UsersName = "";
            var value;
            for (var i = 0; i < itm.Audience.Users.length; i++) {
                if (itm.Audience.UsersName != "")
                    itm.Audience.UsersName += ",";
                for (var j = 0; j < $scope.users.length; j++) {
                    if(itm.Audience.Users[i]==$scope.users[i].UserId)
                        value = $scope.users[i]
                }
                if (value != null)
                itm.Audience.UsersName += value.Name;
            }
            itm.Audience.GroupsName = "";
            for (var j = 0; j < itm.Audience.Groups.length; j++) {
                if (itm.Audience.GroupsName != "")
                    itm.Audience.GroupsName += ",";
                value = UtilsService.getItemByVal($scope.roles, itm.Audience.Groups[j], 'RoleId');
                if (value != null)
                    itm.Audience.GroupsName += value.Name;

            }
        }
        else {
            itm.Audience = {
                UsersName: 'null',
                GroupsName: 'null',
            }
        }
        return itm;
    }
    function definePeriods() {
        $scope.periods = [];
        for (var p in PeriodEnum)
            $scope.periods.push(PeriodEnum[p]);
    }

    function defineTimeDimensionTypes() {
        $scope.timeDimensionTypes = [];
        for (var td in TimeDimensionTypeEnum)
            $scope.timeDimensionTypes.push(TimeDimensionTypeEnum[td]);
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

};

appControllers.controller('Security_DynamicPageManagementController', DynamicPageManagementController);