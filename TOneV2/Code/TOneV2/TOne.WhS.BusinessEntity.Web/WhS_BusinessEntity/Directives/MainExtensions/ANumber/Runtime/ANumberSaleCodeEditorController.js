(function (appControllers) {

    "use strict";

    aNumberSaleCodeEditorController.$inject = ["$scope", "WhS_BE_ANumberSaleCodeAPIService", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService"];

    function aNumberSaleCodeEditorController($scope, WhS_BE_ANumberSaleCodeAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var aNumberGroupId;
        var fileId;
        var sellingNumberPlanSelectorAPI;
        var sellingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                aNumberGroupId = parameters.aNumberGroupId;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.saleCodes = [];

            $scope.scopeModel.saveSaleCode = function () {
                var input = buildSaleCodesInputObject();

                return WhS_BE_ANumberSaleCodeAPIService.AddANumberSaleCodes(input).then(function (response) {
                    if (response) {
                        if (response.InvalidImportedSaleCodes != null && response.InvalidImportedSaleCodes.length > 0) {
                            VRNotificationService.showWarning(response.ResultMessage);
                            for (var i = 0; i < response.InvalidImportedSaleCodes.length; i++) {
                                var index = UtilsService.getItemIndexByVal($scope.scopeModel.saleCodes, response.InvalidImportedSaleCodes[i].Code, "code");
                                if (index > -1) {
                                    $scope.scopeModel.saleCodes[index].message = response.InvalidImportedSaleCodes[i].ErrorMessage;
                                }
                            }
                        }
                        else {
                            if ($scope.onANumberSaleCodesAdded != undefined)
                                $scope.onANumberSaleCodesAdded();
                            VRNotificationService.showSuccess(response.ResultMessage);
                            $scope.modalContext.closeModal();
                        }
                    }
                });

            };

            $scope.scopeModel.onSellingNumberReady = function (api) {
                sellingNumberPlanSelectorAPI = api;
                sellingReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.disabledAddSaleCode = true;

            $scope.scopeModel.addSaleCode = function () {
                $scope.scopeModel.saleCodes.push({ code: $scope.scopeModel.codeValue, message: null });
                $scope.scopeModel.codeValue = undefined;
                $scope.scopeModel.disabledAddSaleCode = true;
            };

            $scope.scopeModel.onCodeValueChange = function (value) {
                $scope.scopeModel.disabledAddSaleCode = (value == undefined && $scope.scopeModel.codeValue.length - 1 < 1) || UtilsService.getItemIndexByVal($scope.scopeModel.saleCodes, $scope.scopeModel.codeValue, "code") != -1;
            };
            $scope.scopeModel.validateSaleCodes = function () {
                if ($scope.scopeModel.saleCodes.length == 0)
                    return "Enter at least one code.";
                if (ValidateSaleCode() == false)
                    return "Invalid sale codes inputs.";
                return null;
            };
            $scope.scopeModel.onFileUploadChange = function (obj) {
                if (obj != undefined) {
                    $scope.scopeModel.isLoadingCodes = true;
                    WhS_BE_ANumberSaleCodeAPIService.GetUploadedSaleCodes(obj.fileId).then(function (response) {
                        if (response) {
                            for (var i = 0; i < response.length ; i++) {
                                if (UtilsService.getItemIndexByVal($scope.scopeModel.saleCodes, response[i], "code") == -1)
                                    $scope.scopeModel.saleCodes.push({ code: response[i], message: null });
                            }
                        }
                    }).finally(function () {
                        $scope.scopeModel.isLoadingCodes = false;
                    });
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlanSelector, setTitle]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadSellingNumberPlanSelector() {
            var sellingLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            sellingReadyPromiseDeferred.promise.then(function () {
                var directivePayload = { selectifsingleitem: true };
                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanSelectorAPI, directivePayload, sellingLoadPromiseDeferred);
            });
            return sellingLoadPromiseDeferred.promise;
        }
        function setTitle() {
            $scope.title = "Add ANumber Sale Codes";
        }

        function getSaleCodesArray() {
            var codes = [];
            for (var i = 0; i < $scope.scopeModel.saleCodes.length ; i++) {
                codes.push($scope.scopeModel.saleCodes[i].code);
            }
            return codes;
        }

        function ValidateSaleCode() {
            for (var i = 0; i < $scope.scopeModel.saleCodes.length ; i++) {
                if ($scope.scopeModel.saleCodes[i].message != null)
                    return false;
            }
            return true;
        }
        function buildSaleCodesInputObject() {
            return {
                ANumberGroupId: aNumberGroupId,
                SellingNumberPlanId: sellingNumberPlanSelectorAPI.getSelectedIds(),
                Codes: getSaleCodesArray(),
                EffectiveOn: $scope.scopeModel.effectiveOn
            }
        }

    }

    appControllers.controller("WhS_BE_ANumberSaleCodeEditorController", aNumberSaleCodeEditorController);
})(appControllers);
