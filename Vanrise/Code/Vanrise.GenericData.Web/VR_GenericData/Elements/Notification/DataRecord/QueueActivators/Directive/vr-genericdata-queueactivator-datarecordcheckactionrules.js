(function (app) {

    'use strict';

    QueueActivatorDataRecordCheckActionRules.$inject = ['UtilsService', 'VRUIUtilsService'];

    function QueueActivatorDataRecordCheckActionRules(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new QueueActivatorDataRecordCheckActionRulesCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/QueueActivators/Directive/Templates/QueueActivatorDataRecordCheckActionRulesTemplate.html'
        };

        function QueueActivatorDataRecordCheckActionRulesCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var alertRuleTypeSelectorAPI;
            var alertRuleTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onAlertRuleTypeSelectorReady = function (api) {
                    alertRuleTypeSelectorAPI = api;
                    alertRuleTypeSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var actionRuleTypeId;

                    if (payload != undefined && payload.QueueActivator) {
                        actionRuleTypeId = payload.QueueActivator.ActionRuleTypeId;
                    }

                    //Loading AlertRuleType Selector
                    var alertRuleTypeSelectorLoadPromise = getAlertRuleTypeSelectorLoadPromise();
                    promises.push(alertRuleTypeSelectorLoadPromise);


                    function getAlertRuleTypeSelectorLoadPromise() {
                        var alertRuleTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        alertRuleTypeSelectorReadyDeferred.promise.then(function () {

                            var alertRuleTypeSelectorPayload;
                            //var alertRuleTypeSelectorPayload = {
                            //    filter: {
                            //        Filters: [{
                            //            $type: "Vanrise.Analytic.Entities.DAProfCalcVRAlertRuleTypeFilter, Vanrise.Analytic.Entities"
                            //        }]
                            //    }
                            //};
                            VRUIUtilsService.callDirectiveLoad(alertRuleTypeSelectorAPI, alertRuleTypeSelectorPayload, alertRuleTypeSelectorLoadDeferred);
                        });

                        return alertRuleTypeSelectorLoadDeferred.promise;
                    }


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.Notification.QueueActivators.DataRecordCheckActionRulesQueueActivator, Vanrise.GenericData.Notification',
                        ActionRuleTypeId: alertRuleTypeSelectorAPI.getSelectedIds(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('vrGenericdataQueueactivatorDatarecordcheckactionrules', QueueActivatorDataRecordCheckActionRules);

})(app);