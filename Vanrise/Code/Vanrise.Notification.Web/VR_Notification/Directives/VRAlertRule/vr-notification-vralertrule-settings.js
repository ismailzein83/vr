
'use strict';

app.directive('vrNotificationVralertruleSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var cstr = new AlertRuleSettings($scope, ctrl, $attrs);
                cstr.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Notification/Directives/VRAlertRule/Templates/VRAlertRuleSettingsTemplate.html'
        };

        function AlertRuleSettings($scope, ctrl, $attrs) {

            var criteriaDirectiveAPI;
            var criteriaDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var criteriaDefinitionFields;
            var criteriaFieldsValues;

            this.initializeController = initializeController;


            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onCriteriaDirectiveReady = function (api) {
                    criteriaDirectiveAPI = api;
                    criteriaDirectiveReadyPromiseDeferred.resolve();
                }

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];



                    if (payload != undefined) {

                    }


                    var loadCriteriaSectionPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadCriteriaSectionPromiseDeferred.promise);


                    criteriaDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            criteriaDefinitionFields: criteriaDefinitionFields,
                            criteriaFieldsValues: criteriaFieldsValues
                        };
                        VRUIUtilsService.callDirectiveLoad(criteriaDirectiveAPI, payload, loadCriteriaSectionPromiseDeferred);
                    });

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return directiveAPI.getData();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }
    }]);

