(function (app) {

    'use strict';

    fieldmappingSelectiveDirective.$inject = ['ExcelConversion_ExcelAPIService', 'CDRComparison_CDRSourceAPIService', 'CDRComparison_CDRSourceConfigAPIService', 'UtilsService', 'VRUIUtilsService'];

    function fieldmappingSelectiveDirective(ExcelConversion_ExcelAPIService, CDRComparison_CDRSourceAPIService, CDRComparison_CDRSourceConfigAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var fieldmappingSelective = new FieldmappingSelective($scope, ctrl, $attrs);
                fieldmappingSelective.initializeController();
            },
            controllerAs: "fieldmappingCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/ExcelConversion/Directives/FieldMapping/Templates/FieldMappingSelectiveTemplate.html"
        };

        function FieldmappingSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;


            function initializeController() {
                $scope.templateConfigs = [];
                $scope.selectedTemplateConfig;

                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directivePayload = {
                        context: getContext()
                    }
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined)
                    {
                        context = payload.context;
                    }
                    var getFieldMappingTemplateConfigsPromise = getFieldMappingTemplateConfigs();
                    promises.push(getFieldMappingTemplateConfigsPromise);

                    return UtilsService.waitMultiplePromises(promises);

                    function getFieldMappingTemplateConfigs() {
                        return ExcelConversion_ExcelAPIService.GetFieldMappingTemplateConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.templateConfigs.push(response[i]);
                                }
                            }
                        });
                    }


                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data;
                    if ($scope.selectedTemplateConfig != undefined) {
                        data = directiveAPI.getData();
                        data.ConfigId = $scope.selectedTemplateConfig.TemplateConfigID;
                    }
                    return data;
                }
            }
            function getContext()
            {
                
                if(context !=undefined)
                {
                    var currentContext = UtilsService.cloneObject(context);
                    return currentContext;
                }
            }
            function getFieldMappingConfig() {
                return CDRComparison_CDRSourceConfigAPIService.GetCDRSourceConfig(cdrSourceConfigId).then(function (cdrSourceConfig) {
                    cdrSource = cdrSourceConfig.CDRSource;
                });
            }
        }
    }

    app.directive('excelconversionFieldmappingSelective', fieldmappingSelectiveDirective);

})(app);