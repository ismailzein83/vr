'use strict';
ViewManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VR_Sec_ViewAPIService','VRUIUtilsService','VR_Sec_ViewService','VR_Sec_ViewTypeAPIService','VRModalService'];

function ViewManagementController($scope, UtilsService, VRNotificationService, VR_Sec_ViewAPIService, VRUIUtilsService, VR_Sec_ViewService, VR_Sec_ViewTypeAPIService, VRModalService) {
    var mainGridAPI;
    var viewTypeDirectiveApi;
    var viewTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
    defineScope();
    load();

    function defineScope() {
        $scope.onGridReady = function (api) {
            mainGridAPI = api;
            var filter = {};
            api.loadGrid(filter);
        };

        $scope.searchClicked = function () {
            return mainGridAPI.loadGrid(getFilterObject());
        };
        $scope.onViewTypeDirectiveReady = function (api) {
            viewTypeDirectiveApi = api;
            viewTypeReadyPromiseDeferred.resolve();
        };

        $scope.addMenuActions = [];

        $scope.hasAddViewPermission = function () {
            return VR_Sec_ViewAPIService.HasAddViewPermission();
        };
    }

    function getFilterObject() {

        var query = {
            Name: $scope.viewName,
            ViewTypes: viewTypeDirectiveApi != undefined ? viewTypeDirectiveApi.getSelectedIds() : undefined
        };
        return query;
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadViewTypeSelector, loadMenuActions])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
    }

    function loadViewTypeSelector() {
        var viewTypeLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        viewTypeReadyPromiseDeferred.promise
            .then(function () {
                VRUIUtilsService.callDirectiveLoad(viewTypeDirectiveApi, undefined, viewTypeLoadPromiseDeferred);
            });
        return viewTypeLoadPromiseDeferred.promise;
    }

    function loadMenuActions()
    {
        VR_Sec_ViewTypeAPIService.GetViewTypes().then(function (response) {

            if (response)
            {
                for (var i = 0; i < response.length; i++) {
                    var viewType = response[i];
                    if (viewType.EnableAdd)
                    {
                        addMenuAction(viewType);
                    }
                }
            }
            function addMenuAction(viewType)
            {
                $scope.addMenuActions.push({
                    name: viewType.Title,
                    clicked: function () {
                        var modalSettings = {
                        };
                        var modalParameters = {
                            viewType: viewType,
                        };
                        modalSettings.onScopeReady = function (modalScope) {
                            modalScope.onViewAdded = function (Obj) {
                                if (mainGridAPI != undefined)
                                    mainGridAPI.onViewAdded(Obj);
                            };
                        };
                        VRModalService.showModal(viewType.Editor, modalParameters, modalSettings);

                    }
                });
            }

        });
    }


};

appControllers.controller('VR_Sec_ViewManagementController', ViewManagementController);