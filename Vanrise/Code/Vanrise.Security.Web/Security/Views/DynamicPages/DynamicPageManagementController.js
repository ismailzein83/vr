'use strict'
DynamicPageManagementController.$inject = ['$scope', 'ViewAPIService', 'VRModalService', 'VRNotificationService', 'DeleteOperationResultEnum', 'PeriodEnum', 'UtilsService', 'TimeDimensionTypeEnum', 'UsersAPIService', 'GroupAPIService', 'DataRetrievalResultTypeEnum'];
function DynamicPageManagementController($scope, ViewAPIService, VRModalService, VRNotificationService, DeleteOperationResultEnum, PeriodEnum, UtilsService, TimeDimensionTypeEnum, UsersAPIService, GroupAPIService, DataRetrievalResultTypeEnum) {
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
        $scope.groups=[];
        $scope.users=[];
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
         //   if ($scope.users.length != 0 && $scope.groups.length != 0)
               retrieveData();
        };
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return ViewAPIService.GetFilteredDynamicPages(dataRetrievalInput)
            .then(function (response) {
                
                if (dataRetrievalInput.DataRetrievalResultType == DataRetrievalResultTypeEnum.Normal.value) {
                    angular.forEach(response.Data, function (itm) {
                        fillNeededData(itm);
                    });                   
                }
                onResponseReady(response);
            });
        };

        $scope.menuActions = [
           {
               name: "Edit",
               permissions : "Root/Administration Module/Dynamic Pages:Edit",
               clicked: function (dataItem) {
                   updatePage(dataItem);
               }
           },
           {
               name: "Delete",
               permissions: "Root/Administration Module/Dynamic Pages:Delete",
               clicked: function (dataItem) {
                   deletePage(dataItem);
               }
           },
         {
             name: "Validate",
             permissions: "Root/Administration Module/Dynamic Pages:Validate",
             clicked: function (dataItem) {
                 validate(dataItem);
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
            addPage();
        };
            $scope.searchClicked = function () {
                return retrieveData();
            };
    }
    function retrieveData() {

        if ($scope.filterValue != undefined && $scope.filterValue) {
            return mainGridAPI.retrieveData($scope.filterValue);
        } else {
            return mainGridAPI.retrieveData($scope.filterValue);
        }

    }
    function validate(dataItem) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Validate Dynamic Page: " + dataItem.Name;
        };
        VRModalService.showModal('/Client/Modules/BI/Views/DynamicPageValidator.html', dataItem, settings);

    }
    function addPage() {
        var settings = {
          //  useModalTemplate: true,
          //  width: '95%'
        };
        settings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForAddEditor("Dynamic Page");
            modalScope.onPageAdded = function (view) {
                fillNeededData(view)
                mainGridAPI.itemAdded(view);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPageEditor.html', null, settings);
    }

    function updatePage(dataItem) {
        var settings = {
          //  useModalTemplate: true,
            //width: '95%'
        };
        
        settings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForUpdateEditor(dataItem.Name, "Dynamic Page");
            modalScope.onPageUpdated = function (view) {
                fillNeededData(view)
                mainGridAPI.itemUpdated(view);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/DynamicPages/DynamicPageEditor.html', dataItem, settings);

    }
    function deletePage(dataItem) {

        VRNotificationService.showConfirmation().then(function (response) {
            
            if (response == true) {
                return ViewAPIService.DeleteView(dataItem.ViewId).then(function (responseObject) {
                    if (responseObject.Result == DeleteOperationResultEnum.Succeeded.value) {
                        fillNeededData(dataItem)
                        mainGridAPI.itemDeleted(dataItem);
                    }
                       
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
        UtilsService.waitMultipleAsyncOperations([loadUsers, loadGroups]).then(function () {
            if(mainGridAPI!=undefined)
                return retrieveData();
        }).finally(function () {
                    $scope.isGettingData = false;
                });
     
    }

    function fillNeededData(itm) {
        if (itm.ViewContent.DefaultPeriod!=null)
            itm.ViewContent.DefaultPeriodDescription = UtilsService.getItemByVal($scope.periods, itm.ViewContent.DefaultPeriod, 'value').description;
        if (itm.ViewContent.DefaultGrouping != null)
        itm.ViewContent.DefaultGroupingDescription = UtilsService.getItemByVal($scope.timeDimensionTypes, itm.ViewContent.DefaultGrouping, 'value').description;
        if (itm.Audience!=null && itm.Audience.Users != undefined) {
            itm.Audience.UsersName = [];
            var usersArray = [];
            var value;
            for (var i = 0; i < itm.Audience.Users.length; i++) {

                value = UtilsService.getItemByVal($scope.users, itm.Audience.Users[i], 'UserId');
                if (value != null)
                    usersArray.push(value.Name);
                
            }
            itm.Audience.UsersName = usersArray.toString();
        }
        if (itm.Audience != null &&  itm.Audience.Groups != undefined) {
      
            itm.Audience.GroupsName = "";
            var groupsArray = [];
            for (var j = 0; j < itm.Audience.Groups.length; j++) {
                if (itm.Audience.GroupsName != "")
                    itm.Audience.GroupsName += ",";
                value = UtilsService.getItemByVal($scope.groups, itm.Audience.Groups[j], 'GroupId');
                if (value != null)
                    groupsArray.push(value.Name);
                

            }
            itm.Audience.GroupsName = groupsArray.toString();
        
        }
        else {
            itm.Audience = {
                UsersName: '',
                GroupsName: '',
            }
        }
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

    function loadGroups() {
        GroupAPIService.GetGroups().then(function (response) {
            angular.forEach(response, function (role) {
                $scope.groups.push(role);
            }
)
        });
    }

};

appControllers.controller('Security_DynamicPageManagementController', DynamicPageManagementController);