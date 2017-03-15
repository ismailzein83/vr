(function (appControllers) {

    'use strict';

    PriceListTemplateEditorController.$inject = ['$scope', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function PriceListTemplateEditorController($scope, VRNotificationService, UtilsService, VRUIUtilsService) {
        var outputTemplateSelectorAPI;
        var outputPriceListTemplateReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                    return saveOutputTemplate();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onOutputPricelistTemplateSelectorReady = function (api) {
                outputTemplateSelectorAPI = api;
                outputPriceListTemplateReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadOutputPricelistTemplateSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function loadOutputPricelistTemplateSelector() {
            var loadOutputPricelistTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
            outputPriceListTemplateReadyPromiseDeferred.promise.then(function () {
                var payload;
                VRUIUtilsService.callDirectiveLoad(outputTemplateSelectorAPI, payload, loadOutputPricelistTemplatePromiseDeferred);
            });

            return loadOutputPricelistTemplatePromiseDeferred.promise;
        }

        function setTitle() {
                $scope.title ='PriceList Template';
        }

        function buildOutputPriceListTemplateObjFromScope() {
            if (outputTemplateSelectorAPI != undefined)
            {
                var obj =
               {
                   pricelistTemplateIds: outputTemplateSelectorAPI.getSelectedIds()
               };
                return obj;
            }
           
        }

        function saveOutputTemplate() {
            $scope.scopeModel.isLoading = true;

            var priceListTemplateObject = buildOutputPriceListTemplateObjFromScope();
            if ($scope.onOutputPriceListTemplateChoosen != undefined)
                $scope.onOutputPriceListTemplateChoosen(priceListTemplateObject);
            $scope.modalContext.closeModal();

        }
    }

    appControllers.controller('XBooster_PriceListConversion_OutputPriceListTemplateEditorController', PriceListTemplateEditorController);

})(appControllers);
