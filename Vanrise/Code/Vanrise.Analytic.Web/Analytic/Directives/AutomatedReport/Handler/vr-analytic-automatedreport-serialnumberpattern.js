"use strict";

app.directive("vrAnalyticAutomatedreportSerialnumberpattern", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VRModalService", "VR_Analytic_AutomatedReportAPIService",
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

                var ctor = new SerialNumberPattern($scope, ctrl, $attrs);
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
            var label = "Serial Number Pattern";
            if (attrs.label != undefined)
                label = attrs.label;
            var template = '<vr-columns colnum="{{ctrl.normalColNum*2/3}}">'
                             + '<vr-label ng-if="ctrl.hidelabel ==undefined">' + label + '</vr-label>'
                             + '<vr-textbox value="ctrl.value" isrequired="ctrl.isrequired"></vr-textbox>'
                         + '</vr-columns>'
                         + '<vr-columns colnum="{{ctrl.normalColNum/3}}" withemptyline>'
                            + '<vr-button type="Help" data-onclick="scopeModel.openSerialNumberPatternHelper" standalone></vr-button>'
                         + '</vr-columns>';
            return template;

        }
        function SerialNumberPattern($scope, ctrl, $attrs) {
            var context;
            var parts;
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.openSerialNumberPatternHelper = function () {
                    var modalSettings = {};

                    modalSettings.onScopeReady = function (modalScope) {
                        modalScope.onSetSerialNumberPattern = function (serialNumberPatternValue) {
                            if (ctrl.value == undefined)
                                ctrl.value = "";
                            ctrl.value += serialNumberPatternValue;
                        };
                    };
                    var parameter = {
                        context: getContext()
                    };
                    VRModalService.showModal('/Client/Modules/Analytic/Directives/AutomatedReport/Handler/Templates/AutomatedReportSerialNumberPatternHelper.html', parameter, modalSettings);
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.serialNumberPattern != undefined)
                            ctrl.value = payload.serialNumberPattern;
                        var promises = [];

                        function loadParts() {
                            return VR_Analytic_AutomatedReportAPIService.GetAutomatedReportSettings().then(function (response) {
                                parts = response.SerialNumberParts;
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