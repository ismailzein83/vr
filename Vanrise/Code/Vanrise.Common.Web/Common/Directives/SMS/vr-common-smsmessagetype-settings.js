(function (app) {

    'use strict';

    SMSMessageTypeSettings.$inject = ['UtilsService', 'VRUIUtilsService'];

    function SMSMessageTypeSettings(UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var smsMessageTypeSettingsDirective = new SMSMessageTypeSettingsDirective(ctrl, $scope, $attrs);
                smsMessageTypeSettingsDirective.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Common/Directives/SMS/Templates/SMSMessageTypeSettings.html'
        };

        function SMSMessageTypeSettingsDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var objectDirectiveAPI;
            var objectDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onObjectDirectiveReady = function (api) {
                    objectDirectiveAPI = api;
                    objectDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    console.log(payload);

                    if (payload != undefined) {
                        $scope.scopeModel.name = payload.componentType.Name;
                    }

                    promises.push(loadObjectDirective());

                    function loadObjectDirective() {
                        var objectDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        objectDirectiveReadyDeferred.promise.then(function () {
                            var objectDirectivePayload;

                            if (payload != undefined && payload.componentType != undefined && payload.componentType.Settings != undefined && payload.componentType.Settings.Objects != undefined) {
                                objectDirectivePayload = {
                                    objects: payload.componentType.Settings.Objects
                                };
                            }

                            VRUIUtilsService.callDirectiveLoad(objectDirectiveAPI, objectDirectivePayload, objectDirectiveLoadDeferred);
                        });

                        return objectDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        Name: $scope.scopeModel.name,
                        Settings:{
                            $type: "Vanrise.Entities.SMSMessageTypeSettings, Vanrise.Entities",
                            Objects: objectDirectiveAPI.getData()
                       }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

            }
        }
        return directiveDefinitionObject;
    }

    app.directive('vrCommonSmsmessagetypeSettings', SMSMessageTypeSettings);

})(app);
