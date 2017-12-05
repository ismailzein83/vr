"use strict";

app.directive("vrAccountmanagerGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VRCommon_ObjectTrackingService", "VR_AccountManager_AccountManagerAPIService", "VR_AccountManager_AccountManagerService","VR_AccountManager_AccountManagerDefinitionAPIService",
function (UtilsService, VRNotificationService, VRUIUtilsService, VRCommon_ObjectTrackingService, VR_AccountManager_AccountManagerAPIService, VR_AccountManager_AccountManagerService, VR_AccountManager_AccountManagerDefinitionAPIService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountManagerGrid = new AccountManagerGrid($scope, ctrl, $attrs);
            accountManagerGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/VR_AccountManager/Elements/AccountManager/Directives/Template/AccountManagerGrid.html'
    };

    function AccountManagerGrid($scope, ctrl, $attrs) {

        var gridAPI;

        var accountManagerDefinitionId;

        var accountManagerSubViewsDefinitions;

        this.initializeController = initializeController;
        
        function initializeController()
        {
            
            $scope.accountManagers = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveAPI());
                }

                function getDirectiveAPI() {
                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (payload) {
                        var promises = [];
                        var gridQuery;
                        if (payload != undefined) {
                            accountManagerDefinitionId = payload.accountManagerDefinitionId;
                            gridQuery = payload.query;
                        }
                        var accountManagerSubViewsDefinitionsLoadPromise = getAccountManagerSubViewsDefinitionLoadPromise();
                        promises.push(accountManagerSubViewsDefinitionsLoadPromise);
                        var gridLoadDeferred = UtilsService.createPromiseDeferred();
                        UtilsService.waitMultiplePromises(promises).then(function () {

                            gridAPI.retrieveData(gridQuery).then(function () {
                                gridLoadDeferred.resolve();
                            }).catch(function (error) {
                                gridLoadDeferred.reject(error);
                            });

                        }).catch(function (error) {
                            gridLoadDeferred.reject(error);
                        });
                        function getAccountManagerSubViewsDefinitionLoadPromise() {
                            return VR_AccountManager_AccountManagerDefinitionAPIService.GetAccountManagerSubViewsDefinition(accountManagerDefinitionId).then(function (response) {
                                accountManagerSubViewsDefinitions = response;

                            });
                        }
                        return gridLoadDeferred.promise;
                    };
                    directiveAPI.onAccountManagerAdded = function (accountManagerObject) {
                        VR_AccountManager_AccountManagerService.defineAccountManagerSubViewTabs(accountManagerDefinitionId, accountManagerObject, gridAPI, accountManagerSubViewsDefinitions);
                        gridAPI.itemAdded(accountManagerObject);
                    };

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                
                return VR_AccountManager_AccountManagerAPIService.GetFilteredAccountManagers(dataRetrievalInput)
                   .then(function (response) {
                       if (response && response.Data) {
                           for (var i = 0; i < response.Data.length; i++) {
                               var accountManager = response.Data[i];
                               VR_AccountManager_AccountManagerService.defineAccountManagerSubViewTabs(accountManagerDefinitionId, accountManager, gridAPI, accountManagerSubViewsDefinitions);
                           }
                       }
                       onResponseReady(response);
                   })
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   });
            };
            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editAccountmanager,
                haspermission: hasEditAccountManagerPermission
            }];
        }

        function editAccountmanager(accountManagerObject) {
            var onAccountManagerUpdated = function (updatedItem) {
                VR_AccountManager_AccountManagerService.defineAccountManagerSubViewTabs(accountManagerDefinitionId, updatedItem, gridAPI, accountManagerSubViewsDefinitions);
                gridAPI.itemUpdated(updatedItem);
            };
            
            VR_AccountManager_AccountManagerService.editAccountmanager(accountManagerObject, onAccountManagerUpdated);
        }
        function hasEditAccountManagerPermission() {
            if (accountManagerDefinitionId != undefined)
                return VR_AccountManager_AccountManagerAPIService.HasEditAccountManagerPermission(accountManagerDefinitionId);
        }
    }
    return directiveDefinitionObject;
}]);