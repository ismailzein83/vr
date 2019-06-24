(function (app) {

    'use strict';

    BEDefinitionRuntimeSelectorSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function BEDefinitionRuntimeSelectorSettingsDirective(UtilsService, VRUIUtilsService) {

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
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/Templates/BEDefinitionRuntimeSelectorSettings.html';
            }
        };

        function BEDefinitionRuntimeSelectorSettingsCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var context;
            var selectedSelectorFilterEditor;

            var beDefinitionSelectorAPI;
            var beDefinitionSelectionChangedPromiseDeferred;

            var selectorFilterEditorAPI;
            var selectorFilterEditorReadyDeferred = UtilsService.createPromiseDeferred();

            var dependantFieldsGridAPI;
            var dependantFieldsGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBEDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDependantFieldsGridReady = function (api) {
                    dependantFieldsGridAPI = api;
                    dependantFieldsGridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSelectorFilterEditorDirectiveReady = function (api) {
                    selectorFilterEditorAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingSelectorFilter = value;
                    };
                    var selectorFilterEditorPayload = {
                        beDefinitionId: beDefinitionSelectorAPI.getSelectedIds()
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, selectorFilterEditorAPI, selectorFilterEditorPayload, setLoader, selectorFilterEditorReadyDeferred);
                };

                $scope.scopeModel.onBEDefinitionSelectionChanged = function (selectedValue) {
                    if (selectedValue != undefined) {
                        var previousSelectedSelectorFilterEditor = selectedSelectorFilterEditor;
                        selectedSelectorFilterEditor = selectedValue.SelectorFilterEditor;

                        if (beDefinitionSelectionChangedPromiseDeferred != undefined) {
                            beDefinitionSelectionChangedPromiseDeferred.resolve();
                        }
                        else {
                            if (selectedSelectorFilterEditor == previousSelectedSelectorFilterEditor) {
                                var setLoader = function (value) {
                                    $scope.scopeModel.isLoadingSelectorFilter = value;
                                };
                                var selectorFilterEditorPayload = {
                                    beDefinitionId: beDefinitionSelectorAPI.getSelectedIds()
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, selectorFilterEditorAPI, selectorFilterEditorPayload, setLoader);
                            }
                            reloadDependantFieldsGrid(selectedValue);
                        }
                    }

                    function reloadDependantFieldsGrid(selectedBusinessEntityDefinition) {

                        var dependantFieldsGridPayload = {
                            beDefinitionId: selectedBusinessEntityDefinition.BusinessEntityDefinitionId,
                            context: context
                        };
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingDependantFieldsGrid = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dependantFieldsGridAPI, dependantFieldsGridPayload, setLoader);
                    }
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var beDefinitionId;
                    var beRuntimeSelectorFilter;
                    var beDependantFields;

                    if (payload != undefined) {
                        beDefinitionId = payload.beDefinitionId;
                        beRuntimeSelectorFilter = payload.beRuntimeSelectorFilter;
                        beDependantFields = payload.beDependantFields;

                        var additionalParameters = payload.additionalParameters;
                        if (additionalParameters != undefined) {
                            context = additionalParameters.context;
                            $scope.scopeModel.showDependantFieldsGrid = additionalParameters.showDependantFieldsGrid;
                        }
                    }

                    var loadBEDefinitionSelectorPromise = loadBEDefinitionSelector();
                    promises.push(loadBEDefinitionSelectorPromise);

                    if (beDefinitionId != undefined) {
                        beDefinitionSelectionChangedPromiseDeferred = UtilsService.createPromiseDeferred();

                        var loadDependantFieldsGridPromise = loadDependantFieldsGrid();
                        promises.push(loadDependantFieldsGridPromise);

                        var selectorFilterPromise = UtilsService.createPromiseDeferred();
                        promises.push(selectorFilterPromise.promise);

                        beDefinitionSelectionChangedPromiseDeferred.promise.then(function () {
                            if (selectedSelectorFilterEditor != undefined) {
                                loadSelectorFilterEditorDirective().then(function () {
                                    selectorFilterPromise.resolve();
                                }).catch(function (error) {
                                    selectorFilterPromise.reject(error);
                                });
                            } else {
                                selectorFilterPromise.resolve();
                            }
                        });
                    }

                    function loadBEDefinitionSelector() {
                        var beDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        var beDefinitionSelectorPayload = {
                            selectedIds: beDefinitionId
                        };
                        VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, beDefinitionSelectorPayload, beDefinitionSelectorLoadDeferred);

                        return beDefinitionSelectorLoadDeferred.promise;
                    }

                    function loadSelectorFilterEditorDirective() {
                        var loadSelectorFilterEditorPromiseDeferred = UtilsService.createPromiseDeferred();

                        selectorFilterEditorReadyDeferred.promise.then(function () {

                            var directivePayload = {
                                beDefinitionId: beDefinitionId,
                                beRuntimeSelectorFilter: beRuntimeSelectorFilter
                            };
                            VRUIUtilsService.callDirectiveLoad(selectorFilterEditorAPI, directivePayload, loadSelectorFilterEditorPromiseDeferred);
                        });

                        return loadSelectorFilterEditorPromiseDeferred.promise;
                    }

                    function loadDependantFieldsGrid() {
                        var loadDependantFieldsGridPromiseDeferred = UtilsService.createPromiseDeferred();

                        dependantFieldsGridReadyPromiseDeferred.promise.then(function () {

                            var dependantFieldsGridPayload = {
                                beDefinitionId: beDefinitionId,
                                context: context
                            };
                            if (beDependantFields != undefined) {
                                dependantFieldsGridPayload.beDependantFields = beDependantFields;
                            }
                            VRUIUtilsService.callDirectiveLoad(dependantFieldsGridAPI, dependantFieldsGridPayload, loadDependantFieldsGridPromiseDeferred);
                        });

                        return loadDependantFieldsGridPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        beDefinitionSelectionChangedPromiseDeferred = undefined;
                        selectorFilterEditorReadyDeferred = undefined;
                    });
                };

                api.getData = function () {
                    return {
                        beDefinitionId: beDefinitionSelectorAPI.getSelectedIds(),
                        beRuntimeSelectorFilter: selectorFilterEditorAPI != undefined ? selectorFilterEditorAPI.getData() : undefined,
                        beDependantFields: dependantFieldsGridAPI != undefined ? dependantFieldsGridAPI.getData() : undefined
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('vrGenericdataBusinessentitydefinitionRuntimeselectorsettings', BEDefinitionRuntimeSelectorSettingsDirective);
})(app);