(function (app) {

    'use strict';

    ArrayFieldTypeFilterEditorDirective.$inject = ['VR_GenericData_DataRecordFieldAPIService', 'UtilsService', 'VRUIUtilsService'];

    function ArrayFieldTypeFilterEditorDirective(VR_GenericData_DataRecordFieldAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectionmode: '@',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var arrayFieldTypeFilterEditor = new ArrayFieldTypeFilterEditor(ctrl, $scope, $attrs);
                arrayFieldTypeFilterEditor.initializeController();
            },
            controllerAs: 'filterEditorCtrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function ArrayFieldTypeFilterEditor(ctrl, $scope, $attrs) {

            var fieldTypeFilterEditorAPI;
            var fieldTypeFilterEditorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                ctrl.onFieldTypeFilterEditorReady = function (api) {
                    fieldTypeFilterEditorAPI = api;
                    fieldTypeFilterEditorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var fieldTitle;
                    var fieldType;

                    if (payload != undefined) {
                        fieldTitle = payload.fieldTitle;
                        fieldType = payload.fieldType;
                    }

                    var promises = [];
                    var fieldTypeConfigs;

                    var getFieldTypeConfigsPromise = getFieldTypeConfigs();
                    promises.push(getFieldTypeConfigsPromise);

                    var fieldTypeFilterEditorLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(fieldTypeFilterEditorLoadDeferred.promise);

                    getFieldTypeConfigsPromise.then(function () {
                        var fieldTypeConfig = UtilsService.getItemByVal(fieldTypeConfigs, fieldType.FieldType.ConfigId, 'ExtensionConfigurationId');
                        ctrl.fieldTypeFilterEditorDirective = fieldTypeConfig.FilterEditor;

                        fieldTypeFilterEditorReadyDeferred.promise.then(function () {
                            var fieldTypeFilterEditorPaylaod = {
                                fieldTitle: fieldTitle,
                                fieldType: fieldType.FieldType
                            };
                            VRUIUtilsService.callDirectiveLoad(fieldTypeFilterEditorAPI, fieldTypeFilterEditorPaylaod, fieldTypeFilterEditorLoadDeferred);
                        });
                    });

                    function getFieldTypeConfigs() {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                            fieldTypeConfigs = [];
                            for (var i = 0; i < response.length; i++) {
                                fieldTypeConfigs.push(response[i]);
                            }
                        });
                    }
                };
                api.getValuesAsArray = function ()
                {
                    return directiveAPI.getData();
                }
                api.getData = function () {
                    return fieldTypeFilterEditorAPI.getData();
                };

                if (ctrl.onReady != undefined) {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }

        function getDirectiveTemplate(attrs) {
            return '<vr-directivewrapper directive="filterEditorCtrl.fieldTypeFilterEditorDirective" on-ready="filterEditorCtrl.onFieldTypeFilterEditorReady" selectionmode="multiple" normal-col-num="{{filterEditorCtrl.normalColNum}}" isrequired="filterEditorCtrl.isrequired" />';
        }
    }

    app.directive('vrGenericdataFieldtypeArrayFiltereditor', ArrayFieldTypeFilterEditorDirective);

})(app);