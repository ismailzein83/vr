'use strict';

app.directive('vrGenericdataRdbrecordstoragesettingsFilterLoggedinuserfilter', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new LoggedInUserFilterCtol(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStorage/DataRecordStorage/Filters/Templates/LoggedInUserFilterTemplate.html"
        };

        function LoggedInUserFilterCtol(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var requiredPermissionAPI;
            var requiredPermissionReadyDeferred = UtilsService.createPromiseDeferred();
            var requiredPermission;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRequiredPermissionReady = function (api) {
                    requiredPermissionAPI = api;
                    requiredPermissionReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    console.log(payload);
                    var promises = [];
                    promises.push(loadRequiredPermission());
                    if (payload != undefined && payload.filter != undefined) {
                        requiredPermission = payload.filter.RequiredPermission;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.RDBDataStorage.MainExtensions.Filters.RDBDataRecordStorageLoggedInUserFilter, Vanrise.GenericData.RDBDataStorage",
                        RequiredPermission: requiredPermissionAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadRequiredPermission() {
                var requiredPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                requiredPermissionReadyDeferred.promise.then(function () {
                    var payload;

                    if (requiredPermission != null) {
                        payload = {
                            data: requiredPermission
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(requiredPermissionAPI, payload, requiredPermissionLoadDeferred);
                });

                return requiredPermissionLoadDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);