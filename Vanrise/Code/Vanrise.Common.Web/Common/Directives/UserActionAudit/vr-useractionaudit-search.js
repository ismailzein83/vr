"use strict";

app.directive("vrUseractionauditSearch", [ 'VRNotificationService', 'UtilsService', 'VRUIUtilsService','VRValidationService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var logSearch = new LogSearch($scope, ctrl, $attrs);
            logSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/UserActionAudit/Templates/UserActionAuditSearch.html"

    };

    function LogSearch($scope, ctrl, $attrs) {


        var gridAPI;

        var userSelectorApi;
        var userSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        this.initializeController = initializeController;
        function initializeController() {
            
            defineScope();

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(getDirectiveAPI());
            function getDirectiveAPI() {

                var directiveAPI = {};
                directiveAPI.load = function () {
                    return load();
                };
                return directiveAPI;
            }

        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.top = 1000;
            var fromTime = new Date();
            fromTime.setHours(0, 0, 0);
            $scope.scopeModel.fromTime = fromTime;
            $scope.scopeModel.searchClicked = function () {
                $scope.showGrid = true;
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.scopeModel.onUserSelectorReady = function (api) {
                userSelectorApi = api;
                userSelectorReadyPromiseDeferred.resolve();
            };           
            $scope.scopeModel.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.scopeModel.fromTime, $scope.scopeModel.toTime);
            };
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };
        }

        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadUserSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }
        function loadUserSelector() {
            var userSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            userSelectorReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(userSelectorApi, undefined, userSelectorLoadPromiseDeferred);
                });
            return userSelectorLoadPromiseDeferred.promise;
        }

       
        function getFilterObject() {
           var  filter = {
                UserIds: userSelectorApi.getSelectedIds(),
                TopRecord: $scope.scopeModel.top,
                BaseUrl: $scope.scopeModel.baseUrl,
                Module: $scope.scopeModel.module,
                Controller: $scope.scopeModel.controller,
                Action: $scope.scopeModel.action,
                FromTime: $scope.scopeModel.fromTime,
                ToTime: $scope.scopeModel.toTime
            };
           return filter;
        }


    }

    return directiveDefinitionObject;

}]);
