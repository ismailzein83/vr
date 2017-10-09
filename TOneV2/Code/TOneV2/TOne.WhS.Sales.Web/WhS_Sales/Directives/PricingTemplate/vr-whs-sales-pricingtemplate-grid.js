'use strict';

app.directive('vrWhsSalesPricingtemplateGrid', ['VRNotificationService', 'VRUIUtilsService', 'WhS_Sales_PricingTemplateAPIService', 'WhS_Sales_PricingTemplateService',
    function (VRNotificationService, VRUIUtilsService, WhS_Sales_PricingTemplateAPIService, WhS_Sales_PricingTemplateService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new PricingTemplateGridCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Sales/Directives/PricingTemplate/Templates/PricingTemplateGridTemplate.html'
        };

        function PricingTemplateGridCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.pricingTemplate = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_Sales_PricingTemplateAPIService.GetFilteredPricingTemplates(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onPricingTemplateAdded = function (addedPricingTemplate) {
                    gridAPI.itemAdded(addedPricingTemplate);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editPricingTemplate,
                    haspermission: hasEditPricingTemplatePermission
                });
            }
            function editPricingTemplate(pricingTemplateItem) {
                var onPricingTemplateUpdated = function (updatedPricingTemplate) {
                    gridAPI.itemUpdated(updatedPricingTemplate);
                };

                WhS_Sales_PricingTemplateService.editPricingTemplate(pricingTemplateItem.Entity.PricingTemplateId, onPricingTemplateUpdated);
            }
            function hasEditPricingTemplatePermission() {
                return WhS_Sales_PricingTemplateAPIService.HasUpdatePricingTemplatePermission();
            }
        }
    }]);
