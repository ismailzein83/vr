"use strict";

app.directive("cpSupplierpricelistUploadsupplierpricelist", ['UtilsService', 'VRUIUtilsService', 'CP_SupplierPricelist_SupplierPriceListAPIService', function (UtilsService, VRUIUtilsService, CP_SupplierPricelist_SupplierPriceListAPIService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: "/Client/Modules/CP_SupplierPricelist/Directives/MainExtensions/Templates/UploadPriceListTemplate.html"
    };


    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        var sourceTypeDirectiveAPI;


        function initializeController() {
            $scope.sourceTypeTemplates = [];
            $scope.onSourceTypeDirectiveReady = function (api) {
                sourceTypeDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingSourceTypeDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceTypeDirectiveAPI, undefined, setLoader, sourceDirectiveReadyPromiseDeferred);
            }
            defineAPI();
        }

        function defineAPI() {

            var api = {};
            api.getData = function () {
                var supplierPriceListConnectorObj = sourceTypeDirectiveAPI.getData();
                supplierPriceListConnectorObj.ConfigId = $scope.selectedSourceTypeTemplate.TemplateConfigID;
                return {
                    $type: "CP.SupplierPricelist.Business.UploadPriceListTaskActionArgument, CP.SupplierPricelist.Business",
                    SupplierPriceListConnectorBase: supplierPriceListConnectorObj,
                    Url: $scope.url,
                    Username: $scope.username,
                    Password: $scope.password
                };
            };


            api.load = function (payload) {

                if (payload != undefined && payload.data != undefined) {
                    $scope.url = payload.data.Url;
                    $scope.username = payload.data.Username;
                    $scope.password = payload.data.Password;
                }

                return CP_SupplierPricelist_SupplierPriceListAPIService.GetUploadPriceListTemplates().then(function (response) {
                    if (payload != undefined && payload.data != undefined && payload.data.SupplierPriceListConnectorBase != undefined)
                        var sourceConfigId = payload.data.SupplierPriceListConnectorBase.ConfigId;
                    angular.forEach(response, function (item) {
                        $scope.sourceTypeTemplates.push(item);
                    });

                    if (sourceConfigId != undefined)
                        $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "TemplateConfigID");
                });
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
