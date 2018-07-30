"use strict";

app.directive("vrAnalyticAutomatedreportFilenamepattern", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VRModalService", "VR_Analytic_AutomatedReportAPIService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VRModalService, VR_Analytic_AutomatedReportAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
                normalColNum: '@',
                isrequired: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new FileNamePattern($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            if (attrs.hidelabel != undefined)
                withemptyline = '';
            var label = "File Name Pattern";
            if (attrs.label != undefined)
                label = attrs.label;
            var template = '<vr-columns colnum="{{ctrl.normalColNum*2/3}}">'
                             + '<vr-label ng-if="ctrl.hidelabel ==undefined">' + label + '</vr-label>'
                             +'<vr-validator validate="scopeModel.validateFileName()">'
                             + '<vr-textbox value="ctrl.value" isrequired="ctrl.isrequired"></vr-textbox>'
                             +'</vr-validator>'
                         + '</vr-columns>'
                         + '<vr-columns colnum="{{ctrl.normalColNum/3}}" withemptyline>'
                            + '<vr-button type="Help" data-onclick="scopeModel.openFileNamePatternHelper" standalone></vr-button>'
                         + '</vr-columns>';
            return template;

        }
        function FileNamePattern($scope, ctrl, $attrs) {
            var context;
            var parts;
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.openFileNamePatternHelper = function () {
                    var modalSettings = {};

                    modalSettings.onScopeReady = function (modalScope) {
                        modalScope.onSetFileNamePattern = function (fileNamePatternValue) {
                            if (ctrl.value == undefined)
                                ctrl.value = "";
                            ctrl.value += fileNamePatternValue;
                        };
                    };
                    var parameter = {
                        context: getContext()
                    };
                    VRModalService.showModal('/Client/Modules/Analytic/Directives/AutomatedReport/Handler/Templates/AutomatedReportFileNamePatternHelper.html', parameter, modalSettings);
                };

                $scope.scopeModel.validateFileName = function () {
                    if (ctrl.value != undefined) {
                        return UtilsService.validateFileName(ctrl.value);
                    }
                    return null;
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.fileNamePattern != undefined)
                            ctrl.value = payload.fileNamePattern;
                        var promises = [];

                        function loadParts() {
                            return VR_Analytic_AutomatedReportAPIService.GetAutomatedReportSettings().then(function (response) {
                                parts = response.FileNameParts;
                            });
                        }
                        promises.push(loadParts());
                        

                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return ctrl.value;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                if (currentContext.getParts == undefined) {
                    currentContext.getParts = function () {
                        var returnedParts = [];
                        if (parts != undefined) {
                            for (var i = 0, length = parts.length ; i < length; i++) {
                                returnedParts.push(parts[i]);
                            }
                        }
                        return returnedParts;
                    };
                }
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);