﻿(function (appControllers) {

    "use strict";

    BusinessProcess_BP_BusinessRuleSetManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_BPBusinessRuleSetService'];

    function BusinessProcess_BP_BusinessRuleSetManagementController($scope, UtilsService, VRUIUtilsService, BusinessProcess_BPBusinessRuleSetService) {
        var gridAPI;
        var filter = {};

        var bpDefinitionDirectiveApi;
        var bpDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        defineScope();
        loadAllControls();
        function defineScope() {
            $scope.onBPDefinitionDirectiveReady = function (api) {
                bpDefinitionDirectiveApi = api;
                bpDefinitionReadyPromiseDeferred.resolve();
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };

            $scope.searchClicked = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.addBusinessRuleSet = function () {
                var onBusinessRuleSetAdded = function (addedItem) {
                    var addedItemObj = { Entity: addedItem };
                    gridAPI.onBusinessRuleSetAdded(addedItemObj);
                };

                BusinessProcess_BPBusinessRuleSetService.addBusinessRuleSet(onBusinessRuleSetAdded);
            }
        }

        function getFilterObject() {
            filter = {
                DefinitionsId: bpDefinitionDirectiveApi.getSelectedIds(),
            };
        }

        function loadAllControls() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadBPDefinitions])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = 'Business Rule';
        }

        function loadBPDefinitions() {
            var loadBPDefinitionsPromiseDeferred = UtilsService.createPromiseDeferred();
            bpDefinitionReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(bpDefinitionDirectiveApi, undefined, loadBPDefinitionsPromiseDeferred);
            });

            return loadBPDefinitionsPromiseDeferred.promise;


        }
    }

    appControllers.controller('BusinessProcess_BP_BusinessRuleSetManagementController', BusinessProcess_BP_BusinessRuleSetManagementController);
})(appControllers);