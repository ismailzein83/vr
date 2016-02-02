(function (app) {

    'use strict';

    DataRecordFieldTypeSelectiveDirective.$inject = ['VR_GenericData_DataRecordFieldTypeConfigAPIService'];

    function DataRecordFieldTypeSelectiveDirective(VR_GenericData_DataRecordFieldTypeConfigAPIService) {
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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordField/Templates/DataRecordFieldManagement.html"
        };

        function DataRecordFieldTypeSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;

            function initializeController() {
                ctrl.dataRecordFieldTypeTemplates = [];
                ctrl.selectedDataRecordFieldTypeTemplate;

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        ctrl.isLoadingDirective = value
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function getDirectiveAPI() {
                var api = {};
                
                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    var selectedId;

                    if (payload != undefined) {
                        selectedId = payload.selectedId;
                        directivePayload = payload.dataRecordFieldType;
                    }

                    return VR_GenericData_DataRecordFieldTypeConfigAPIService.GetDataRecordFieldTypes().then(function (response) {
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.dataRecordFieldTypeTemplates.push(item);
                            }

                            if (selectedId != undefined) {
                                ctrl.selectedDataRecordFieldTypeTemplate = UtilsService.getItemByValue(ctrl.dataRecordFieldTypeTemplates, selectedId, 'TemplateConfigID');
                            }
                        }
                    });
                };

                api.getData = function () {
                    var data = null;

                    if (ctrl.selectedDataRecordFieldTypeTemplate != undefined) {
                        data = directiveAPI.getData();
                        data.ConfigId = ctrl.selectedDataRecordFieldTypeTemplate.TemplateConfigID;
                    }

                    return data;
                };
            }
        }
    }

    app.directive('vrGenericdataDatarecordfieldtypeSelective', DataRecordFieldTypeSelectiveDirective);

})(app);