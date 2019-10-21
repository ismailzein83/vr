(function (app) {

    'use strict';

    onNotificationCreatedHandler.$inject = ['VR_Notification_VRDataRecordNotificationTypeAPIService', 'UtilsService', 'VRUIUtilsService'];

    function onNotificationCreatedHandler(VR_Notification_VRDataRecordNotificationTypeAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                label: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VROnNotificationActionHandler($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "handlerCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function VROnNotificationActionHandler($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var dataRecordTypeId;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.extensionConfigs = [];
                $scope.scopeModel.selectedExtensionConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var directivePayload = { mappingDataRecordTypeId: dataRecordTypeId };
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var onNotificationCreatedHandler;
                    var action;

                    if (payload != undefined) {
                        onNotificationCreatedHandler = payload.onNotificationCreatedHandler;
                        dataRecordTypeId = payload.dataRecordTypeId;

                        if (onNotificationCreatedHandler != undefined && onNotificationCreatedHandler.Action != undefined) {
                            action = onNotificationCreatedHandler.Action;
                        }
                    }

                    var loadDataRecordNotificationActionConfigsPromise = loadDataRecordNotificationActionConfigs();
                    promises.push(loadDataRecordNotificationActionConfigsPromise);

                    if (action != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function loadDataRecordNotificationActionConfigs() {
                        return VR_Notification_VRDataRecordNotificationTypeAPIService.GetDataRecordNotificationActionConfigSettings().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.extensionConfigs.push(response[i]);
                                }
                                if (action != undefined) {
                                    $scope.scopeModel.selectedExtensionConfig = UtilsService.getItemByVal($scope.scopeModel.extensionConfigs, action.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }

                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = action;

                            if (directivePayload == undefined)
                                directivePayload = {};

                            directivePayload.mappingDataRecordTypeId = dataRecordTypeId;

                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode({ promises: promises });
                };

                api.getData = function () {
                    var action = directiveAPI.getData();

                    if (action == undefined || $scope.scopeModel.selectedExtensionConfig == undefined)
                        return undefined;

                    action.ConfigId = $scope.scopeModel.selectedExtensionConfig.ExtensionConfigurationId;
                    return {
                        Action: action
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {
            var label = "label='Action'";

            if (attrs.hidelabel != undefined) {
                label = "";
            }

            return '<vr-columns colnum="{{handlerCtrl.normalColNum}}">'
                + '<vr-select on-ready="scopeModel.onSelectorReady" datasource="scopeModel.extensionConfigs" selectedvalues="scopeModel.selectedExtensionConfig" datavaluefield="ExtensionConfigurationId" datatextfield="Title" ' + label + '>'
                + '</vr-select>'
                + '</vr-columns> '
                + '<vr-directivewrapper directive="scopeModel.selectedExtensionConfig.Editor" on-ready="scopeModel.onDirectiveReady" normal-col-num="{{handlerCtrl.normalColNum}}" isrequired="handlerCtrl.isrequired"> '
                + '</vr-directivewrapper>';
        }
    }

    app.directive('vrGenericdataDatarecordnotificationtypeOnnotificationcreatedHandler', onNotificationCreatedHandler);
})(app);