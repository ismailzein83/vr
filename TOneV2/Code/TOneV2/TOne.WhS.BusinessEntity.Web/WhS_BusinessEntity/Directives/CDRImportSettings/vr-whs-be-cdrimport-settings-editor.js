'use strict';

app.directive('vrWhsBeCdrimportSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new cdrImportEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CDRImportSettings/Templates/CDRImportSettingsTemplate.html"
        };

        function cdrImportEditorCtor(ctrl, $scope, $attrs) {

            var switchCDRProcessConfigurationDirectiveAPI;
            var switchCDRProcessConfigurationDirectiveDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSwitchCDRProcessConfiguration = function (api) {
                    switchCDRProcessConfigurationDirectiveAPI = api;
                    switchCDRProcessConfigurationDirectiveDeferred.resolve();
                };


                UtilsService.waitMultiplePromises([switchCDRProcessConfigurationDirectiveDeferred.promise]).then(function () {
                    defineAPI();
                });

            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var switchCDRProcessConfiguration;

                    if (payload != undefined && payload.data != undefined) {
                        switchCDRProcessConfiguration = payload.data.SwitchCDRProcessConfiguration;
                    }

                    var promises = [];

                    //Loading SwitchCDRProcessConfiguration Directive
                    var switchCDRProcessConfigurationLoadDeferred = UtilsService.createPromiseDeferred();
                    var switchCDRProcessConfigurationPayload = {
                        switchCDRProcessConfiguration: switchCDRProcessConfiguration
                    };
                    VRUIUtilsService.callDirectiveLoad(switchCDRProcessConfigurationDirectiveAPI, switchCDRProcessConfigurationPayload, switchCDRProcessConfigurationLoadDeferred);
                    promises.push(switchCDRProcessConfigurationLoadDeferred.promise);


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.CDRImportSettings, TOne.WhS.BusinessEntity.Entities",
                        SwitchCDRProcessConfiguration: switchCDRProcessConfigurationDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);