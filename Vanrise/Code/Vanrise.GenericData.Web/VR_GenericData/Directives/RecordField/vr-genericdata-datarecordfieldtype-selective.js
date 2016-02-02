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

            var payloadObj;
            function initializeController() {
                $scope.scopeModal = {};
                $scope.scopeModal.fieldTypeConfigs = [];
                $scope.scopeModal.selectedFieldTypeConfig;

                $scope.scopeModal.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                $scope.scopeModal.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModal.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModal, directiveAPI, payloadObj, setLoader, directiveReadyDeferred);
                };
            }

            function getDirectiveAPI() {
                var api = {};
                
                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    return VR_GenericData_DataRecordFieldTypeConfigAPIService.GetDataRecordFieldTypes().then(function (response) {
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModal.fieldTypeConfigs.push(response[i]);
                            }
                            if (payload != undefined && payload.ConfigId != undefined) {
                                payloadObj = payload;
                                $scope.scopeModal.selectedFieldTypeConfig = UtilsService.getItemByVal($scope.scopeModal.fieldTypeConfigs, payload.ConfigId, 'DataRecordFieldTypeConfigId');
                            }
                        }
                    });
                };

                api.getData = function () {
                    var data = null;

                    if ($scope.scopeModal.selectedFieldTypeConfig != undefined) {
                        data = directiveAPI.getData();
                        data.ConfigId = $scope.scopeModal.selectedFieldTypeConfig.DataRecordFieldTypeConfigId;
                    }

                    return data;
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataDatarecordfieldtypeSelective', DataRecordFieldTypeSelectiveDirective);

})(app);