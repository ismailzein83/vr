(function (app) {

    'use strict';

    DataRecordFieldTypeSelectiveDirective.$inject = ['VR_GenericData_DataRecordFieldTypeConfigAPIService', 'UtilsService', 'VRUIUtilsService'];

    function DataRecordFieldTypeSelectiveDirective(VR_GenericData_DataRecordFieldTypeConfigAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataRecordFieldTypeSelective = new DataRecordFieldTypeSelective($scope, ctrl, $attrs);
                dataRecordFieldTypeSelective.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Templates/FieldTypeSelectiveTemplate.html"
        };

        function DataRecordFieldTypeSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            var payloadObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.fieldTypeConfigs = [];
                $scope.scopeModel.selectedFieldTypeConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, directiveAPI, payloadObj, setLoader, directiveReadyDeferred);
                };
            }

            function getDirectiveAPI() {
                var api = {};
                
                api.load = function (payload) {
                    var promises = [];
                    var configId;

                    if (payload != undefined) {
                        configId = payload.ConfigId;
                    }

                    var getFieldTypeConfigsPromise = getFieldTypeConfigs();
                    promises.push(getFieldTypeConfigsPromise);

                    var loadDirectiveDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadDirectiveDeferred.promise);

                    getFieldTypeConfigsPromise.then(function () {
                        if (configId != undefined) {
                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            $scope.scopeModel.selectedFieldTypeConfig = UtilsService.getItemByVal($scope.scopeModel.fieldTypeConfigs, configId, 'DataRecordFieldTypeConfigId');
                        }
                    });

                    return UtilsService.waitMultiplePromises(promises);

                    function getFieldTypeConfigs() {
                        return VR_GenericData_DataRecordFieldTypeConfigAPIService.GetDataRecordFieldTypes().then(function (response) {
                            if (response != null) {
                                selectorAPI.clearDataSource();

                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.fieldTypeConfigs.push(response[i]);
                                }
                            }
                        });
                    }
                };

                api.getData = function () {
                    var data = null;

                    if ($scope.scopeModel.selectedFieldTypeConfig != undefined) {
                        data = directiveAPI.getData();
                        data.ConfigId = $scope.scopeModel.selectedFieldTypeConfig.DataRecordFieldTypeConfigId;
                    }

                    return data;
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataFieldtypeSelective', DataRecordFieldTypeSelectiveDirective);

})(app);