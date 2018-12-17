//"use strict";

//app.directive("vrCommonTextpatternbuilder", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VRModalService",
//    function (UtilsService, VRNotificationService, VRUIUtilsService, VRModalService) {

//        var directiveDefinitionObject = {
//            restrict: "E",
//            scope:{
//                onReady: "=",
//                normalColNum: '@',
//                isrequired: "="
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new TextPatternBuilderCtor($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            template: function (element, attrs) {
//                return getTemplate(attrs);
//            }       
//        };

//        function getTemplate(attrs) {

//            var withemptyline = 'withemptyline';
//            if (attrs.hidelabel != undefined)
//                withemptyline = '';

//            var label = "Text Pattern";
//            if (attrs.label != undefined)
//                label = attrs.label;

//            var template = '<vr-columns colnum="{{ctrl.normalColNum}}">'
//                + '<vr-label ng-if="ctrl.hidelabel ==undefined">' + label + '</vr-label>'
//                + '<vr-textbox value="ctrl.fullTextPattern" isrequired="ctrl.isrequired"></vr-textbox>'
//                + '</vr-columns>'
//                + '<vr-columns width="normal" ' + withemptyline + '>'
//                + '<vr-button type="Help" data-onclick="scopeModel.openPatternHelper" standalone></vr-button>'
//                + '</vr-columns>';
//            return template;

//        }

//        function TextPatternBuilderCtor($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var parts;
//            var context;
//            var patternSeparator = "#";

//            function initializeController() {
//                $scope.scopeModel = {};

//                $scope.scopeModel.openPatternHelper = function () {
//                    var modalSettings = {};

//                    modalSettings.onScopeReady = function (modalScope) {
//                        modalScope.onSetPattern = function (patternName) {
//                            if (ctrl.fullTextPattern == undefined)
//                                ctrl.fullTextPattern = "";
//                            ctrl.fullTextPattern += patternSeparator + patternName + patternSeparator;
//                        };
//                    };
//                    var parameter = {
//                        context: getContext()
//                    };
//                    VRModalService.showModal('/Client/Modules/Common/Directives/VRTextPattern/Templates/PatternHelperTemplate.html', parameter, modalSettings);
//                };

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {

//                    if (payload != undefined) {
//                        context = payload.context;
//                        parts = payload.parts;
//                        ctrl.fullTextPattern = payload.fullTextPattern;

//                        if (payload.patternSeparator != undefined)
//                            patternSeparator = payload.patternSeparator;

//                        var promises = [];
//                        return UtilsService.waitMultiplePromises(promises);
//                    }
//                };

//                api.getData = function () {
//                    return ctrl.fullTextPattern;
//                };

//                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
//                    ctrl.onReady(api);
//            }

//            function getContext() {
//                var currentContext = context;
//                if (currentContext != undefined)
//                    return currentContext;

//                currentContext = {};
//                currentContext.getParts = function () {
//                    return parts;
//                };
//                return currentContext;
//            }
//        }

//        return directiveDefinitionObject;
//    }
//]);