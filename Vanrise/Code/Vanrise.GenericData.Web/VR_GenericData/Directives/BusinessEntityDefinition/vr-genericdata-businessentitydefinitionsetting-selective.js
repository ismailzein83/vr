(function (app) {

    'use strict';

    BusinessEntityDefinitionSettingSelectiveDirective.$inject = ['VR_GenericData_BusinessEntityDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function BusinessEntityDefinitionSettingSelectiveDirective(VR_GenericData_BusinessEntityDefinitionAPIService, UtilsService, VRUIUtilsService) {
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
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };
        function getDirectiveTemplate(attrs) {
            var label = 'label="Type"';
            var removeLine = "";
            if (attrs.hidelabel != undefined) {
                label = "";
                removeLine = "removeline";
            }


            return '<vr-columns width="1/3row">'
                  + '<vr-select on-ready="scopeModel.onSelectorReady"'
                  + 'datasource="scopeModel.bEDefinitionSettingConfigs"'
                  + 'selectedvalues="scopeModel.selectedFieldTypeConfig"'
                  + 'datavaluefield="ExtensionConfigurationId"'
                  + 'datatextfield="Title"'
                  + label
                  + ' text="None"'
                   + ' isrequired'
                   + ' hideremoveicon>'
           + '</vr-select>'
          + '</vr-columns>'
        + ' <span  ng-if="scopeModel.selectedFieldTypeConfig != undefined" vr-loader="scopeModel.isLoadingDirective">'
             + ' <vr-directivewrapper normal-col-num="4" directive="scopeModel.selectedFieldTypeConfig.Editor" on-ready="scopeModel.onDirectiveReady"></vr-directivewrapper>'
         + '</span>';
        }
        function DataRecordFieldTypeSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.bEDefinitionSettingConfigs = [];
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
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var configId;

                    if (payload != undefined) {
                        configId = payload.ConfigId;
                        directivePayload = payload;
                    }

                    var getBEDefinitionSettingConfigsPromise = getBEDefinitionSettingConfigs();
                    promises.push(getBEDefinitionSettingConfigsPromise);

                    var loadDirectiveDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadDirectiveDeferred.promise);

                    getBEDefinitionSettingConfigsPromise.then(function () {
                        if (configId != undefined) {
                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            $scope.scopeModel.selectedFieldTypeConfig = UtilsService.getItemByVal($scope.scopeModel.bEDefinitionSettingConfigs, configId, 'ExtensionConfigurationId');

                            directiveReadyDeferred.promise.then(function () {
                                directiveReadyDeferred = undefined;
                                VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, loadDirectiveDeferred);
                            });
                        }
                        else {
                            loadDirectiveDeferred.resolve();
                        }
                    });

                    return UtilsService.waitMultiplePromises(promises);

                    function getBEDefinitionSettingConfigs() {
                        return VR_GenericData_BusinessEntityDefinitionAPIService.GetBEDefinitionSettingConfigs().then(function (response) {
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
                        data.ConfigId = $scope.scopeModel.selectedFieldTypeConfig.ExtensionConfigurationId;
                    }

                    return data;
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataBusinessentitydefinitionsettingSelective', BusinessEntityDefinitionSettingSelectiveDirective);

})(app);