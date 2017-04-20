(function (app) {

    'use strict';

    BusinessEntityDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_BusinessEntityDefinitionAPIService'];

    function BusinessEntityDirective(UtilsService, VRUIUtilsService, VR_GenericData_BusinessEntityDefinitionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var businessEntity = new BusinessEntity(ctrl, $scope);
                businessEntity.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/BusinessEntity/Templates/BusinessEntityFieldTypeTemplate.html';
            }
        };

        function BusinessEntity(ctrl, $scope) {
            this.initializeController = initializeController;

            var selectorAPI;
            var selectedBusinessEntityDefinitionReadyPromiseDeferred;
            var selectorFilterEditorAPI;
            var selectorFilterEditorReadyDeferred;
            var selectedSelector;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onSelectorFilterEditorDirectiveReady = function (api) {
                    selectedSelector = $scope.scopeModel.selectedBusinessEntityDefinition.SelectorFilterEditor;
                    selectorFilterEditorAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingSelectorFilter = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, selectorFilterEditorAPI, undefined, setLoader, selectorFilterEditorReadyDeferred);
                };
                $scope.scopeModel.onBusinessEntityDefinitionSelectionChanged = function (value) {
                    if (selectedBusinessEntityDefinitionReadyPromiseDeferred != undefined)
                        selectedBusinessEntityDefinitionReadyPromiseDeferred.resolve();
                    else
                    {
                        if (value != undefined && value.SelectorFilterEditor != undefined && selectedSelector == value.SelectorFilterEditor) {
                            var setLoader = function (value) {
                                $scope.scopeModel.isLoadingSelectorFilter = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, selectorFilterEditorAPI, undefined, setLoader);
                        }
                    }
                };
                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedId;
                    var promises = [];
                    if (payload != undefined) {
                        selectedId = payload.BusinessEntityDefinitionId;
                        $scope.scopeModel.isNullable = payload.IsNullable;
                    }
                    promises.push(loadSelector());
                    if (payload != undefined && payload.BusinessEntityDefinitionId != undefined)
                    {
                        selectedBusinessEntityDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                        var selectorFilterPromise = UtilsService.createPromiseDeferred();
                        promises.push(selectorFilterPromise.promise);
                        VR_GenericData_BusinessEntityDefinitionAPIService.GetBusinessEntityDefinition(payload.BusinessEntityDefinitionId).then(function (response) {
                            if (response && response.Settings != undefined && response.Settings.SelectorFilterEditor != undefined)
                            {
                                selectorFilterEditorReadyDeferred = UtilsService.createPromiseDeferred();
                                loadSelectorFilterEditorDirectiveSection().then(function () {
                                    selectorFilterPromise.resolve();
                                }).catch(function (error) {
                                    selectorFilterPromise.reject(error);
                                });
                            }else
                            {
                                selectorFilterPromise.resolve();
                            }
                        }).catch(function (error) {
                            selectorFilterPromise.reject(error);
                        });
                    }
                    function loadSelector() {
                        var selectorLoadDeferred = UtilsService.createPromiseDeferred();
                        var selectorPayload = { selectedIds: selectedId };
                        VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, selectorLoadDeferred);
                        return selectorLoadDeferred.promise;
                    }
                    function loadSelectorFilterEditorDirectiveSection() {
                        var loadSelectorFilterEditorPromiseDeferred = UtilsService.createPromiseDeferred();
                        UtilsService.waitMultiplePromises([selectorFilterEditorReadyDeferred.promise, selectedBusinessEntityDefinitionReadyPromiseDeferred.promise])
                            .then(function () {
                                selectorFilterEditorReadyDeferred = undefined;
                                selectedBusinessEntityDefinitionReadyPromiseDeferred = undefined;
                                var directivePayload = { beFilter: payload.SelectorFilter };
                                VRUIUtilsService.callDirectiveLoad(selectorFilterEditorAPI, directivePayload, loadSelectorFilterEditorPromiseDeferred);
                            });

                        return loadSelectorFilterEditorPromiseDeferred.promise;
                    }

                   return  UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions',
                        BusinessEntityDefinitionId: selectorAPI.getSelectedIds(),
                        SelectorFilter:selectorFilterEditorAPI != undefined? selectorFilterEditorAPI.getData():undefined,
                        IsNullable: $scope.scopeModel.isNullable
                    };
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataFieldtypeBusinessentity', BusinessEntityDirective);

})(app);