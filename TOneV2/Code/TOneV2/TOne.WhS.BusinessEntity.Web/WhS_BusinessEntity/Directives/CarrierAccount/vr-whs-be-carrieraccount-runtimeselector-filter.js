'use strict';

app.directive('vrWhsBeCarrieraccountRuntimeselectorFilter', ['UtilsService', 'VRUIUtilsService', 'WhS_BE_AccountManagerDataTypeEnum',
    function (UtilsService, VRUIUtilsService, WhS_BE_AccountManagerDataTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                ctrl.datasource = [];
                ctrl.selectedvalues;

                var ctor = new CarrierAccountBERuntimeSelectorFilterCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CarrierAccount/Templates/CarrierAccountBERuntimeSelectorFilterTemplate.html"
        };

        function CarrierAccountBERuntimeSelectorFilterCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var dataTypeSelectorAPI;
            var dataTypeSelectorReadyPromiseDefereed = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataTypes = [];

                $scope.scopeModel.onDataTypeSelectorReady = function (api) {
                    dataTypeSelectorAPI = api;
                    dataTypeSelectorReadyPromiseDefereed.resolve();
                };

                UtilsService.waitMultiplePromises([dataTypeSelectorReadyPromiseDefereed.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    $scope.scopeModel.dataTypes.length = 0;

                    $scope.scopeModel.dataTypes = UtilsService.getArrayEnum(WhS_BE_AccountManagerDataTypeEnum);

                    if (payload != undefined && payload.beRuntimeSelectorFilter != undefined) {
                        $scope.scopeModel.selectedDataType = UtilsService.getItemByVal($scope.scopeModel.dataTypes, payload.beRuntimeSelectorFilter.DataType, 'value');
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    if ($scope.scopeModel.selectedDataType == undefined)
                        return null;

                    return {
                        $type: "TOne.WhS.BusinessEntity.Business.CarrierAccountBERuntimeSelectorFilter, TOne.WhS.BusinessEntity.Business",
                        DataType: $scope.scopeModel.selectedDataType.value
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);