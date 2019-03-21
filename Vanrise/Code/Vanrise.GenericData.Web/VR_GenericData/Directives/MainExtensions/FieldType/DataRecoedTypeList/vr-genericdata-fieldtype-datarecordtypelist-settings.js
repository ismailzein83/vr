//(function (app) {

//    'use strict';

//    DataRecordFieldTypeDataRecordTypeListDirective.$inject = ['VR_GenericData_DataRecordFieldAPIService', 'UtilsService', 'VRUIUtilsService'];

//    function DataRecordFieldTypeDataRecordTypeListDirective(VR_GenericData_DataRecordFieldAPIService, UtilsService, VRUIUtilsService) {
//        return {
//            restrict: "E",
//            scope:
//            {
//                onReady: "=",
//                normalColNum: "@"
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;

//                $scope.normalColNum = "4";
//                if (ctrl.normalColNum != undefined)
//                    $scope.normalColNum = ctrl.normalColNum;

//                var runtimeViewTypeSelective = new RuntimeViewTypeSelective($scope, ctrl, $attrs);
//                runtimeViewTypeSelective.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            template: function (element, attrs) {
//                return getDirectiveTemplate(attrs);
//            }
//        };
//        function getDirectiveTemplate(attrs) {
//            var label = 'label="Type"';
//            var removeLine = "";
//            if (attrs.hidelabel != undefined) {
//                label = "";
//                removeLine = "removeline";
//            }

//            var required = "isrequired";
//            if (attrs.isnotrequired != undefined)
//                required = "";

//            return '<vr-columns colnum="{{normalColNum}}">'
//                  + '<vr-select on-ready="scopeModel.onSelectorReady"'
//                + 'datasource="scopeModel.runtimeViewTypesConfigs"'
//                  + 'selectedvalues="scopeModel.selectedFieldTypeConfig"'
//                  + 'datavaluefield="ExtensionConfigurationId"'
//                  + 'datatextfield="Title"'
//                  + label
//                  + ' text="None"'
//                  + ' ' + required
//                  + ' hideremoveicon>'
//           + '</vr-select>'
//          + '</vr-columns>'
//        + ' <span  ng-if="scopeModel.selectedFieldTypeConfig != undefined" vr-loader="scopeModel.isLoadingDirective">'
//             + ' <vr-directivewrapper normal-col-num="{{normalColNum}}" directive="scopeModel.selectedFieldTypeConfig.Editor" on-ready="scopeModel.onDirectiveReady"></vr-directivewrapper>'
//         + '</span>';
//        }
//        function RuntimeViewTypeSelective($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var selectorAPI;

//            var directiveAPI;
//            var directiveReadyDeferred;
//            var directivePayload;

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.runtimeViewTypesConfigs = [];
//                $scope.scopeModel.selectedFieldTypeConfig;

//                $scope.scopeModel.onSelectorReady = function (api) {
//                    selectorAPI = api;
//                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                        ctrl.onReady(getDirectiveAPI());
//                    }
//                };

//                $scope.scopeModel.onDirectiveReady = function (api) {
//                    directiveAPI = api;
//                    var setLoader = function (value) {
//                        $scope.scopeModel.isLoadingDirective = value;
//                    };
//                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
//                };
//            }

//            function getDirectiveAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];
//                    var configId;

//                    if (payload != undefined) {
//                        configId = payload.ConfigId;
//                        directivePayload = payload;
//                    }
            
//                    var getFieldTypeConfigsPromise = getFieldTypeConfigs();
//                    promises.push(getFieldTypeConfigsPromise);

//                    var loadDirectiveDeferred = UtilsService.createPromiseDeferred();
//                    promises.push(loadDirectiveDeferred.promise);

//                    getFieldTypeConfigsPromise.then(function () {
//                        if (configId != undefined) {
//                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
//                            $scope.scopeModel.selectedFieldTypeConfig = UtilsService.getItemByVal($scope.scopeModel.runtimeViewTypesConfigs, configId, 'ExtensionConfigurationId');

//                            directiveReadyDeferred.promise.then(function () {
//                                directiveReadyDeferred = undefined;
//                                VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, loadDirectiveDeferred);
//                            });
//                        }
//                        else {
//                            loadDirectiveDeferred.resolve();
//                        }
//                    });

//                    return UtilsService.waitMultiplePromises(promises);

//                    function getFieldTypeConfigs() {
//                        return VR_GenericData_DataRecordFieldAPIService.GetListRecordRuntimeViewTypeConfigs().then(function (response) {
//                            if (response != null) {
//                                selectorAPI.clearDataSource();
//                                for (var i = 0; i < response.length; i++) {
//                                    $scope.scopeModel.runtimeViewTypesConfigs.push(response[i]);
//                                }
//                            }
//                        });
//                    }
//                };

//                api.getData = function () {
//                    var data = null;

//                    if ($scope.scopeModel.selectedFieldTypeConfig != undefined) {
//                        data = directiveAPI.getData();
//                        data.ConfigId = $scope.scopeModel.selectedFieldTypeConfig.ExtensionConfigurationId;
//                    }

//                    return data;
//                };

//                return api;
//            }
//        }
//    }

//    app.directive('vrGenericdataFieldtypeDatarecordtypelistSettings', DataRecordFieldTypeDataRecordTypeListDirective);

//})(app);