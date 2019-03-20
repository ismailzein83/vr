'use strict';
app.directive('vrGenericdataFieldtypeCustomobjectRuntimeeditor', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {

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
            var ctor = new customobjectCtor(ctrl, $scope, $attrs);
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
        template: function (element, attrs) {
            return getDirectiveTemplate(attrs);
        }
    };

	function customobjectCtor(ctrl, $scope, $attrs) {

        function initializeController() {
            $scope.scopeModel = {};


            defineAPI();
        }


        function defineAPI() {
            var api = {};
            var directiveAPI;
            var directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            api.load = function (payload) {

                var fieldType;
                var innerSectionPromises = [];
                

                if (payload != undefined) {
                    $scope.scopeModel.fieldTitle = payload.fieldTitle;
                    fieldType = payload.fieldType;
                }

                if (fieldType != undefined && fieldType.Settings!=undefined) {

                    if ($scope.selector == undefined) {
                        $scope.selector = {
                            directive:fieldType.Settings.SelectorUIControl
                        };
                    }
                    $scope.selector.onDirectiveReady = function (api) {
                        directiveAPI = api;
                        directiveReadyPromiseDeferred.resolve();
                    };

                    var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    innerSectionPromises.push(directiveLoadPromiseDeferred.promise);
                    $scope.selector.onselectionchanged = function (selectedvalue) {
                       
                    };

                    directiveReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            fieldTitle: $scope.scopeModel.fieldTitle,
                        };
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, payload, directiveLoadPromiseDeferred);
                    });
                }
                return UtilsService.waitMultiplePromises(innerSectionPromises);
            };

            api.getData = function () {
                return directiveAPI.getData();
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    function getDirectiveTemplate(attrs) {
        if (attrs.selectionmode == 'single') { 
            return getSingleSelectionModeTemplate();
        }
     
        function getSingleSelectionModeTemplate() {
            var multipleselection = "";

            if (attrs.selectionmode == "dynamic" || attrs.selectionmode == "multiple") {
                multipleselection = "ismultipleselection";
            }
          
            return '<vr-directivewrapper directive="selector.directive" normal-col-num="{{runtimeEditorCtrl.normalColNum}}"  on-ready="selector.onDirectiveReady" onselectionchanged="selector.onselectionchanged" '
                    + multipleselection + ' isrequired="runtimeEditorCtrl.isrequired"></vr-directivewrapper>';
        }
    }

    return directiveDefinitionObject;
}]);

