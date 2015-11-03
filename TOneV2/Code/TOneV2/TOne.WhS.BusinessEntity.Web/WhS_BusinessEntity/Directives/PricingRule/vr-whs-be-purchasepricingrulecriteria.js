'use strict';
app.directive('vrWhsBePurchasepricingrulecriteria', ['UtilsService', '$compile', 'WhS_BE_PricingRuleAPIService','WhS_BE_CarrierAccountAPIService','VRUIUtilsService','VRNotificationService',
function (UtilsService, $compile, WhS_BE_PricingRuleAPIService, WhS_BE_CarrierAccountAPIService, VRUIUtilsService, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new bePurchasePricingRuleCriteria(ctrl, $scope, $attrs);
            ctor.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PricingRule/Templates/PurchasePricingRuleCriteriaTemplate.html"

    };


    function bePurchasePricingRuleCriteria(ctrl, $scope, $attrs) {
        var suppliersWithZonesGroupsDirectiveAPI;
        var directiveAppendixData;
        function initializeController() {
            ctrl.purchasePricingRuleCriteriaTemplates = [];
            ctrl.onDirectiveReady = function (api) {

                suppliersWithZonesGroupsDirectiveAPI = api;
                if (directiveAppendixData != undefined) {
                    tryLoadAppendixDirectives();
                } else {
                    $scope.suppliersWithZonesAppendixLoader;
                    VRUIUtilsService.loadDirective($scope, suppliersWithZonesGroupsDirectiveAPI, $scope.suppliersWithZonesAppendixLoader);
                }
            }
            $scope.onSelectionChanged = function () {
                if (carrierAccountDirectiveAPI!=undefined)
                    $scope.suppliers = carrierAccountDirectiveAPI.getData();
            };
            declareDirectiveAsReady();
        }
        function declareDirectiveAsReady() {
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {};
                if ($scope.selectedSuppliersWithZonesStettingsTemplate!=undefined)
                {
                    obj = {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.SuppliersWithZonesGroups.SelectiveSuppliersWithZonesGroup,TOne.WhS.BusinessEntity.MainExtensions",
                        SuppliersWithZones: suppliersWithZonesGroupsDirectiveAPI.getData(),
                        ConfigId: $scope.selectedSuppliersWithZonesStettingsTemplate.TemplateConfigID,
                    }
                    var suppliersWithZonesGroupSettings = {
                        SuppliersWithZonesGroupSettings: obj
                    }
                    return suppliersWithZonesGroupSettings;
                }
               else
                    return obj;
            }
            api.setData = function (settings) {
                if (settings.SuppliersWithZonesGroupSettings != null) {
                    $scope.selectedSuppliersWithZonesStettingsTemplate = UtilsService.getItemByVal($scope.suppliersWithZonesStettingsTemplates, settings.SuppliersWithZonesGroupSettings.ConfigId, "TemplateConfigID")
                    directiveAppendixData = settings.SuppliersWithZonesGroupSettings
                    tryLoadAppendixDirectives();
                }
               
            }
            api.load = function () {
                return loadSuppliersWithZonesGroupsTemplates();

            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        function loadSuppliersWithZonesGroupsTemplates() {
            $scope.suppliersWithZonesStettingsTemplates = [];
            return WhS_BE_CarrierAccountAPIService.GetSuppliersWithZonesGroupsTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.suppliersWithZonesStettingsTemplates.push(item);
                });
            });
        }
        function tryLoadAppendixDirectives() {
            var loadOperations = [];
            var setDirectivesDataOperations = [];

            if ($scope.selectedSuppliersWithZonesStettingsTemplate != undefined) {
                if (suppliersWithZonesGroupsDirectiveAPI == undefined)
                    return;

                loadOperations.push(suppliersWithZonesGroupsDirectiveAPI.load);

                setDirectivesDataOperations.push(setSuppliersWithZonesStettingsDirective);
            }

            UtilsService.waitMultipleAsyncOperations(loadOperations).then(function () {

                setAppendixDirectives();

            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });

            function setAppendixDirectives() {
                UtilsService.waitMultipleAsyncOperations(setDirectivesDataOperations).then(function () {
                    directiveAppendixData = undefined;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.isGettingData = false;
                });
            }
            function setSuppliersWithZonesStettingsDirective() {
                return suppliersWithZonesGroupsDirectiveAPI.load(directiveAppendixData.SuppliersWithZones);
            }
        }
        

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);