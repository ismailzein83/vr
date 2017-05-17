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

            var beDefinitionRuntimeSelectorSettingsSelectorAPI;
            var beDefinitionRuntimeSelectorSettingsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBEDefinitionRuntimeSelectorSettingsSelectorReady = function (api) {
                    beDefinitionRuntimeSelectorSettingsSelectorAPI = api;
                    beDefinitionRuntimeSelectorSettingsSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var isNullable;
                    var beDefinitionId;
                    var beRuntimeSelectorFilter;

                    if (payload != undefined) {
                        isNullable = payload.IsNullable;
                        beDefinitionId = payload.BusinessEntityDefinitionId;
                        beRuntimeSelectorFilter = payload.BERuntimeSelectorFilter;
                    }

                    $scope.scopeModel.isNullable = isNullable;

                    var beDefinitionRuntimeSeletorSettingsLoadPromise = getBEDefinitionRuntimeSeletorSettingsLoadPromise();
                    promises.push(beDefinitionRuntimeSeletorSettingsLoadPromise);


                    function getBEDefinitionRuntimeSeletorSettingsLoadPromise() {
                        var beDefinitionRuntimeSeletorSettingsLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionRuntimeSelectorSettingsSelectorReadyPromiseDeferred.promise.then(function () {
                            var beDefinitionRuntimeSeletorSettingsPayload = {
                                beDefinitionId: beDefinitionId,
                                beRuntimeSelectorFilter: beRuntimeSelectorFilter
                            };
                            VRUIUtilsService.callDirectiveLoad(beDefinitionRuntimeSelectorSettingsSelectorAPI, beDefinitionRuntimeSeletorSettingsPayload, beDefinitionRuntimeSeletorSettingsLoadDeferred);
                        });

                        return beDefinitionRuntimeSeletorSettingsLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var beDefinitionId, beRuntimeSelectorFilter;
                    var beDefinitionRuntimeSelectorSettings = beDefinitionRuntimeSelectorSettingsSelectorAPI.getData();
                    if (beDefinitionRuntimeSelectorSettings != undefined) {
                        beDefinitionId = beDefinitionRuntimeSelectorSettings.beDefinitionId;
                        beRuntimeSelectorFilter = beDefinitionRuntimeSelectorSettings.beRuntimeSelectorFilter;
                    }

                    return {
                        $type: 'Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType, Vanrise.GenericData.MainExtensions',
                        IsNullable: $scope.scopeModel.isNullable,
                        BusinessEntityDefinitionId: beDefinitionId,
                        BERuntimeSelectorFilter: beRuntimeSelectorFilter
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('vrGenericdataFieldtypeBusinessentity', BusinessEntityDirective);

})(app);