'use strict';
app.directive('vrWhsBeSuppliergroupSelective', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new selectiveCtor(ctrl, $scope);
                ctor.initializeController();

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
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/SupplierGroup/Templates/SelectiveSuppliersDirectiveTemplate.html';
            }

        };

        function selectiveCtor(ctrl, $scope) {
            var carrierAccountDirectiveAPI;

            function initializeController() {

                $scope.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveAPI = api;
                    declareDirectiveAsReady();
                }
                
                declareDirectiveAsReady();
            }

            function declareDirectiveAsReady()
            {
                if (carrierAccountDirectiveAPI == undefined)
                    return;

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {
                    return carrierAccountDirectiveAPI.load();
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.SelectiveSuppliersSettings, TOne.WhS.BusinessEntity.Entities",
                        SupplierIds: carrierAccountDirectiveAPI.getSelectedIds()
                    };
                }

                api.setData = function (supplierGroupSettings) {
                    return carrierAccountDirectiveAPI.setData(supplierGroupSettings.SupplierIds);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);