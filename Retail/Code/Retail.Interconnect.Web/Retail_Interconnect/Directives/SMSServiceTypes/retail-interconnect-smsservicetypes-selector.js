(function (app) {

    'use strict';

    smsservicetypesSelectorDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function smsservicetypesSelectorDirective(UtilsService, VRUIUtilsService, VRNotificationService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SmsservicetypesSelectorDirective(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Interconnect/Directives/SMSServiceTypes/Templates/SMSServiceTypesSelectorTemplate.html"
        };

        function SmsservicetypesSelectorDirective(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var selectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    selectorReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    promises.push(loadSelector(payload));
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return selectorAPI.getSelectedIds();
                };

                if (ctrl.onReady != undefined)
                    ctrl.onReady(api);
            }

            function loadSelector(payloadEntity) {
                var loadpromise = UtilsService.createPromiseDeferred();
                selectorReadyPromiseDeferred.promise
                    .then(function () {
                        var selectorPayload = {
                            businessEntityDefinitionId: "d5153143-a6ef-44d1-a6f8-fea6b399d853"
                        };

                        if (payloadEntity != undefined) {
                            selectorPayload.selectedIds = payloadEntity.selectedIds;
                        }
                        VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, loadpromise);
                    });
                return loadpromise.promise;
            }
        }
    }
    app.directive('retailInterconnectSmsservicetypesSelector', smsservicetypesSelectorDirective);

})(app);