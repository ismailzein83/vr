'use strict';
app.directive('vrWhsBePurchasepricingrulecriteria', ['UtilsService', '$compile', 'WhS_BE_PricingRuleAPIService','WhS_BE_CarrierAccountAPIService','VRUIUtilsService',
function (UtilsService, $compile, WhS_BE_PricingRuleAPIService, WhS_BE_CarrierAccountAPIService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var bePurchasePricingRuleCriteriaObject = new bePurchasePricingRuleCriteria(ctrl, $scope, $attrs);
            bePurchasePricingRuleCriteriaObject.initializeController();

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
            $scope.purchasePricingRuleCriteriaTemplates = [];
            $scope.onSuppliersWithZonesGroupsDirectiveReady = function (api) {
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
                var obj={
                    $type: "TOne.WhS.BusinessEntity.MainExtensions.SuppliersWithZonesGroups.SelectiveSuppliersWithZonesGroup,TOne.WhS.BusinessEntity.MainExtensions",
                    SuppliersWithZones: suppliersWithZonesGroupsDirectiveAPI.getData(),
                    ConfigId : $scope.selectedSuppliersWithZonesStettingsTemplate.TemplateConfigID,
                }
                var suppliersWithZonesGroupSettings = {
                    SuppliersWithZonesGroupSettings: obj
                    }
                return suppliersWithZonesGroupSettings;
            }
            api.setData = function (settings) {

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

                setDirectivesDataOperations.push(setSuppliersWithZinesStettingsDirective);
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
            function setSuppliersWithZinesStettingsDirective() {
                return suppliersWithZonesGroupsDirectiveAPI.setData(directiveAppendixData.SuppliersWithZones);
            }
        }
        

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);