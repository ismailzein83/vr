'use strict';
app.directive('vrWhsBeSelectivesuppliers', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var beSuppliersCtor = new beSuppliers(ctrl, $scope);
                beSuppliersCtor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return getBeSelectiveSuppliersTemplate(attrs);
            }

        };

        function getBeSelectiveSuppliersTemplate(attrs) {
            return '/Client/Modules/WhS_BusinessEntity/Directives/Templates/SelectiveSuppliersDirectiveTemplate.html';
        }

        function beSuppliers(ctrl, $scope) {
            var carrierAccountDirectiveAPI;

            function initializeController() {
                $scope.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveAPI = api;
                    carrierAccountDirectiveAPI.load().then(function () {
                        defineAPI();
                    });
                }
                
            }

            function defineAPI() {
                var api = {};

                api.load = function () {

                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.SelectiveSuppliersSettings, TOne.WhS.BusinessEntity.Entities",
                        SupplierIds: UtilsService.getPropValuesFromArray(carrierAccountDirectiveAPI.getData(), "CarrierAccountId")
                    };
                }

                api.setData = function (supplierGroupSettings) {
                    carrierAccountDirectiveAPI.setData(supplierGroupSettings.SupplierIds);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);