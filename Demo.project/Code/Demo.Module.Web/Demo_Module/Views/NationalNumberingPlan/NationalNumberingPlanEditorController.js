(function (appControllers) {

    "use strict";

    nationalNumberingPlanEditorController.$inject = ['$scope', 'Demo_NationalNumberingPlanAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VRValidationService'];

    function nationalNumberingPlanEditorController($scope, Demo_NationalNumberingPlanAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VRValidationService) {
        var isEditMode;
        var nationalNumberingPlanId;
        var nationalNumberingPlanEntity;

        var operatorProfileDirectiveAPI;
        var operatorProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                nationalNumberingPlanId = parameters.NationalNumberingPlanId;
            }
            isEditMode = (nationalNumberingPlanId != undefined);

        }

        function defineScope() {

            $scope.scopeModal = {
                range: []
            };

            $scope.validateDateRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            }

            $scope.scopeModal.onOperatorProfileDirectiveReady = function (api) {
                operatorProfileDirectiveAPI = api;
                operatorProfileReadyPromiseDeferred.resolve();

            }
            $scope.scopeModal.disabledrange = true;
            $scope.scopeModal.onRangeValueChange = function (value) {
                $scope.scopeModal.disabledrange = (contains($scope.scopeModal.range, value) || value == undefined);
            }


            $scope.SaveNationalNumberingPlan = function () {
                if (isEditMode) {
                    return updateNationalNumberingPlan();
                }
                else {
                    return insertNationalNumberingPlan();
                }
            };
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.addRangeOption = function () {
                var range = $scope.scopeModal.rangevalue;
                $scope.scopeModal.range.push({
                    range: range
                });
                $scope.scopeModal.rangevalue = undefined;
                $scope.scopeModal.disabledrange = true;
            };

        }


        function contains(a, obj) {
            for (var i = 0; i < a.length; i++) {
                if (a[i].range === obj) {
                    return true;
                }
            }
            return false;
        }


        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getNationalNumberingPlan().then(function () {
                    loadAllControls()
                        .finally(function () {
                            nationalNumberingPlanEntity = undefined;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getNationalNumberingPlan() {
            return Demo_NationalNumberingPlanAPIService.GetNationalNumberingPlan(nationalNumberingPlanId).then(function (nationalNumberingPlan) {
                nationalNumberingPlanEntity = nationalNumberingPlan;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticSection, loadOperatorProfileDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(nationalNumberingPlanEntity ? '' : null, 'National Numbering Plan') : UtilsService.buildTitleForAddEditor('National Numbering Plan');
        }

        function loadOperatorProfileDirective() {

            var loadOperatorProfilePromiseDeferred = UtilsService.createPromiseDeferred();

            operatorProfileReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: (nationalNumberingPlanEntity != undefined ? nationalNumberingPlanEntity.OperatorId : (nationalNumberingPlanId != undefined ? nationalNumberingPlanId : undefined))
                    }
                    VRUIUtilsService.callDirectiveLoad(operatorProfileDirectiveAPI, directivePayload, loadOperatorProfilePromiseDeferred);
                });

            return loadOperatorProfilePromiseDeferred.promise;
        }

        function loadStaticSection() {
            if (nationalNumberingPlanEntity != undefined) {
                $scope.scopeModal.fromDate = nationalNumberingPlanEntity.FromDate;
                $scope.scopeModal.toDate = nationalNumberingPlanEntity.ToDate;

                if (nationalNumberingPlanEntity.Settings != null) {
                    $scope.scopeModal.range = [];
                    if (nationalNumberingPlanEntity.Settings.Range == undefined)
                        nationalNumberingPlanEntity.Settings.Range = [];
                    for (var j = 0; j < nationalNumberingPlanEntity.Settings.Range.length; j++) {
                        $scope.scopeModal.range.push({
                            range: nationalNumberingPlanEntity.Settings.Range[j]
                        });
                    }
                }
            }
        }

        function buildNationalNumberingPlanObjFromScope() {

            var obj = {
                NationalNumberingPlanId: (nationalNumberingPlanId != null) ? nationalNumberingPlanId : 0,
                FromDate: $scope.scopeModal.fromDate,
                ToDate: $scope.scopeModal.toDate,
                OperatorId: operatorProfileDirectiveAPI.getSelectedIds(),
                Settings: {
                    Range: UtilsService.getPropValuesFromArray($scope.scopeModal.range, "range")
                }
            };

            return obj;
        }

        function insertNationalNumberingPlan() {
            $scope.isLoading = true;

            var nationalNumberingPlanObject = buildNationalNumberingPlanObjFromScope();

            return Demo_NationalNumberingPlanAPIService.AddNationalNumberingPlan(nationalNumberingPlanObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("National Numbering Plan", response, undefined)) {
                    if ($scope.onNationalNumberingPlanAdded != undefined)
                        $scope.onNationalNumberingPlanAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateNationalNumberingPlan() {
            $scope.isLoading = true;

            var nationalNumberingPlanObject = buildNationalNumberingPlanObjFromScope();

            Demo_NationalNumberingPlanAPIService.UpdateNationalNumberingPlan(nationalNumberingPlanObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("National Numbering Plan", response, undefined)) {
                    if ($scope.onNationalNumberingPlanUpdated != undefined)
                        $scope.onNationalNumberingPlanUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('Demo_NationalNumberingPlanEditorController', nationalNumberingPlanEditorController);
})(appControllers);
