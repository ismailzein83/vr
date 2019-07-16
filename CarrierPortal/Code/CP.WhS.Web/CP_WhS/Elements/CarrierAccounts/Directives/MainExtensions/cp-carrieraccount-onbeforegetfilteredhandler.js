(function (app) {

    'use strict';

    carrierAccountOnBeforeGetFilteredHandler.$inject = ['UtilsService', 'VRUIUtilsService'];

    function carrierAccountOnBeforeGetFilteredHandler(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new carrierAccountOnBeforeGetFilteredHandlerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/CP_WhS/Elements/CarrierAccounts/Directives/MainExtensions/Templates/CarrierAccountOnBeforeGetFilteredHandler.html'
        };

        function carrierAccountOnBeforeGetFilteredHandlerCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            var settings;
            var dataRecordTypeId;

            var fieldNameSelectorAPI;
            var fieldNameSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeFieldsReady = function (api) {
                    fieldNameSelectorAPI = api;
                    fieldNameSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        context = payload.context;
                        settings = payload.settings;

                        if (context != undefined) {
                            dataRecordTypeId = context.getDataRecordTypeId();
                        }
                    }

                    if (dataRecordTypeId != undefined) {
                        initialPromises.push(loadFieldNameSelector());
                    }

                    var rootPromiseNode = {
                        promises: initialPromises,
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {

                    return {
                        $type: 'CP.WhS.Business.GenericBECarrierAccountOnBeforeGetFilteredHandler, CP.WhS.Business',
                        FieldName: fieldNameSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadFieldNameSelector() {
                var loadFieldNameSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                fieldNameSelectorReadyDeferred.promise.then(function () {
                    var fieldPayload = {
                        dataRecordTypeId: dataRecordTypeId
                    };
                    if (settings != undefined) {
                        fieldPayload.selectedIds = settings.FieldName;
                    }
                    VRUIUtilsService.callDirectiveLoad(fieldNameSelectorAPI, fieldPayload, loadFieldNameSelectorPromiseDeferred);
                });
                return loadFieldNameSelectorPromiseDeferred.promise;
            }
        }
    }

    app.directive('cpCarrieraccountOnbeforegetfilteredhandler', carrierAccountOnBeforeGetFilteredHandler);

})(app);