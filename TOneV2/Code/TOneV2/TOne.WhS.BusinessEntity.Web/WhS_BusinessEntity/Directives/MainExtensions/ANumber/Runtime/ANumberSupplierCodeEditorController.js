(function (appControllers) {

    "use strict";

    aNumberSupplierCodeEditorController.$inject = ["$scope", "WhS_BE_ANumberSupplierCodeAPIService", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService"];

    function aNumberSupplierCodeEditorController($scope, WhS_BE_ANumberSupplierCodeAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var aNumberGroupId;
        var fileId;
        var supplierSelectorAPI;
        var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.saveSupplierCode = function () {
                var input = buildSupplierCodesInputObject();

                return WhS_BE_ANumberSupplierCodeAPIService.AddANumberSupplierCodes(input).then(function (response) {
                    if (response) {
                        $scope.scopeModel.isUploadingComplete = true;
                        $scope.scopeModel.addedSupplierCodes = response.CountOfSupplierCodesAdded;
                        $scope.scopeModel.failedSupplierCodes = response.CountOfSupplierCodesFailed;
                        fileId = response.FileID;
                    }
                });

            };

            $scope.scopeModel.downloadLog = function () {
                if (fileId != undefined) {
                    return WhS_BE_ANumberSupplierCodeAPIService.DownloadSupplierCodesLog(fileId).then(function (response) {
                        UtilsService.downloadFile(response.data, response.headers);
                    });
                }
            };
            $scope.scopeModel.supplierCodes = [];

            $scope.scopeModel.onSupplierReady = function (api) {
                supplierSelectorAPI = api;
                supplierReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.disabledAddSupplierCode = true;

            $scope.scopeModel.addSupplierCode = function () {
                $scope.scopeModel.supplierCodes.push($scope.scopeModel.codeValue);
                $scope.scopeModel.codeValue = undefined;
                $scope.scopeModel.disabledAddSupplierCode = true;
            };

            $scope.scopeModel.onCodeValueChange = function (value) {
                $scope.scopeModel.disabledAddSupplierCode = (value == undefined && $scope.scopeModel.codeValue.length - 1 < 1) || $scope.scopeModel.supplierCodes.indexOf($scope.scopeModel.codeValue) != -1;
            };
            $scope.scopeModel.validateSupplierCodes = function () {
                if ($scope.scopeModel.supplierCodes.length == 0)
                    return "Enter at least one code.";
                return null;
            };
            $scope.scopeModel.onFileUploadChange = function (obj) {
                if (obj != undefined) {
                    $scope.scopeModel.isLoadingCodes = true;
                    WhS_BE_ANumberSupplierCodeAPIService.GetUploadedSupplierCodes(obj.fileId).then(function (response) {
                        if (response) {
                            for (var i = 0; i < response.length ; i++) {
                                if ($scope.scopeModel.supplierCodes.indexOf(response[i]) == -1)
                                    $scope.scopeModel.supplierCodes.push(response[i]);
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
            $scope.modalContext.onModalHide = function () {
                if ($scope.onANumberSupplierCodesAdded != undefined)
                    $scope.onANumberSupplierCodesAdded();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSupplierSelector, setTitle]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadSupplierSelector() {
            var supplierLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            supplierReadyPromiseDeferred.promise.then(function () {
                var directivePayload ;
                VRUIUtilsService.callDirectiveLoad(supplierSelectorAPI, directivePayload, supplierLoadPromiseDeferred);
            });
            return supplierLoadPromiseDeferred.promise;
        }
        function setTitle() {
            $scope.title = "Add ANumber Supplier Codes";
        }

        function getSupplierCodesArray() {
            var codes = [];
            for (var i = 0; i < $scope.scopeModel.supplierCodes.length ; i++) {
                codes.push($scope.scopeModel.supplierCodes[i]);
            }
            return codes;
        }
        function buildSupplierCodesInputObject() {
            return {
                ANumberGroupId: aNumberGroupId,
                SupplierId: supplierSelectorAPI.getSelectedIds(),
                Codes: getSupplierCodesArray(),
                EffectiveOn: $scope.scopeModel.effectiveOn
            }
        }

    }

    appControllers.controller("WhS_BE_ANumberSupplierCodeEditorController", aNumberSupplierCodeEditorController);
})(appControllers);
