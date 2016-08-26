'use strict';

app.directive('vrWhsRoutingRouteSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RouteSettings/Templates/RouteSettingsTemplate.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var mailMessageTemplateSettingsDatabaseConfigurationAPI;
            var mailMessageTemplateSettingsDatabaseConfigurationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};

            $scope.scopeModel.onMailMessageTemplateSettingsReady = function (api) {
                mailMessageTemplateSettingsDatabaseConfigurationAPI = api;
                mailMessageTemplateSettingsDatabaseConfigurationReadyPromiseDeferred.resolve();
            }

            function initializeController() {
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var mailMessageTemplateSettingsPayload;
                    var productRoutePayload;
                    var subProcessSettingsPayload;
                    var mailMessageTemplateSettingsSettingsPayload;

                    if (payload != undefined && payload.data != undefined) {
                        mailMessageTemplateSettingsPayload = payload.data.RouteDatabasesToKeep.MailMessageTemplateSettingsConfiguration;
                        productRoutePayload = payload.data.RouteDatabasesToKeep.ProductRouteConfiguration;
                        subProcessSettingsPayload = payload.data.SubProcessSettings;
                        mailMessageTemplateSettingsSettingsPayload = payload.data.RouteBuildConfiguration;
                    }

                    //Loading Customer Route Database Configuration
                    var mailMessageTemplateSettingsDatabaseConfigurationLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    mailMessageTemplateSettingsDatabaseConfigurationReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(mailMessageTemplateSettingsDatabaseConfigurationAPI, mailMessageTemplateSettingsPayload, mailMessageTemplateSettingsDatabaseConfigurationLoadPromiseDeferred);
                        });

                    return mailMessageTemplateSettingsDatabaseConfigurationLoadPromiseDeferred.promise;
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Entities.RouteSettingsData, TOne.WhS.Routing.Entities",
                        RouteDatabasesToKeep: {
                            MailMessageTemplateSettingsConfiguration: mailMessageTemplateSettingsDatabaseConfigurationAPI.getData(),
                            ProductRouteConfiguration: productRouteDatabaseConfigurationAPI.getData()
                        },
                        SubProcessSettings: subProcessSettingsAPI.getData(),
                        RouteBuildConfiguration: mailMessageTemplateSettingsSettingsAPI.getData()
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);