'use strict';

app.directive('vrGenericdataFieldtypeCustomobjectRuntimeeditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

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
                var ctor = new customObjectCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'runtimeEditorCtrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function customObjectCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;


            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var fieldType;
                    var fieldValue;
                    var allFieldValuesByName;

                    if (payload != undefined) {
                        fieldType = payload.fieldType;
                        fieldValue = payload.fieldValue;
                        allFieldValuesByName = payload.allFieldValuesByName;

                        $scope.scopeModel.fieldTitle = payload.fieldTitle;
                    }

                    if (fieldType != undefined && fieldType.Settings != undefined) {

                        if ($scope.selector == undefined) {
                            $scope.selector = { directive: fieldType.Settings.SelectorUIControl };
                        }

                        $scope.selector.onDirectiveReady = function (api) {
                            directiveAPI = api;
                            directiveReadyDeferred.resolve();
                        };

                        var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(directiveLoadPromiseDeferred.promise);

                        directiveReadyDeferred.promise.then(function () {

                            var directivePayload = {
                                fieldTitle: $scope.scopeModel.fieldTitle,
                                fieldValue: fieldValue,
                                fieldType: fieldType,
                                allFieldValuesByName: allFieldValuesByName
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadPromiseDeferred);
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return directiveAPI.getData();
                };

                api.setOnlyViewMode = function () {
                    UtilsService.setContextReadOnly($scope);
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames) { //allFieldValuesByFieldNames { field1: [value1, value2], ...}
                    if (directiveAPI != undefined && directiveAPI.onFieldValueChanged != undefined && typeof (directiveAPI.onFieldValueChanged) == "function")
                        return directiveAPI.onFieldValueChanged(allFieldValuesByFieldNames);

                    return UtilsService.waitPromiseNode({ promises: [] });
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }           
        }

        function getDirectiveTemplate(attrs) {

            if (attrs.selectionmode == 'single') {
                return getSingleSelectionModeTemplate();
            }

            function getSingleSelectionModeTemplate() {

                return '<vr-directivewrapper ng-if="selector.directive != undefined" directive="selector.directive" normal-col-num="{{runtimeEditorCtrl.normalColNum}}"  on-ready="selector.onDirectiveReady" onselectionchanged="selector.onselectionchanged" '
                    + ' isrequired="runtimeEditorCtrl.isrequired"></vr-directivewrapper>';
            }
        }

        return directiveDefinitionObject;
    }]);