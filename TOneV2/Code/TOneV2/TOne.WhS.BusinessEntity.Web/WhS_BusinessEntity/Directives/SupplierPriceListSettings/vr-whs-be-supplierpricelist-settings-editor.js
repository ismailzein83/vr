'use strict';

app.directive('vrWhsBeSupplierpricelistSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SupplierPriceListSettings/Templates/SupplierPriceListSettingsTemplate.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {
           
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined)
                    {
                        ctrl.retroActiveMinDateOffset = payload.data.RetroActiveMinDate;
                    }
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.SupplierPriceListSettingsData, TOne.WhS.BusinessEntity.Entities",
                        RetroActiveMinDate: ctrl.retroActiveMinDateOffset
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);