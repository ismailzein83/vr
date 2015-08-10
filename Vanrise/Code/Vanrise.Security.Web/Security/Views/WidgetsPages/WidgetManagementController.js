'use strict'
WidgetManagementController.$inject = ['$scope', 'UtilsService', 'WidgetAPIService', 'VRModalService', 'VRNotificationService','DeleteOperationResultEnum'];
function WidgetManagementController($scope, UtilsService, WidgetAPIService, VRModalService, VRNotificationService, DeleteOperationResultEnum) {
    var mainGridAPI;
    defineScope();
    load();

    function defineScope() {
        $scope.widgetsTypes = [];
        $scope.selectedWidgetsType;
        $scope.widgetName;
        $scope.widgets = [];
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            if(api!=undefined)
            retrieveData();
        };
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return WidgetAPIService.GetFilteredWidgets(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        };
        $scope.menuActions = [{
            name: "Edit",
            permissions: "Root/Administration Module/Dynamic Pages:Edit",
                clicked: function (dataItem) {
                 updateWidget(dataItem);
                }
        }, {
            name: "Delete",
            permissions: "Root/Administration Module/Dynamic Pages:Delete",
            clicked: function (dataItem) {
                deleteWidget(dataItem);
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
            addNewWidget();
        };
        $scope.searchClicked = function () {
                retrieveData();
        }

    }
    function retrieveData() {
        var widgetType;
        if ($scope.selectedWidgetsType != undefined)
            widgetType = $scope.selectedWidgetsType.ID;
        else
            widgetType = 0;
        var query = {
            WidgetName: $scope.widgetName,
            WidgetType: widgetType
        }
        
        return mainGridAPI.retrieveData(query);


    }
    function deleteWidget(dataItem) {
       
        var message = "Do you want to delete " + dataItem.Name + "?";
        VRNotificationService.showConfirmation(message).then(function (response) {
            if (response == true) {
                return WidgetAPIService.DeleteWidget(dataItem.Id).then(function (responseObject) {
                    if (responseObject.Result == DeleteOperationResultEnum.Succeeded.value)
                        mainGridAPI.itemDeleted(dataItem);
                    VRNotificationService.notifyOnItemDeleted("Widget", responseObject);
                    $scope.isGettingData = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    
                   
                });
            }

        });


    }
    function addNewWidget() {
        var settings = {};
        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Widget";
            modalScope.onWidgetAdded = function (widget) {
                mainGridAPI.itemAdded(widget);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/WidgetsPages/WidgetEditor.html', null, settings);

    }

    function updateWidget(dataItem) {
        var settings = {};
        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Update Widget:" + dataItem.Name;
            modalScope.onWidgetUpdated = function (widget) {
                mainGridAPI.itemUpdated(widget);
            };
        };

        VRModalService.showModal('/Client/Modules/Security/Views/WidgetsPages/WidgetEditor.html', dataItem, settings);
    }

    function load() {
        loadWidgets();
    }


    function loadWidgets() {
        return WidgetAPIService.GetWidgetsDefinition().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.widgetsTypes.push(itm);
            });
        });

    }
    
};

appControllers.controller('Security_WidgetManagementController', WidgetManagementController);