"use strict";

app.directive("whsSplSupplierpricelistPreviewdata", ["WhS_SupPL_PreviewChangeTypeEnum", "WhS_SupPL_PreviewGroupedBy", "UtilsService", "VRUIUtilsService", "VRNotificationService",
function (WhS_SupPL_PreviewChangeTypeEnum, WhS_SupPL_PreviewGroupedBy, UtilsService, VRUIUtilsService, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var supplierPriceListPreviewData = new SupplierPriceListPreviewData($scope, ctrl, $attrs);
            supplierPriceListPreviewData.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
        },
        templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/SupplierPriceListPreviewSection/Templates/SupplierPricelistPreviewDataTemplate.html"
    };

    function SupplierPriceListPreviewData($scope, ctrl, $attrs) {

        var processInstanceId;
        var taskData;
        var requireWarningConfirmation;
        var pricelistDate;
        var currencySymbol;
        var supplierPricelistType;
        var fileId;
        var currencyId;
        var changeType = true;
        var isExcluded;

        var directiveWrapperAPI;
        var directiveWrapperReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var viewChangeTypeSelectorAPI;
        var viewChangeTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var groupedBySelectorAPI;
        var groupedByReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            $scope.onViewChangeTypeSelectorReady = function (api) {
                viewChangeTypeSelectorAPI = api;
                viewChangeTypeReadyPromiseDeferred.resolve();
            };

            $scope.onGroupedBySelectorReady = function (api) {
                groupedBySelectorAPI = api;
                groupedByReadyPromiseDeferred.resolve();
            };

            $scope.onDirectiveWrapperReady = function (api) {
                directiveWrapperAPI = api;

                var setLoader = function (value) {
                    $scope.isLoadingPreviewDataSection = value;
                };

                var previewDataPayload = {
                    ProcessInstanceId: processInstanceId,

                    OnlyModified: viewChangeTypeSelectorAPI.getSelectedIds(),
                    IsExcluded: isExcluded != undefined ? isExcluded : false
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
                    isExcluded = payload.isExcluded;
                    pricelistDate = payload.pricelistDate;
                    currencyId = payload.currencyId;
                    supplierPricelistType = payload.supplierPricelistType;
                    fileId = payload.fileId;
                    requireWarningConfirmation = payload.requireWarningConfirmation;
                }
                return UtilsService.waitMultipleAsyncOperations([loadViewChangeTypeSelector, loadGroupedBySelector, loadPreviewDataSection])
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
                    OnlyModified: changeType,
                    IsExcluded: isExcluded != undefined ? isExcluded:false
                };

                VRUIUtilsService.callDirectiveLoad(directiveWrapperAPI, payload, loadPreviewDataPromiseDeferred);
            });
            return loadPreviewDataPromiseDeferred.promise;
        }


    }

    return directiveDefinitionObject;
}]);
