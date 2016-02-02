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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordField/Templates/DataRecordFieldTypeSelectiveTemplate.html"
        };

        function DataRecordFieldTypeSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {

                $scope.fieldTypeConfigs = [];
                $scope.selectedFieldTypeConfig;

                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, $scope.selectedFieldTypeConfig.DataRecordFieldTypeConfigId, setLoader, directiveReadyDeferred);
                };
            }

            function getDirectiveAPI() {
                var api = {};
                
                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    var configId;

                    if (payload != undefined) {
                        configId = payload.configId;
                    }

                    return VR_GenericData_DataRecordFieldTypeConfigAPIService.GetDataRecordFieldTypes().then(function (response) {
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.fieldTypeConfigs.push(response[i]);
                            }

                            if (configId != undefined) {
                                $scope.selectedFieldTypeConfig = UtilsService.getItemByValue($scope.fieldTypeConfigs, configId, 'DataRecordFieldTypeConfigId');
                            }
                        }
                    });
                };

                api.getData = function () {
                    var data = null;

                    if ($scope.selectedFieldTypeConfig != undefined) {
                        data = directiveAPI.getData();
                        data.DataRecordFieldTypeConfigId = $scope.selectedFieldTypeConfig.DataRecordFieldTypeConfigId;
                    }

                    return data;
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataDatarecordfieldtypeSelective', DataRecordFieldTypeSelectiveDirective);

})(app);