"use strict";

app.directive("vrSecViewcommonpropertiesEditor", ['VRNotificationService', 'VRModalService', 'UtilsService', 'VRUIUtilsService', function (VRNotificationService, VRModalService, UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var viewcommonpropertiesEditor = new ViewcommonpropertiesEditor($scope, ctrl, $attrs);
            viewcommonpropertiesEditor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Security/Directives/View/Templates/ViewCommonPropertiesEditorTemplate.html"

    };

    function ViewcommonpropertiesEditor($scope, ctrl, $attrs) {

        var titleResourceKeySelectorAPI;
        var titleResourceKeySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var nameResourceKeySelectorAPI;
        var nameResourceKeySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var viewEntity;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onTitleResourceKeySelectorReady = function (api) {
                titleResourceKeySelectorAPI = api;
                titleResourceKeySelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onNameResourceKeySelectorReady = function (api) {
                nameResourceKeySelectorAPI = api;
                nameResourceKeySelectorReadyDeferred.resolve();
            };
            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(getDirectiveAPI());


        }
        function getDirectiveAPI() {

            var directiveAPI = {};

            directiveAPI.load = function (payload) {
                var promises = [];

                if (payload != undefined) {
                    viewEntity = payload.viewEntity;
                }
                promises.push(loadTitleResourceKeySelector());
                promises.push(loadNameResourceKeySelector());



                return UtilsService.waitMultiplePromises(promises);
            };
            directiveAPI.setCommonProperties = function (viewSettings) {
                viewSettings.ViewTitleResourceKey = titleResourceKeySelectorAPI.getResourceKey();
                viewSettings.ViewNameResourceKey = nameResourceKeySelectorAPI.getResourceKey();
            };

            return directiveAPI;
        }

        function loadTitleResourceKeySelector() {
            var resourceKeySelectorLoadDeferred = UtilsService.createPromiseDeferred();
            titleResourceKeySelectorReadyDeferred.promise.then(function () {
                var payload = {};
                if (viewEntity != undefined && viewEntity.Settings != undefined) {
                    payload.selectedResourceKey = viewEntity.Settings.ViewTitleResourceKey;
                }
                VRUIUtilsService.callDirectiveLoad(titleResourceKeySelectorAPI, payload, resourceKeySelectorLoadDeferred);
            });
            return resourceKeySelectorLoadDeferred.promise;
        }

        function loadNameResourceKeySelector() {
            var resourceKeySelectorLoadDeferred = UtilsService.createPromiseDeferred();
            nameResourceKeySelectorReadyDeferred.promise.then(function () {
                var payload = {};
                if (viewEntity != undefined && viewEntity.Settings != undefined) {
                    payload.selectedResourceKey = viewEntity.Settings.ViewNameResourceKey;
                }
                VRUIUtilsService.callDirectiveLoad(nameResourceKeySelectorAPI, payload, resourceKeySelectorLoadDeferred);
            });
            return resourceKeySelectorLoadDeferred.promise;
        }
    }
    return directiveDefinitionObject;

}]);