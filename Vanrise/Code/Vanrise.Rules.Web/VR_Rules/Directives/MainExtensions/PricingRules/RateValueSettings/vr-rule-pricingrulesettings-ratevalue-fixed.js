(function (app) {

    'use strict';

    FixedRateValueSettingsDirective.$inject = ['VRCommon_RateTypeAPIService', 'UtilsService', 'VRUIUtilsService'];

    function FixedRateValueSettingsDirective(VRCommon_RateTypeAPIService, UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var rateValueSettings = new RateValueSettings(ctrl, $scope, $attrs);
                rateValueSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Rules/Directives/MainExtensions/PricingRules/RateValueSettings/Templates/PricingRuleFixedRateValueTemplate.html"
        };

        function RateValueSettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.dataSource = [];

                ctrl.onGridReady = function (api) {
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.getData = function () {
                    var data = {
                        $type: 'Vanrise.Rules.Pricing.MainExtensions.RateValue.FixedRateValueSettings, Vanrise.Rules.Pricing.MainExtensions',
                        NormalRate: ctrl.normalRate
                    };

                    var dictionary = {};
                    for (var i = 0; i < ctrl.dataSource.length; i++) {
                        var dataItem = ctrl.dataSource[i];
                        if (dataItem.RateTypeValue != null) {
                            dictionary[dataItem.RateTypeId] = dataItem.RateTypeValue;
                        }
                    }

                    var hasOwnProperties = false;
                    for (var key in dictionary) {
                        if (dictionary.hasOwnProperty(key)) {
                            hasOwnProperties = true;
                            break;
                        }
                    }

                    data.RatesByRateType = (hasOwnProperties) ? dictionary : null;
                    return data;
                };

                api.load = function (payload) {
                    var promises = [];
                    var ratesByRateType;

                    if (payload != undefined) {
                        ctrl.normalRate = payload.NormalRate;
                        ratesByRateType = payload.RatesByRateType;
                    }

                    var getAllRateTypesPromise = VRCommon_RateTypeAPIService.GetAllRateTypes();
                    promises.push(getAllRateTypesPromise);

                    getAllRateTypesPromise.then(function (response) {
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                var dataItem = {
                                    RateTypeId: response[i].RateTypeId,
                                    RateTypeName: response[i].Name
                                };

                                if (ratesByRateType != undefined) {
                                    for (var prop in ratesByRateType) {
                                        if (prop == dataItem.RateTypeId) {
                                            dataItem.RateTypeValue = ratesByRateType[prop];
                                            break;
                                        }  
                                    }     
                                }

                                ctrl.dataSource.push(dataItem);
                            }
                        }
                    });

                    return UtilsService.waitMultiplePromises(promises);
                };

                return api;
            }
        }
    }

    app.directive('vrRulesPricingrulesettingsRatevalueFixed', FixedRateValueSettingsDirective);

})(app);