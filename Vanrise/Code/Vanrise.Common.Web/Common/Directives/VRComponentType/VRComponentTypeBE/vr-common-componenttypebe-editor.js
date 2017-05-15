'use strict';

app.directive('vrCommonComponenttypebeEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VRComponentTypeBESettingsEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/Common/Directives/VRComponentType/VRComponentTypeBE/Templates/VRComponentTypeBETemplate.html'
        };

        function VRComponentTypeBESettingsEditorCtor(ctrl, $scope, $attrs) {

            var vrComponentTypeConfigSelectorAPI;
            var vrComponentTypeConfigSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onComponentTypeConfigSelectorReady = function (api) {
                    vrComponentTypeConfigSelectorAPI = api;
                    vrComponentTypeConfigSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var vrComponentTypeConfigId;

                    if (payload != undefined && payload.businessEntityDefinitionSettings) {
                        vrComponentTypeConfigId = payload.businessEntityDefinitionSettings.VRComponentTypeConfigId;
                    }

                    var vrComponentTypeConfigSelectorLoadPromise = getVRComponentTypeConfigSelectorLoadPromise();
                    promises.push(vrComponentTypeConfigSelectorLoadPromise);

                    function getVRComponentTypeConfigSelectorLoadPromise() {
                        var loadVRComponentTypeConfigPromiseDeferred = UtilsService.createPromiseDeferred();

                        vrComponentTypeConfigSelectorReadyDeferred.promise.then(function () {
                            var vrComponentTypeConfigSelectorPayload = {
                                selectedIds: vrComponentTypeConfigId
                            };
                            VRUIUtilsService.callDirectiveLoad(vrComponentTypeConfigSelectorAPI, vrComponentTypeConfigSelectorPayload, loadVRComponentTypeConfigPromiseDeferred);
                        });

                        return loadVRComponentTypeConfigPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.Common.Business.VRComponentTypeBESettings, Vanrise.Common.Business",
                        VRComponentTypeConfigId: vrComponentTypeConfigSelectorAPI.getSelectedIds()
                    };

                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]); 