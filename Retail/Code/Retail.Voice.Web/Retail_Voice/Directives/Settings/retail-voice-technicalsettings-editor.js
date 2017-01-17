'use strict';

app.directive('retailVoiceTechnicalsettingsEditor', ['UtilsService','VRUIUtilsService',
function (UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new settingsCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/Retail_Voice/Directives/Settings/Templates/VoiceTechnicalSettings.html'
    };


    function settingsCtor(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;

        var accountIdentificationTemplateReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var accountIdentificationTemplateDirectiveApi;

        var internationalIdentificationTemplateReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var internationalIdentificationTemplateDirectiveApi;

        function initializeController() {

            $scope.onAccountIdentificationTemplateReady = function (api) {
                accountIdentificationTemplateDirectiveApi = api;
                accountIdentificationTemplateReadyPromiseDeferred.resolve();
            };

            $scope.onInternationalIdentificationTemplateReady = function (api) {
                internationalIdentificationTemplateDirectiveApi = api;
                internationalIdentificationTemplateReadyPromiseDeferred.resolve();
            };
            
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                var loadAccountIdentificationTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
                accountIdentificationTemplateReadyPromiseDeferred.promise.then(function () {
                    var accountIdentificationPayload;
                    if (payload != undefined && payload.data !=undefined && payload.data.AccountIdentification != undefined) {
                        accountIdentificationPayload = payload.data.AccountIdentification;
                    }
                    VRUIUtilsService.callDirectiveLoad(accountIdentificationTemplateDirectiveApi, accountIdentificationPayload, loadAccountIdentificationTemplatePromiseDeferred);
                });
                promises.push(loadAccountIdentificationTemplatePromiseDeferred.promise);

                var loadInternationalIdentificationTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
                internationalIdentificationTemplateReadyPromiseDeferred.promise.then(function () {
                    var internationalIdentificationPayload;
                    if (payload != undefined && payload.data != undefined && payload.data.InternationalIdentification != undefined) {
                        internationalIdentificationPayload = payload.data.InternationalIdentification;
                    }
                    VRUIUtilsService.callDirectiveLoad(internationalIdentificationTemplateDirectiveApi, internationalIdentificationPayload, loadInternationalIdentificationTemplatePromiseDeferred);
                });
                promises.push(loadInternationalIdentificationTemplatePromiseDeferred.promise);

                return UtilsService.waitMultiplePromises(promises);
            };


            api.getData = function () {
                var obj = {
                    $type: "Retail.Voice.Entities.VoiceTechnicalSettings, Retail.Voice.Entities",
                    AccountIdentification: accountIdentificationTemplateDirectiveApi.getData(),
                    InternationalIdentification: internationalIdentificationTemplateDirectiveApi.getData()
                };
                return obj;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        };
    }

    return directiveDefinitionObject;
}]);