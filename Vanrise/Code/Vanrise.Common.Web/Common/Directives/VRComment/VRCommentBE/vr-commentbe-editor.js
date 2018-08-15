'use strict';

app.directive('vrCommentbeEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VRCommentBESettingsEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/Common/Directives/VRComment/VRCommentBE/Templates/VRCommentBETemplate.html'
        };

        function VRCommentBESettingsEditorCtor(ctrl, $scope, $attrs) {

            var viewPermissionAPI;
            var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var addPermissionAPI;
            var addPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onViewRequiredPermissionReady = function (api) {
                    viewPermissionAPI = api;
                    viewPermissionReadyDeferred.resolve();
                };

                $scope.scopeModel.onAddRequiredPermissionReady = function (api) {
                    addPermissionAPI = api;
                    addPermissionReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var loadViewRequiredPermissionPromise = loadViewRequiredPermission();
                    promises.push(loadViewRequiredPermissionPromise);

                    var loadAddRequiredPermissionPromise = loadAddRequiredPermission();
                    promises.push(loadAddRequiredPermissionPromise);
                   
                    function loadViewRequiredPermission() {
                        var viewSettingPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        viewPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: payload && payload.businessEntityDefinitionSettings && payload.businessEntityDefinitionSettings.Security && payload.businessEntityDefinitionSettings.Security.ViewRequiredPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(viewPermissionAPI, dataPayload, viewSettingPermissionLoadDeferred);
                        });
                        return viewSettingPermissionLoadDeferred.promise;
                    }
                 

                    function loadAddRequiredPermission() {
                        var addPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        addPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: payload && payload.businessEntityDefinitionSettings && payload.businessEntityDefinitionSettings.Security && payload.businessEntityDefinitionSettings.Security.AddRequiredPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(addPermissionAPI, dataPayload, addPermissionLoadDeferred);
                        });
                        return addPermissionLoadDeferred.promise;
                    }
                   

                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.Common.Business.VRCommentBEDefinitionSettings, Vanrise.Common.Business",
                        Security: buildSecurityObj()
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function buildSecurityObj() {
                return {
                    ViewRequiredPermission: viewPermissionAPI.getData(),
                    AddRequiredPermission: addPermissionAPI.getData()
                };
            };


            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]); 