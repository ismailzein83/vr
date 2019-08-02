'use strict';

app.directive('businessprocessVrWorkflowactivityAddcommentSettings', ['UtilsService', 'VR_Sec_UserService', 'VRUIUtilsService','VRCommon_FieldTypesService',
    function (UtilsService, VR_Sec_UserService, VRUIUtilsService, VRCommon_FieldTypesService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                remove: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new addBusinessEntitySettings(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowActivityAddCommentSettingsTemplate.html'
        };

        function addBusinessEntitySettings(ctrl, $scope, $attrs) {
            var context;
            var settings;

            var isSucceededExpressionBuilderDirectiveAPI;
            var isSucceededExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();

            var userIdExpressionBuilderDirectiveAPI;
            var userIdExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();

            var contentExpressionBuilderDirectiveAPI;
            var contentExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();

            var objectIdExpressionBuilderDirectiveAPI;
            var objectIdExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();

            var textFieldType = VRCommon_FieldTypesService.getTextFieldType();
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onIsSucceededExpressionBuilderDirectiveReady = function (api) {
                    isSucceededExpressionBuilderDirectiveAPI = api;
                    isSucceededExpressionBuilderPromiseReadyDeffered.resolve();
                };
                $scope.scopeModel.onUserIdExpressionBuilderDirectiveReady = function (api) {
                    userIdExpressionBuilderDirectiveAPI = api;
                    userIdExpressionBuilderPromiseReadyDeffered.resolve();
                };
                $scope.scopeModel.onContentExpressionBuilderDirectiveReady = function (api) {
                    contentExpressionBuilderDirectiveAPI = api;
                    contentExpressionBuilderPromiseReadyDeffered.resolve();
                };
                $scope.scopeModel.onObjectIdExpressionBuilderDirectiveReady = function (api) {
                    objectIdExpressionBuilderDirectiveAPI = api;
                    objectIdExpressionBuilderPromiseReadyDeffered.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    $scope.scopeModel.objectId = undefined;
                    $scope.scopeModel.content = undefined;
                    $scope.scopeModel.isSucceeded = undefined;
                    $scope.scopeModel.userId = undefined;
                    var promises = [];

                    if (payload != undefined) {
                        context = payload.context;
                         settings = payload.settings;

                        promises.push(loadContentExpressionBuilder());
                        promises.push(loadIsSucceededExpressionBuilder());
                        promises.push(loadObjectIdExpressionBuilder());
                        promises.push(loadUserIdExpressionBuilder());
                    }
                    return UtilsService.waitPromiseNode({ promises: promises});
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.BEActivities.VRWorkflowAddCommentActivity, Vanrise.BusinessProcess.MainExtensions",
                        ObjectId: objectIdExpressionBuilderDirectiveAPI != undefined ? objectIdExpressionBuilderDirectiveAPI.getData() : undefined,
                        Content: contentExpressionBuilderDirectiveAPI != undefined ? contentExpressionBuilderDirectiveAPI.getData() : undefined,
                        IsSucceeded: isSucceededExpressionBuilderDirectiveAPI != undefined ? isSucceededExpressionBuilderDirectiveAPI.getData() : undefined,
                        UserId: userIdExpressionBuilderDirectiveAPI != undefined ? userIdExpressionBuilderDirectiveAPI.getData() : undefined,
                    };
                };
              
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadIsSucceededExpressionBuilder() {
                var isSucceededExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                isSucceededExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: settings != undefined ? settings.IsSucceeded : undefined,
                    };
                    VRUIUtilsService.callDirectiveLoad(isSucceededExpressionBuilderDirectiveAPI, payload, isSucceededExpressionBuilderPromiseLoadDeffered);
                });
                return isSucceededExpressionBuilderPromiseLoadDeffered.promise;
            }
            function loadUserIdExpressionBuilder() {

                var userIdExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                userIdExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: settings != undefined ? settings.UserId : undefined,
                        fieldType: VR_Sec_UserService.getUserIdFieldType()
                    };
                    VRUIUtilsService.callDirectiveLoad(userIdExpressionBuilderDirectiveAPI, payload, userIdExpressionBuilderPromiseLoadDeffered);
                });
                return userIdExpressionBuilderPromiseLoadDeffered.promise;
            }

            function loadContentExpressionBuilder() {
                var contentExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                contentExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: settings != undefined ? settings.Content : undefined,
                        fieldType: textFieldType
                    };
                    VRUIUtilsService.callDirectiveLoad(contentExpressionBuilderDirectiveAPI, payload, contentExpressionBuilderPromiseLoadDeffered);
                });
                return contentExpressionBuilderPromiseLoadDeffered.promise;
            }
            function loadObjectIdExpressionBuilder() {

                var objectIdExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                objectIdExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: settings != undefined ? settings.ObjectId : undefined,
                        fieldType: textFieldType
                    };
                    VRUIUtilsService.callDirectiveLoad(objectIdExpressionBuilderDirectiveAPI, payload, objectIdExpressionBuilderPromiseLoadDeffered);
                });
                return objectIdExpressionBuilderPromiseLoadDeffered.promise;
            }
        }
        return directiveDefinitionObject;
    }]);