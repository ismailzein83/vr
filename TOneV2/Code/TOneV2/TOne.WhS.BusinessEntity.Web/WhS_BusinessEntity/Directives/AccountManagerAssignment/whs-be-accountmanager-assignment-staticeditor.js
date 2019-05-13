(function (app) {

    'use strict';

    WHSBEAccountManagerAssignmentStaticEditor.$inject = ['UtilsService', 'VRUIUtilsService', 'VRValidationService','WhS_BE_CarrierAccountTypeEnum'];

    function WHSBEAccountManagerAssignmentStaticEditor(UtilsService, VRUIUtilsService, VRValidationService, WhS_BE_CarrierAccountTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new WHSBEAccountManagerAssignmentStaticEditorCtol($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/AccountManagerAssignment/Templates/AccountManagerAssignmentStaticEditorTemplate.html"
        };

        function WHSBEAccountManagerAssignmentStaticEditorCtol($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountManagerAssignementEntity;
            var previousCarrierAccount;

            var carrierAccountSelectorAPI;
            var carrierAccountSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountSelectorAPI = api;
                    carrierAccountSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.validateBED = function () {
                    return VRValidationService.validateTimeRange($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
                };

                $scope.scopeModel.validateEED = function () {
                    return VRValidationService.validateTimeRange($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        accountManagerAssignementEntity = payload.selectedValues;
                        
                        if (!angular.equals({}, accountManagerAssignementEntity) && accountManagerAssignementEntity != undefined) {
                            $scope.scopeModel.beginEffectiveDate = accountManagerAssignementEntity.BED;
                            $scope.scopeModel.endEffectiveDate = accountManagerAssignementEntity.EED;
                        }
                    }

                    initialPromises.push(loadCarrierAccountSelector());

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.setData = function (obj) {
                    obj.CarrierAccountId = carrierAccountSelectorAPI.getSelectedIds();
                    obj.BED = $scope.scopeModel.beginEffectiveDate;
                    obj.EED = $scope.scopeModel.endEffectiveDate;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function loadCarrierAccountSelector() {
                var carrierAccountLoadPromisDeferred = UtilsService.createPromiseDeferred();
                carrierAccountSelectorReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            Filters: getCarrierAccountSelectorFilter()
                        },
                        selectedIds: accountManagerAssignementEntity != undefined ? accountManagerAssignementEntity.CarrierAccountId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, payload, carrierAccountLoadPromisDeferred);
                });
                return carrierAccountLoadPromisDeferred.promise;
            }

            function getCarrierAccountSelectorFilter() {
                var carrierAccountSelectorFilters = [];

                var carrierAccountFilterAffected = {
                    $type: 'TOne.WhS.BusinessEntity.Business.AffectedCarrierAccountFilterInAccountManagerAssignment, TOne.WhS.BusinessEntity.Business',
                    AccountManagerAssignmentId: (accountManagerAssignementEntity != undefined) ? accountManagerAssignementEntity.ID : undefined
                };
                carrierAccountSelectorFilters.push(carrierAccountFilterAffected);

                return carrierAccountSelectorFilters;
            }
        }
    }

    app.directive('whsBeAccountmanagerAssignmentStaticeditor', WHSBEAccountManagerAssignmentStaticEditor);

})(app);