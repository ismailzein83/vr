"use strict";

app.directive("vrCommonTextpatternbuilder", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VRModalService", "TextPatternBuilderService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VRModalService, TextPatternBuilderService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: "=",
                customlabel: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TextPatternBuilderCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {

            var withemptyline = 'withemptyline';
            if (attrs.hidelabel != undefined)
                withemptyline = '';

            var label = "Text Pattern";
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var template = '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + '<vr-label ng-if="ctrl.hidelabel==undefined">' + label + '</vr-label>'
                + '<vr-textbox value="ctrl.textPattern" isrequired="ctrl.isrequired" style="display:inline-block;width:calc(100% - 28px);"></vr-textbox>'
                + '<vr-button type="Help" data-onclick="scopeModel.openPatternHelper" standalone></vr-button>'
                + '</vr-columns>';
            return template;
        }

        function TextPatternBuilderCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var parts;
            var patternSeparator = "#";

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.openPatternHelper = function () {
                    var onSetPattern = function (patternName) {
                        if (ctrl.textPattern == undefined)
                            ctrl.textPattern = "";
                        ctrl.textPattern += patternSeparator + patternName + patternSeparator;
                    };
                    TextPatternBuilderService.openPatternHelper(onSetPattern, parts);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        parts = payload.parts;
                        ctrl.textPattern = payload.textPattern;

                        if (payload.patternSeparator != undefined)
                            patternSeparator = payload.patternSeparator;

                        var promises = [];
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return ctrl.textPattern;
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);