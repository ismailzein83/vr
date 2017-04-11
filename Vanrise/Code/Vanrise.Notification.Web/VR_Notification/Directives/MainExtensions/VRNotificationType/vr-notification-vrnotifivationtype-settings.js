(function (app) {

    'use strict';

    vrNotificationTypeSettings.$inject = ['VR_Notification_VRNotificationTypeAPIService', 'UtilsService', 'VRUIUtilsService'];

    function vrNotificationTypeSettings(VR_Notification_VRNotificationTypeAPIService, utilsService, vruiUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                label: '@',
                customvalidate: '=',
                customlabel: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VRNotificationTypeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "typeSettingsCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function VRNotificationTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.extensionConfigs = [];
                $scope.selectedExtensionConfig;

                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var directivePayload = undefined;
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var settings;

                    if (payload != undefined) {
                        settings = payload.extendedSettings;
                    }

                    var loadVRNotificationTypeSettingsPromise = loadVRNotificationTypeSettings();
                    promises.push(loadVRNotificationTypeSettingsPromise);

                    if (settings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function loadVRNotificationTypeSettings() {
                        return VR_Notification_VRNotificationTypeAPIService.GetVRNotificationTypeDefinitionConfigSettings().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.extensionConfigs.push(response[i]);
                                }
                                if (settings != undefined)
                                    $scope.selectedExtensionConfig = utilsService.getItemByVal($scope.extensionConfigs, settings.ConfigId, 'ExtensionConfigurationId');

                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = utilsService.createPromiseDeferred();
                        var directiveLoadDeferred = utilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = settings;
                            vruiUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return utilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = directiveAPI.getData();
                    if (data != undefined)
                        data.ConfigId = $scope.selectedExtensionConfig.ExtensionConfigurationId;
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {
            var label = "label='Notification Type'";

            if (attrs.hidelabel != undefined) {
                label = "";
            }

            if(attrs.customlabel != undefined) {
                label = "label='" + attrs.customlabel + "'";
            }

            return '<vr-columns colnum="{{typeSettingsCtrl.normalColNum}}">' +
                        '<vr-select on-ready="onSelectorReady" datasource="extensionConfigs" selectedvalues="selectedExtensionConfig" datavaluefield="ExtensionConfigurationId" datatextfield="Title" ' + label +
                           ' isrequired="typeSettingsCtrl.isrequired" hideremoveicon>' +
                        '</vr-select>' +
                     '</vr-columns>' +
                     '<vr-directivewrapper directive="selectedExtensionConfig.Editor" on-ready="onDirectiveReady" ' +
                        'normal-col-num="{{typeSettingsCtrl.normalColNum}}" isrequired="typeSettingsCtrl.isrequired" customvalidate="typeSettingsCtrl.customvalidate"> ' +
                     '</vr-directivewrapper>';
        }
    }

    app.directive('vrNotificationVrnotifivationtypeSettings', vrNotificationTypeSettings);
})(app);