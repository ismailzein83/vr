"use strict";

app.directive("vrGenericdataNotificationtypesettingsBodyeditor", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new NotificationTypeSettingsBodyEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/Directive/Templates/NotificationTypeSettingsBodyEditorTemplate.html"
        };

        function NotificationTypeSettingsBodyEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridAPIDirectiveAPIReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridAPIDirectiveAPIReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    //Loading Grid
                    var gridLoadPromise = getGridLoadPromise();
                    promises.push(gridLoadPromise);

                    function getGridLoadPromise() {
                        var gridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        gridAPIDirectiveAPIReadyDeferred.promise.then(function () {
                            var gridQuery = payload;
                            VRUIUtilsService.callDirectiveLoad(gridAPI, gridQuery, gridLoadPromiseDeferred);
                        });

                        return gridLoadPromiseDeferred.promise;
                    }

                    UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {

                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);