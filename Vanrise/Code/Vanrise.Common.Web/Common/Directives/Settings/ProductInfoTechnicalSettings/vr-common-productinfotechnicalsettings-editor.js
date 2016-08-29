'use strict';

app.directive('vrCommonProductinfotechnicalsettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var productInfoTechnicalSettingsEditor = new ProductInfoTechnicalSettingsEditor(ctrl, $scope, $attrs);
                productInfoTechnicalSettingsEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/Settings/ProductInfoTechnicalSettings/Templates/ProductInfoTechnicalSettingsTemplate.html"
        };

        function ProductInfoTechnicalSettingsEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            $scope.scopeModel = {};


            function initializeController() {
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    console.log(payload);

                    var productInfoTechnicalSettingsPayload;
                    if (payload != undefined && payload.data != undefined) {
                        productInfoTechnicalSettingsPayload = payload.data;
                    }

                    $scope.scopeModel.productName = productInfoTechnicalSettingsPayload.ProductName;
                    $scope.scopeModel.versionNumber = productInfoTechnicalSettingsPayload.VersionNumber;
                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.Entities.ProductInfoTechnicalSettings, Vanrise.Entities",
                        ProductName: $scope.scopeModel.productName,
                        VersionNumber: $scope.scopeModel.versionNumber
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);