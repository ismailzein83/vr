(function (app) {

    'use strict';

    PricingTemplateRuleManagementDirective.$inject = ['UtilsService', 'VRNotificationService', 'WhS_Sales_PricingTemplateService'];

    function PricingTemplateRuleManagementDirective(UtilsService, VRNotificationService, WhS_Sales_PricingTemplateService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new PricingTemplateRuleManagementCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/WhS_Sales/Directives/PricingTemplate/Templates/PricingTemplateRuleManagementTemplate.html'
        };

        function PricingTemplateRuleManagementCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var countryNameByIds;
            var zoneNameByIds;
            var context;

            var selectedCountries = [];
            var selectedZones = [];

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.pricingTemplateRules = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onAddPricingTemplateRule = function () {
                    var onPricingTemplateRuleAdded = function (addedPricingTemplateRule) {
                        extendPricingTemplateRuleObj(addedPricingTemplateRule, undefined);
                        $scope.scopeModel.pricingTemplateRules.push({ Entity: addedPricingTemplateRule });
                    };

                    WhS_Sales_PricingTemplateService.addPricingTemplateRule(context, onPricingTemplateRuleAdded);
                };

                $scope.scopeModel.onDeletePricingTemplateRule = function (pricingTemplateRule) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.pricingTemplateRules, pricingTemplateRule.Entity.PricingTemplateRuleIndex, 'Entity.PricingTemplateRuleIndex');
                            $scope.scopeModel.pricingTemplateRules.splice(index, 1);
                        }
                    });
                };

                $scope.scopeModel.validate = function () {
                    if ($scope.scopeModel.pricingTemplateRules.length == 0)
                        return 'You Should Define Rules!!';
                    return null;
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.pricingTemplateRules.length = 0;

                    var pricingTemplateRules;

                    if (payload != undefined) {
                        context = payload.context;
                        pricingTemplateRules = payload.pricingTemplateRules;

                        var pricingTemplateRulesEditorRuntime = payload.pricingTemplateRulesEditorRuntime;
                        if (pricingTemplateRulesEditorRuntime != undefined) {
                            countryNameByIds = pricingTemplateRulesEditorRuntime.CountryNameByIds;
                            zoneNameByIds = pricingTemplateRulesEditorRuntime.ZoneNameByIds;
                        }
                    }

                    //Loading PricingTemplateRules Grid
                    if (pricingTemplateRules != undefined) {
                        for (var index = 0; index < pricingTemplateRules.length; index++) {
                            var currentPricingTemplateRule = pricingTemplateRules[index];

                            var countries = currentPricingTemplateRule.Countries;
                            if (countries != undefined) {
                                for (var index = 0; index < countries.length; index++) {
                                    selectedCountries.push(countries[index]);
                                }
                            }

                            extendPricingTemplateRuleObj(currentPricingTemplateRule, index);
                            $scope.scopeModel.pricingTemplateRules.push({ Entity: currentPricingTemplateRule });
                        }
                    }
                };

                api.getData = function () {

                    var pricingTemplateRules;
                    if ($scope.scopeModel.pricingTemplateRules.length > 0) {
                        pricingTemplateRules = [];
                        for (var i = 0; i < $scope.scopeModel.pricingTemplateRules.length; i++) {
                            var pricingTemplateRule = $scope.scopeModel.pricingTemplateRules[i].Entity;
                            pricingTemplateRules.push({ Countries: pricingTemplateRule.Countries, Zones: pricingTemplateRule.Zones, Rates: pricingTemplateRule.Rates });
                        }
                    }

                    return pricingTemplateRules;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editPricingTemplateRuleDefinition
                }];
            }
            function editPricingTemplateRuleDefinition(pricingTemplateRule) {
                var onPricingTemplateRuleUpdated = function (updatedPricingTemplateRule) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.pricingTemplateRules, pricingTemplateRule.Entity.PricingTemplateRuleIndex, 'Entity.PricingTemplateRuleIndex');
                    extendPricingTemplateRuleObj(updatedPricingTemplateRule, undefined);
                    $scope.scopeModel.pricingTemplateRules[index] = { Entity: updatedPricingTemplateRule };
                };

                WhS_Sales_PricingTemplateService.editPricingTemplateRule(pricingTemplateRule.Entity, context, onPricingTemplateRuleUpdated);
            }

            function extendPricingTemplateRuleObj(pricingTemplateRule, pricingTemplateRuleIndex) {
                if (pricingTemplateRule == undefined)
                    return;

                //Index
                if (pricingTemplateRuleIndex != undefined)
                    pricingTemplateRule.PricingTemplateRuleIndex = pricingTemplateRuleIndex;

                //Countries
                if (pricingTemplateRule.CountriesName != undefined) {
                    pricingTemplateRule.CountriesAsString = pricingTemplateRule.CountriesName.join(", ");
                }
                else {
                    var countriesName = [];
                    for (var index = 0; index < pricingTemplateRule.Countries.length; index++) {
                        var currentCountry = pricingTemplateRule.Countries[index];
                        countriesName.push(countryNameByIds[currentCountry.CountryId]);
                    }
                    pricingTemplateRule.CountriesName = countriesName;
                    pricingTemplateRule.CountriesAsString = pricingTemplateRule.CountriesName.join(", ");
                }

                //Zones
                if (pricingTemplateRule.ZonesName != undefined) {
                    var tempIncludedZoneNames = UtilsService.getPropValuesFromArray(pricingTemplateRule.ZonesName, "IncludedZoneNames");
                    pricingTemplateRule.ZonesAsString = tempIncludedZoneNames ? tempIncludedZoneNames.join(", ") : undefined;
                }
                else {
                    var zonesName = [];
                    for (var i = 0; i < pricingTemplateRule.Zones.length; i++) {
                        var currentZonePricingTemplate = pricingTemplateRule.Zones[i];
                        var includedZoneNames = { IncludedZoneNames: [] };

                        for (var j = 0; j < currentZonePricingTemplate.IncludedZoneIds.length; j++) {
                            var currentZoneId = currentZonePricingTemplate.IncludedZoneIds[j];
                            includedZoneNames.IncludedZoneNames.push(zoneNameByIds[currentZoneId]);
                        }
                        zonesName.push(includedZoneNames)
                    }
                    pricingTemplateRule.ZonesName = zonesName;
                    var tempIncludedZoneNames = UtilsService.getPropValuesFromArray(pricingTemplateRule.ZonesName, "IncludedZoneNames");
                    pricingTemplateRule.ZonesAsString = tempIncludedZoneNames ? tempIncludedZoneNames.join(", ") : undefined;
                }
            }
        }
    }

    app.directive('vrWhsSalesPricingtemplateruleManagement', PricingTemplateRuleManagementDirective);

})(app);