"use strict";

app.directive("vrWhsBeSalepricelistfilenamepattern", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VRModalService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VRModalService) {

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

                var ctor = new SalePricelistFileNamePattern($scope, ctrl, $attrs);
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
            var template = '<vr-columns colnum="{{ctrl.normalColNum}}">'
                             + '<vr-label ng-if="ctrl.hidelabel ==undefined">' + label + '</vr-label>'
                             + '<vr-textbox value="ctrl.value" isrequired="ctrl.isrequired"></vr-textbox>'
                         + '</vr-columns>'
                         + '<vr-columns width="normal" ' + withemptyline + '>'
                            + '<vr-button type="Help" data-onclick="scopeModel.openFileNamePatternHelper" standalone></vr-button>'
                         + '</vr-columns>';
            return template;

        }

        function SalePricelistFileNamePattern($scope, ctrl, $attrs) {
            var context;
            var invoiceTypeId;
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
                    };
                    VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/SalePricelistFileNamePatternTemplateHelper.html', parameter, modalSettings);
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {

                        if (payload.fileNamePattern != undefined)
                            ctrl.value = payload.fileNamePattern;

                        var promises = [];

                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return ctrl.value;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);