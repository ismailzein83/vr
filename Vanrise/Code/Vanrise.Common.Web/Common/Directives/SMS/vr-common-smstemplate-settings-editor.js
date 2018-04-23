'use strict';

app.directive('vrCommonSmstemplateSettingsEditor', ['UtilsService', 'VRUIUtilsService', 
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
            templateUrl: "/Client/Modules/Common/Directives/SMS/Templates/SMSTemplateSettings.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {

            var smsSendHandlerSelectorAPI;
            var smsSendHandlerSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onSMSSendHandlerSelectorReady = function (api) {
                    smsSendHandlerSelectorAPI = api;
                    smsSendHandlerSelectorReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    
                    promises.push(loadSMSSendHandlerSelector());

                    function loadSMSSendHandlerSelector() {
                        var smsSendHandlerSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        smsSendHandlerSelectorReadyDeferred.promise.then(function () {
                            var smsSendHandlerSelectorPayload;
                            if (payload != undefined && payload.data != undefined && payload.data.SMSSendHandler != undefined && payload.data.SMSSendHandler.Settings != undefined)
                                smsSendHandlerSelectorPayload = { HandlerSettings: payload.data.SMSSendHandler.Settings };

                            VRUIUtilsService.callDirectiveLoad(smsSendHandlerSelectorAPI, smsSendHandlerSelectorPayload, smsSendHandlerSelectorLoadDeferred);

                            return smsSendHandlerSelectorLoadDeferred.promise;
                        });
                    }

                   // return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Entities.SMSSettingData, Vanrise.Entities",
                        SMSSendHandler: {
                            Settings: smsSendHandlerSelectorAPI.getData()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            
            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);