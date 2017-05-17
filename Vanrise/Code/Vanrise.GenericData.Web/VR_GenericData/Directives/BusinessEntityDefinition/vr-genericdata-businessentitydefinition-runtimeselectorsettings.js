(function (app) {

    'use strict';

    BEDefinitionRuntimeSelectorSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_BusinessEntityDefinitionAPIService'];

    function BEDefinitionRuntimeSelectorSettingsDirective(UtilsService, VRUIUtilsService, VR_GenericData_BusinessEntityDefinitionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                customlabel: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BEDefinitionRuntimeSelectorSettingsCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrlRuntimeSelectorSettings',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/Templates/BEDefinitionRuntimeSelectorSettings.html';
            }
        };

        function BEDefinitionRuntimeSelectorSettingsCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var beDefinitionSelectorAPI;
            var beDefinitionSelectionChangedPromiseDeferred;

            var selectorFilterEditorAPI;
            var selectorFilterEditorReadyDeferred;
            var selectedSelectorFilterEditor;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBEDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorAPI = api;
                    defineAPI();
                };
                $scope.scopeModel.onSelectorFilterEditorDirectiveReady = function (api) {
                    selectedSelectorFilterEditor = $scope.scopeModel.selectedBusinessEntityDefinition.SelectorFilterEditor;
                    selectorFilterEditorAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingSelectorFilter = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, selectorFilterEditorAPI, undefined, setLoader, selectorFilterEditorReadyDeferred);
                };

                $scope.scopeModel.onBEDefinitionSelectionChanged = function (selectedValue) {
                    if (selectedValue != undefined) {
                        if (beDefinitionSelectionChangedPromiseDeferred != undefined)
                            beDefinitionSelectionChangedPromiseDeferred.resolve();
                        else {
                            if (selectedValue.SelectorFilterEditor != undefined && selectedSelectorFilterEditor == selectedValue.SelectorFilterEditor) {

                                var selectorFilterEditorPayload = {
                                    beDefinitionId: selectedValue.Id
                                };
                                var setLoader = function (selectedValue) {
                                    $scope.scopeModel.isLoadingSelectorFilter = selectedValue;
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, selectorFilterEditorAPI, undefined, setLoader);
                            }
                        }
                    }
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var beDefinitionId;
                    var beRuntimeSelectorFilter;

                    if (payload != undefined) {
                        beDefinitionId = payload.beDefinitionId;
                        beRuntimeSelectorFilter = payload.beRuntimeSelectorFilter;
                    }

                    var beDefinitionSelectorLoadPromise = getBEDefinitionSelectorLoadPromise();
                    promises.push(beDefinitionSelectorLoadPromise);

                    if (beDefinitionId != undefined) {
                        beDefinitionSelectionChangedPromiseDeferred = UtilsService.createPromiseDeferred();

                        var selectorFilterPromise = UtilsService.createPromiseDeferred();
                        promises.push(selectorFilterPromise.promise);

                        VR_GenericData_BusinessEntityDefinitionAPIService.GetBusinessEntityDefinition(beDefinitionId).then(function (response) {
                            if (response && response.Settings != undefined && response.Settings.SelectorFilterEditor != undefined) {
                                selectorFilterEditorReadyDeferred = UtilsService.createPromiseDeferred();
                                getSelectorFilterEditorDirectiveLoadPromise().then(function () {
                                    selectorFilterPromise.resolve();
                                }).catch(function (error) {
                                    selectorFilterPromise.reject(error);
                                });
                            } else {
                                selectorFilterPromise.resolve();
                            }
                        }).catch(function (error) {
                            selectorFilterPromise.reject(error);
                        });
                    }

                    function getBEDefinitionSelectorLoadPromise() {
                        var beDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        var beDefinitionSelectorPayload = {
                            selectedIds: beDefinitionId
                        };
                        VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, beDefinitionSelectorPayload, beDefinitionSelectorLoadDeferred);

                        return beDefinitionSelectorLoadDeferred.promise;
                    }
                    function getSelectorFilterEditorDirectiveLoadPromise() {
                        var loadSelectorFilterEditorPromiseDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([selectorFilterEditorReadyDeferred.promise, beDefinitionSelectionChangedPromiseDeferred.promise])
                            .then(function () {
                                selectorFilterEditorReadyDeferred = undefined;
                                beDefinitionSelectionChangedPromiseDeferred = undefined;

                                var directivePayload = {
                                    beDefinitionId: beDefinitionId,
                                    beRuntimeSelectorFilter: beRuntimeSelectorFilter
                                };
                                VRUIUtilsService.callDirectiveLoad(selectorFilterEditorAPI, directivePayload, loadSelectorFilterEditorPromiseDeferred);
                            });

                        return loadSelectorFilterEditorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        beDefinitionId: beDefinitionSelectorAPI.getSelectedIds(),
                        beRuntimeSelectorFilter: selectorFilterEditorAPI != undefined ? selectorFilterEditorAPI.getData() : undefined
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('vrGenericdataBusinessentitydefinitionRuntimeselectorsettings', BEDefinitionRuntimeSelectorSettingsDirective);

})(app);