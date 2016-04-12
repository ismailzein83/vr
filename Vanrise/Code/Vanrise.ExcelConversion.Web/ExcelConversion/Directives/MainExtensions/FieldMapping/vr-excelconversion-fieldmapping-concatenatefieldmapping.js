(function (app) {

    'use strict';

    concatenatedpartSelective.$inject = ['VR_ExcelConversion_ExcelAPIService', 'UtilsService', 'VRUIUtilsService'];

    function concatenatedpartSelective(VR_ExcelConversion_ExcelAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                label: '@',
                customvalidate: '=',
                type: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var concatenatedpartSelective = new ConcatenatedpartSelective($scope, ctrl, $attrs);
                concatenatedpartSelective.initializeController();
            },
            controllerAs: "concatenateFieldMappingCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/ExcelConversion/Directives/MainExtensions/FieldMapping/Templates/ConcatenateFieldMappingTemplate.html"
        };

        function ConcatenatedpartSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;


            function initializeController() {


                $scope.templateConfigs = [];
                $scope.selectedTemplateConfig;



                ctrl.removeField = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.id, 'id');
                    ctrl.datasource.splice(index, 1);
                };

                ctrl.onActionTemplateChanged = function () {
                    ctrl.disableAddButton = ($scope.selectedTemplateConfig == undefined);
                };

                ctrl.addField = function () {
                    var dataItem = {
                        id: ctrl.datasource.length + 1,
                        configId: $scope.selectedTemplateConfig.TemplateConfigID,
                        editor: $scope.selectedTemplateConfig.Editor,
                        name: $scope.selectedTemplateConfig.Name
                    };
                    dataItem.onDirectiveReady = function (api) {
                        dataItem.directiveAPI = api;
                        var payload = {
                            context: getContext()
                        };
                        var setLoader = function (value) { ctrl.isLoadingDirective = value };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, payload, setLoader);
                    };
                    ctrl.datasource.push(dataItem);


                    $scope.selectedTemplateConfig = undefined;
                };
                ctrl.disableAddButton = true;

                ctrl.isValid = function () {

                    if (ctrl.datasource.length > 0)
                        return null;
                    return "At least one field type should be added.";
                }



                ctrl.datasource = [];

                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                    }
                    var getFieldMappingTemplateConfigsPromise = getFieldMappingTemplateConfigs();
                    promises.push(getFieldMappingTemplateConfigsPromise);

                    function getFieldMappingTemplateConfigs() {
                        return VR_ExcelConversion_ExcelAPIService.GetConcatenatedPartTemplateConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.templateConfigs.push(response[i]);
                                }
                                $scope.selectedTemplateConfig = $scope.templateConfigs[0];
                            }
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data;

                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0) {
                        var parts = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var item = ctrl.datasource[i];
                            if (item.directiveAPI != undefined) {
                                var fieldData = item.directiveAPI.getData();
                                if (fieldData != undefined) {
                                    fieldData.ConfigId = item.configId;
                                    parts.push(fieldData);
                                }

                            }

                        }
                        data = {
                            $type: "ExcelConversion.MainExtensions.FieldMappings.ConcatenateFieldMapping, ExcelConversion.MainExtensions",
                            Parts: parts
                        };
                    }
                    return data;
                }
            }
            function getContext() {

                if (context != undefined) {
                    var currentContext = UtilsService.cloneObject(context);
                    return currentContext;
                }
            }
        }
    }

    app.directive('vrExcelconversionFieldmappingConcatenatefieldmapping', concatenatedpartSelective);

})(app);