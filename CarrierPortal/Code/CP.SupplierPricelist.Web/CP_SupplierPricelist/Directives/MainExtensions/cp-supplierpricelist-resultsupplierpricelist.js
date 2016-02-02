"use strict";

app.directive("cpSupplierpricelistResultsupplierpricelist", ['UtilsService', 'VRUIUtilsService', 'CP_SupplierPricelist_SupplierPriceListAPIService', function (UtilsService, VRUIUtilsService, CP_SupplierPricelist_SupplierPriceListAPIService) {

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
        templateUrl: "/Client/Modules/CP_SupplierPricelist/Directives/MainExtensions/Templates/ResultPriceListTemplate.html"
    };


    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        var sourceTypeDirectiveAPI;
        var sourceDirectiveReadyPromiseDeferred;

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
                return {
                    $type: "CP.SupplierPricelist.Business.PriceListTasks.ResultTaskActionArgument, CP.SupplierPricelist.Business",
                    SupplierPriceListConnector: sourceTypeDirectiveAPI.getData()
                };
            };


            api.load = function (payload) {
                var promises = [];
                var pricelistTemplatesLoad = CP_SupplierPricelist_SupplierPriceListAPIService.GetUploadPriceListTemplates().then(function (response) {
                    if (payload != undefined && payload.data != undefined && payload.data.SupplierPriceListConnector != undefined)
                        var sourceConfigId = payload.data.SupplierPriceListConnector.ConfigId;
                    angular.forEach(response, function (item) {
                        $scope.sourceTypeTemplates.push(item);
                    });

                    if (sourceConfigId != undefined)
                        $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "TemplateConfigID");
                });

                promises.push(pricelistTemplatesLoad);

                if (sourceTypeDirectiveAPI) {
                    sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    var loadSourceTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
                    sourceDirectiveReadyPromiseDeferred.promise.then(function () {
                        sourceDirectiveReadyPromiseDeferred = undefined;
                        var obj;
                        //if (payload != undefined && payload.data != undefined && payload.data.SourceZoneReader != undefined)
                        //    obj = {
                        //        connectionString: payload.data.SourceZoneReader.ConnectionString
                        //    };
                        VRUIUtilsService.callDirectiveLoad(sourceTypeDirectiveAPI, obj, loadSourceTemplatePromiseDeferred);
                    });
                    promises.push(loadSourceTemplatePromiseDeferred.promise);
                }


                return UtilsService.waitMultiplePromises(promises);
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
