(function (app) {

    'use strict';

    MappingRuleSettingsFilterEditorDirective.$inject = ['VR_GenericData_DataRecordFieldAPIService', 'UtilsService', 'VRUIUtilsService'];

    function MappingRuleSettingsFilterEditorDirective(VR_GenericData_DataRecordFieldAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var isolatedCtrl = this;
                var mappingRuleSettingsFilterEditor = new MappingRuleSettingsFilterEditor(isolatedCtrl, $scope, $attrs);
                mappingRuleSettingsFilterEditor.initializeController();
            },
            controllerAs: 'isolatedCtrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function MappingRuleSettingsFilterEditor(isolatedCtrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                isolatedCtrl.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directiveReadyDeferred.resolve();
                };

                if (isolatedCtrl.onReady != undefined) {
                    isolatedCtrl.onReady(getDirectiveAPI());
                }
            }

            function getDirectiveAPI() {
                var api = {};
                api.load = function (payload) {
                    
                    var fieldTitle;
                    var fieldType;

                    if (payload != undefined) {
                        fieldTitle = payload.settings.FieldTitle;
                        fieldType = payload.settings.FieldType;
                    }

                    var promises = [];
                    var fieldTypeConfigs;

                    var getFieldTypeConfigsPromise = getFieldTypeConfigs();
                    promises.push(getFieldTypeConfigsPromise);

                    var loadWrapperDirectiveDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadWrapperDirectiveDeferred.promise);

                    getFieldTypeConfigsPromise.then(function () {
                        isolatedCtrl.directiveEditor = UtilsService.getItemByVal(fieldTypeConfigs, fieldType.ConfigId, 'ExtensionConfigurationId').FilterEditor;

                        directiveReadyDeferred.promise.then(function () {
                            var directivePayload = {
                                fieldTitle: fieldTitle,
                                fieldType: fieldType
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, loadWrapperDirectiveDeferred);
                        });
                    });

                    return UtilsService.waitMultiplePromises(promises);

                    function getFieldTypeConfigs() {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                            fieldTypeConfigs = [];
                            for (var i = 0; i < response.length; i++) {
                                fieldTypeConfigs.push(response[i]);
                            }
                        });
                    }
                };
                api.getData = function () {
                    return directiveAPI.getData();
                };
                return api;
            }
        }

        function getDirectiveTemplate(attrs) {
            return '<vr-directivewrapper directive="isolatedCtrl.directiveEditor" on-ready="isolatedCtrl.onDirectiveReady" normal-col-num="{{isolatedCtrl.normalColNum}}"" />';
        }
    }

    app.directive('vrGenericdataGenericrulesettingsMappingFiltereditor', MappingRuleSettingsFilterEditorDirective);

})(app);