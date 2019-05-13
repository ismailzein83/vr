(function (app) {

    'use strict';

    WHSBEAccountManagerStaticEditor.$inject = ['UtilsService', 'VRUIUtilsService', 'VRValidationService', 'WhS_BE_CarrierAccountTypeEnum'];

    function WHSBEAccountManagerStaticEditor(UtilsService, VRUIUtilsService, VRValidationService, WhS_BE_CarrierAccountTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new WHSBEAccountManagerStaticEditorCtol($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/AccountManager/Templates/AccountManagerStaticEditorTemplate.html"
        };

        function WHSBEAccountManagerStaticEditorCtol($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountManagerEntity;
            var selectedParentId;

            var userSelectorAPI;
            var userSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var parentUserSelectorAPI;
            var parentUserSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isParentSelectorDisabled = false;

                $scope.scopeModel.onUserSelectorReady = function (api) {
                    userSelectorAPI = api;
                    userSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onAccountManagerGBESelectorReady = function (api) {
                    parentUserSelectorAPI = api;
                    parentUserSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];
                    var parentFieldValues;

                    if (payload != undefined) {
                        accountManagerEntity = payload.selectedValues;
                        parentFieldValues = payload.parentFieldValues;
                    }

                    if (parentFieldValues != undefined) {
                        selectedParentId = parentFieldValues.ParentId.value;
                        $scope.scopeModel.isParentSelectorDisabled = true;
                    }
                    else if (accountManagerEntity != undefined) {
                        selectedParentId = accountManagerEntity.ParentId;
                    }

                    initialPromises.push(loadUserSelector());
                    initialPromises.push(loadParentUserSelector());

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
                    obj.UserId = userSelectorAPI.getSelectedIds();
                    obj.ParentId = parentUserSelectorAPI.getSelectedIds();
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function loadUserSelector() {
                var userLoadPromisDeferred = UtilsService.createPromiseDeferred();
                userSelectorReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            Filters: [{
                                $type: "TOne.WhS.BusinessEntity.Business.AffectedUsersFilterInAccountManager, TOne.WhS.BusinessEntity.Business",
                                UserId: (accountManagerEntity != undefined) ? accountManagerEntity.UserId : undefined
                            }]
                        },
                        selectedIds: accountManagerEntity != undefined ? accountManagerEntity.UserId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(userSelectorAPI, payload, userLoadPromisDeferred);
                });
                return userLoadPromisDeferred.promise;
            }

            function loadParentUserSelector() {
                var parentUserLoadPromisDeferred = UtilsService.createPromiseDeferred();
                parentUserSelectorReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        businessEntityDefinitionId: "0146109f-4e5d-4d66-be2f-15d689c960ee",
                        selectedIds: selectedParentId
                    };
                    VRUIUtilsService.callDirectiveLoad(parentUserSelectorAPI, payload, parentUserLoadPromisDeferred);
                });
                return parentUserLoadPromisDeferred.promise;
            }
        }
    }

    app.directive('whsBeAccountmanagerStaticeditor', WHSBEAccountManagerStaticEditor);

})(app);