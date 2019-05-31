'use strict';
app.directive('vrGenericdataFieldtypeTextresourceRuntimeeditor', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            selectionmode: '@',
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            $scope.scopeModel = {};
            var ctor = new textResourceCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'runtimeEditorCtrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: function (element, attrs) {
            return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/TextResource/Templates/TextResourceFieldTypeRuntimeTemplate.html';
        }
    };

    function textResourceCtor(ctrl, $scope, $attrs) {

        this.initializeController = initializeController;

        var gridAPI;
        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.translatedValues = [];
            $scope.scopeModel.normalColNum = ctrl.normalColNum; 
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            }; 
            $scope.scopeModel.addTranslatedValue = function () {
                var dataItem = {
                 
                };
                dataItem.onLanguageSelectorReady = function (api) {
                    dataItem.languageSeletorAPI = api;
                    var setLoader = function (value) { dataItem.isLanguageSelectorLoading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.languageSeletorAPI, undefined, setLoader);
                };
                $scope.scopeModel.translatedValues.push(dataItem);
            };
            $scope.scopeModel.onDeleteRow = function (dataItem) {
                var index = $scope.scopeModel.translatedValues.indexOf(dataItem);
                $scope.scopeModel.translatedValues.splice(index, 1);
            };  
        }


        function defineAPI() {
            var api = {};

            api.load = function (payload) {
              
                var fieldValue;
                var promises = [];
                if (payload != undefined) {
                 
                    fieldValue = payload.fieldValue;
                }
                if (fieldValue != undefined && fieldValue.TranslatedValues) {
                    var translatedValues = fieldValue.TranslatedValues;

                    for (var languageId in translatedValues) {
                        if (languageId != "$type") {
                            var translationPayload = {
                                languageId: languageId,
                                translation: translatedValues[languageId]
                            };

                            var translationItem = {
                                payload: translationPayload,
                                languageReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                languageLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            promises.push(translationItem.languageLoadPromiseDeferred.promise);
                            addSelectedTranslatedValue(translationItem);
                        }
                    }
                }
                return UtilsService.waitPromiseNode({ promises: promises });

            };
            function addSelectedTranslatedValue(translationItem) {

                var languagePayload;
                var dataItem = {};
                if (translationItem.payload != undefined) {
                    dataItem.Translation = translationItem.payload.translation;
                    languagePayload = { selectedIds: translationItem.payload.languageId };
                }
                dataItem.onLanguageSelectorReady = function (api) {
                    dataItem.languageSeletorAPI = api;
                    translationItem.languageReadyPromiseDeferred.resolve();
                };

                translationItem.languageReadyPromiseDeferred.promise
                    .then(function () {
                        VRUIUtilsService.callDirectiveLoad(dataItem.languageSeletorAPI, languagePayload, translationItem.languageLoadPromiseDeferred);
                    });

                $scope.scopeModel.translatedValues.push(dataItem);

            }

            api.getData = function () {
                var retVal = {};
                for (var i = 0; i < $scope.scopeModel.translatedValues.length; i++) {
                    var translatedValue = $scope.scopeModel.translatedValues[i];
                    retVal[translatedValue.languageSeletorAPI.getSelectedIds()] = translatedValue.Translation;

                }
                return {
                    $type: "Vanrise.GenericData.Entities.TextResourceFieldTypeEntity,Vanrise.GenericData.Entities",
                    TranslatedValues:retVal
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

    }

    return directiveDefinitionObject;
}]);

