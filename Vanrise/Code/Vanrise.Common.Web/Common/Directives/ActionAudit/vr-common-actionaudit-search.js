"use strict";

app.directive("vrCommonActionauditSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VRCommon_ActionAuditLKUPEnum', 'VRDateTimeService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, VRCommon_ActionAuditLKUPEnum, VRDateTimeService) {

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
        templateUrl: "/Client/Modules/Common/Directives/ActionAudit/Templates/ActionAuditSearch.html"

    };

    function LogSearch($scope, ctrl, $attrs) {

        var gridAPI;

        var userSelectorApi;
        var userSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var moduleDirectiveApi;
        var moduleReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var actionDirectiveApi;
        var actionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var entityDirectiveApi;
        var entityReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        this.initializeController = initializeController;
        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.top = 1000;
            var fromTime = VRDateTimeService.getNowDateTime();
            fromTime.setHours(0, 0, 0);
            $scope.scopeModel.fromTime = fromTime;
            $scope.scopeModel.searchClicked = function () {
                $scope.showGrid = true;
                return gridAPI.loadGrid(getFilterObject());
            };
            $scope.scopeModel.onModuleDirectiveReady = function (api) {
                moduleDirectiveApi = api;
                moduleReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onEntityDirectiveReady = function (api) {
                entityDirectiveApi = api;
                entityReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onActionDirectiveReady = function (api) {
                actionDirectiveApi = api;
                actionReadyPromiseDeferred.resolve();
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

            UtilsService.waitMultiplePromises([userSelectorReadyPromiseDeferred.promise, actionReadyPromiseDeferred.promise, entityReadyPromiseDeferred.promise, moduleReadyPromiseDeferred.promise]).then(function () {
                defineAPI();
            });
        }

        function defineAPI() {
            var api = {};

            api.load = function () {
                var promises = [];
                $scope.isLoadingFilters = true;

                promises.push(loadUserSelector());
                promises.push(loadModuleSelector());
                promises.push(loadEntitySelector());
                promises.push(loadActionSelector());

                function loadUserSelector() {
                    var userSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    userSelectorReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(userSelectorApi, { filter: { IncludeSystemUsers: true } }, userSelectorLoadPromiseDeferred);
                        });
                    return userSelectorLoadPromiseDeferred.promise;
                }
                function loadModuleSelector() {
                    var moduleSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    moduleReadyPromiseDeferred.promise
                         .then(function () {
                             var directivePayload = {
                                 selectedIds: undefined,
                                 filter: {
                                     Type: VRCommon_ActionAuditLKUPEnum.Module.value
                                 }
                             };
                             VRUIUtilsService.callDirectiveLoad(moduleDirectiveApi, directivePayload, moduleSelectorLoadPromiseDeferred);
                         });
                    return moduleSelectorLoadPromiseDeferred.promise;
                }
                function loadEntitySelector() {
                    var entitySelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    entityReadyPromiseDeferred.promise
                         .then(function () {
                             var directivePayload = {
                                 selectedIds: undefined,
                                 filter: {
                                     Type: VRCommon_ActionAuditLKUPEnum.Entity.value
                                 }
                             };

                             VRUIUtilsService.callDirectiveLoad(entityDirectiveApi, directivePayload, entitySelectorLoadPromiseDeferred);
                         });
                    return entitySelectorLoadPromiseDeferred.promise;
                }
                function loadActionSelector() {
                    var actionSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    actionReadyPromiseDeferred.promise
                         .then(function () {
                             var directivePayload = {
                                 selectedIds: undefined,
                                 filter: {
                                     Type: VRCommon_ActionAuditLKUPEnum.Action.value
                                 }
                             };

                             VRUIUtilsService.callDirectiveLoad(actionDirectiveApi, directivePayload, actionSelectorLoadPromiseDeferred);
                         });
                    return actionSelectorLoadPromiseDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises).finally(function () {
                    $scope.isLoadingFilters = false;
                });
            };


            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

        function getFilterObject() {

            var filter = {
                UserIds: userSelectorApi.getSelectedIds(),
                TopRecord: $scope.scopeModel.top,
                ModuleIds: moduleDirectiveApi.getSelectedIds(),
                EntityIds: entityDirectiveApi.getSelectedIds(),
                ActionIds: actionDirectiveApi.getSelectedIds(),
                ObjectId: $scope.scopeModel.objectId,
                ObjectName: $scope.scopeModel.objectName,
                FromTime: $scope.scopeModel.fromTime,
                ToTime: $scope.scopeModel.toTime
            };
            return filter;
        }
    }

    return directiveDefinitionObject;

}]);
