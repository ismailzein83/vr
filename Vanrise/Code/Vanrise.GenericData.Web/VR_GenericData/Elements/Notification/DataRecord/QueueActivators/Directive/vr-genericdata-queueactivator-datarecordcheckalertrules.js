(function (app) {

    'use strict';

    QueueActivatorDataRecordCheckAlertRules.$inject = ['UtilsService', 'VRUIUtilsService'];

    function QueueActivatorDataRecordCheckAlertRules(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new QueueActivatorDataRecordCheckAlertRulesCtor(ctrl, $scope);
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
            templateUrl: '/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/QueueActivators/Directive/Templates/QueueActivatorDataRecordCheckAlertRulesTemplate.html'
        };

        function QueueActivatorDataRecordCheckAlertRulesCtor(ctrl, $scope) {
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

                    var dataRecordTypeId;
                    var alertRuleTypeId;

                    if (payload != undefined) {
                        dataRecordTypeId = payload.DataRecordTypeId;

                        if (payload.QueueActivator != undefined) {
                            alertRuleTypeId = payload.QueueActivator.AlertRuleTypeId;
                        }
                    }

                    //Loading AlertRuleType Selector
                    var alertRuleTypeSelectorLoadPromise = getAlertRuleTypeSelectorLoadPromise();
                    promises.push(alertRuleTypeSelectorLoadPromise);


                    function getAlertRuleTypeSelectorLoadPromise() {
                        var alertRuleTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        alertRuleTypeSelectorReadyDeferred.promise.then(function () {

                            var alertRuleTypeSelectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.GenericData.Notification.DataRecordAlertRuleTypeFilter, Vanrise.GenericData.Notification",
                                        DataRecordTypeId: dataRecordTypeId
                                    }]
                                }
                            };
                            if (alertRuleTypeId != undefined) {
                                alertRuleTypeSelectorPayload.selectedIds = alertRuleTypeId;
                            }
                            VRUIUtilsService.callDirectiveLoad(alertRuleTypeSelectorAPI, alertRuleTypeSelectorPayload, alertRuleTypeSelectorLoadDeferred);
                        });

                        return alertRuleTypeSelectorLoadDeferred.promise;
                    }


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.Notification.DataRecordCheckAlertRulesQueueActivator, Vanrise.GenericData.Notification',
                        AlertRuleTypeId: alertRuleTypeSelectorAPI.getSelectedIds(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('vrGenericdataQueueactivatorDatarecordcheckalertrules', QueueActivatorDataRecordCheckAlertRules);

})(app);