(function (app) {

    'use strict';

    DataDatarecordtypefieldsFormulaDirective.$inject = ['VR_GenericData_DataRecordFieldAPIService', 'UtilsService', 'VRUIUtilsService'];

    function DataDatarecordtypefieldsFormulaDirective(VR_GenericData_DataRecordFieldAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataDatarecordtypefieldsFormula = new DataDatarecordtypefieldsFormula($scope, ctrl, $attrs);
                dataDatarecordtypefieldsFormula.initializeController();
            },
            controllerAs: "formulaCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function DataDatarecordtypefieldsFormula($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.extensionConfigs = [];
                $scope.scopeModel.selectedExtensionConfig;
                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directivePayload = { context: context };
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var formula;

                    if (payload != undefined) {
                        formula = payload.formula;
                        context = payload.context;
                    }

                    var loadDataRecordFieldFormulaExtensionConfigsPromise = loadDataRecordFieldFormulaExtensionConfigs();
                    promises.push(loadDataRecordFieldFormulaExtensionConfigsPromise);

                    if (formula != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function loadDataRecordFieldFormulaExtensionConfigs() {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldFormulaExtensionConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.extensionConfigs.push(response[i]);
                                }
                                if (formula != undefined && formula.ConfigId !=undefined)
                                    $scope.scopeModel.selectedExtensionConfig = UtilsService.getItemByVal($scope.scopeModel.extensionConfigs, formula.ConfigId, 'ExtensionConfigurationId');
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { formula: formula, context: context };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;
                    if ($scope.scopeModel.selectedExtensionConfig != undefined)
                    {
                        data = directiveAPI != undefined ? directiveAPI.getData() : undefined;
                        if (data != undefined)
                            data.ConfigId = $scope.scopeModel.selectedExtensionConfig.ExtensionConfigurationId;
                    }
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }

        function getTamplate(attrs) {
            var label = "label='Formula Type'";

            if (attrs.hidelabel != undefined) {
                label = "label='Formula Types'";
            }

            return '<vr-row><vr-columns colnum="{{formulaCtrl.normalColNum * 2}}">'
                    + '<vr-select on-ready="scopeModel.onSelectorReady" datasource="scopeModel.extensionConfigs" selectedvalues="scopeModel.selectedExtensionConfig" datavaluefield="ExtensionConfigurationId" datatextfield="Title" ' + label + ' isrequired="formulaCtrl.isrequired"></vr-select>'
                + '</vr-columns></vr-row>'
                + '<vr-row ng-if="scopeModel.selectedExtensionConfig !=undefined"><vr-directivewrapper directive="scopeModel.selectedExtensionConfig.Editor" on-ready="scopeModel.onDirectiveReady" normal-col-num="{{formulaCtrl.normalColNum}}" isrequired="formulaCtrl.isrequired" customvalidate="formulaCtrl.customvalidate"></vr-directivewrapper></vr-row>';
        }
    }

    app.directive('vrGenericdataDatarecordtypefieldsFormula', DataDatarecordtypefieldsFormulaDirective);

})(app);