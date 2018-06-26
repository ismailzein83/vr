"use strict";

app.directive("whsSplSupplierpricelistpreviewSection", ["WhS_SupPL_PreviewChangeTypeEnum", "WhS_SupPL_PreviewGroupedBy", "UtilsService", "VRUIUtilsService", "VRNotificationService",
function (WhS_SupPL_PreviewChangeTypeEnum, WhS_SupPL_PreviewGroupedBy, UtilsService, VRUIUtilsService, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var supplierPriceListPreviewSection = new SupplierPriceListPreviewSection($scope, ctrl, $attrs);
            supplierPriceListPreviewSection.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
        },
        templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/SupplierPriceListPreviewSection/Templates/SupplierPriceListPreviewSectionTemplate.html"
    };

    function SupplierPriceListPreviewSection($scope, ctrl, $attrs) {

        var processInstanceId;
        var taskData;
        var requireWarningConfirmation;
        var pricelistDate;
        var currencySymbol;
        var supplierPricelistType;
        var fileId;
        var currencyId;
        var changeType = true;

        var validationMessageHistoryGridAPI;
        var validationMessageHistoryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var directiveWrapperAPI;
        var directiveWrapperReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var viewChangeTypeSelectorAPI;
        var viewChangeTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var groupedBySelectorAPI;
        var groupedByReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierPricelistPreviewSummaryAPI;
        var supplierPricelistPreviewSummaryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        this.initializeController = initializeController;

        function initializeController() {

            $scope.onViewChangeTypeSelectItem = function (dataItem) {
                if (dataItem != undefined) {
                    directiveWrapperReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    directiveWrapperReadyPromiseDeferred.resolve();
                    changeType = dataItem.value;
                    return loadPreviewDataSection();
                }
            };

            $scope.onValidationMessageHistoryGridReady = function (api) {
                validationMessageHistoryGridAPI = api;
                validationMessageHistoryReadyPromiseDeferred.resolve();
            };

            $scope.onViewChangeTypeSelectorReady = function (api) {
                viewChangeTypeSelectorAPI = api;
                viewChangeTypeReadyPromiseDeferred.resolve();
            };

            $scope.onGroupedBySelectorReady = function (api) {
                groupedBySelectorAPI = api;
                groupedByReadyPromiseDeferred.resolve();
            };

            $scope.onSupplierPricelistPreviewSummaryReady = function (api) {
                supplierPricelistPreviewSummaryAPI = api;
                supplierPricelistPreviewSummaryReadyPromiseDeferred.resolve();
            };

            $scope.onDirectiveWrapperReady = function (api) {
                directiveWrapperAPI = api;

                var setLoader = function (value) {
                    $scope.isLoadingPreviewDataSection = value;
                };

                var previewDataPayload = {
                    ProcessInstanceId: processInstanceId,

                    OnlyModified: viewChangeTypeSelectorAPI.getSelectedIds()
                };

                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveWrapperAPI, previewDataPayload, setLoader, directiveWrapperReadyPromiseDeferred);
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.load = function (payload) {

                if (payload != null) {
                    processInstanceId = payload.processInstanceId;
                    pricelistDate = payload.pricelistDate;
                    currencyId = payload.currencyId;
                    supplierPricelistType = payload.supplierPricelistType;
                    fileId = payload.fileId;
                    requireWarningConfirmation = payload.requireWarningConfirmation;
                }
                return UtilsService.waitMultipleAsyncOperations([loadViewChangeTypeSelector, loadGroupedBySelector, loadPreviewDataSection, loadValidationMessageHistory,loadSupplierPricelistPreviewSummary])
                          .catch(function (error) {
                              VRNotificationService.notifyException(error);
                          });
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadViewChangeTypeSelector() {
            var loadViewChangeTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            viewChangeTypeReadyPromiseDeferred.promise.then(function () {

                var viewChangeTypeSelectorPayload = {
                    selectedIds: WhS_SupPL_PreviewChangeTypeEnum.OnlyModifiedEntities.value
                };

                VRUIUtilsService.callDirectiveLoad(viewChangeTypeSelectorAPI, viewChangeTypeSelectorPayload, loadViewChangeTypeSelectorPromiseDeferred);
            });
            return loadViewChangeTypeSelectorPromiseDeferred.promise;
        }

        function loadGroupedBySelector() {
            var loadGroupedBySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            groupedByReadyPromiseDeferred.promise.then(function () {

                var viewChangeTypeSelectorPayload = {
                    selectedIds: WhS_SupPL_PreviewGroupedBy.Countries.description
                };

                VRUIUtilsService.callDirectiveLoad(groupedBySelectorAPI, viewChangeTypeSelectorPayload, loadGroupedBySelectorPromiseDeferred);
            });
            return loadGroupedBySelectorPromiseDeferred.promise;
        }

        function loadPreviewDataSection() {
            var loadPreviewDataPromiseDeferred = UtilsService.createPromiseDeferred();
            directiveWrapperReadyPromiseDeferred.promise.then(function () {

                directiveWrapperReadyPromiseDeferred = undefined;

                var payload = {
                    ProcessInstanceId: processInstanceId,
                    OnlyModified: changeType
                };

                VRUIUtilsService.callDirectiveLoad(directiveWrapperAPI, payload, loadPreviewDataPromiseDeferred);
            });
            return loadPreviewDataPromiseDeferred.promise;
        }


        function loadSupplierPricelistPreviewSummary() {
            var loadSupplierPricelistPreviewSummaryPromiseDeferred = UtilsService.createPromiseDeferred();

            supplierPricelistPreviewSummaryReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    processInstanceId: processInstanceId,
                    pricelistDate: pricelistDate, 
                    fileID: fileId,
                    currencyId: currencyId, 
                    supplierPricelistType: supplierPricelistType,
                };
                VRUIUtilsService.callDirectiveLoad(supplierPricelistPreviewSummaryAPI, payload, loadSupplierPricelistPreviewSummaryPromiseDeferred);
            });

            return loadSupplierPricelistPreviewSummaryPromiseDeferred.promise;
        }

        function loadValidationMessageHistory() {
            var loadValidationMessageHistoryPromiseDeferred = UtilsService.createPromiseDeferred();

            validationMessageHistoryReadyPromiseDeferred.promise.then(function () {

                var payload = {
                    BPInstanceID: processInstanceId,
                    requireWarningConfirmation: requireWarningConfirmation,
                };

                VRUIUtilsService.callDirectiveLoad(validationMessageHistoryGridAPI, payload, loadValidationMessageHistoryPromiseDeferred);
            });

            return loadValidationMessageHistoryPromiseDeferred.promise;
        }

    }

    return directiveDefinitionObject;
}]);
