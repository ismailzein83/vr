"use strict";

app.directive("vrVouchercardsSerialnumberPattern", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VRModalService", "VR_Voucher_VoucherTypeAPIService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VRModalService, VR_Voucher_VoucherTypeAPIService) {

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

                var ctor = new OverallVoucherCardSerialNumberPart($scope, ctrl, $attrs);
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
            var template ='<vr-row>'
                +'<vr-columns colnum="{{ctrl.normalColNum}}">'
                +'<vr-label>Serial Number Part Initial Sequence</vr-label>'
                             + '<vr-textbox value="ctrl.serialNumberPartInitialSequence" isrequired="ctrl.isrequired"></vr-textbox>'
                             + '</vr-columns>'
            +'</vr-row>'
                             +'<vr-columns colnum="{{ctrl.normalColNum}}">'
                             + '<vr-label ng-if="ctrl.hidelabel ==undefined">' + label + '</vr-label>'
                             + '<vr-textbox value="ctrl.serialNumberPattern" isrequired="ctrl.isrequired"></vr-textbox>'
                         + '</vr-columns>'
                         + '<vr-columns width="normal" ' + withemptyline + '>'
                            + '<vr-button type="Help" data-onclick="scopeModel.openSerialNumberPatternHelper" standalone></vr-button>'
                         + '</vr-columns>';
            return template;
        }
        function OverallVoucherCardSerialNumberPart($scope, ctrl, $attrs) {
            var parts;
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.openSerialNumberPatternHelper = function () {
                    var modalSettings = {};

                    modalSettings.onScopeReady = function (modalScope) {
                        modalScope.onSetSerialNumberPattern = function (serialNumberPatternValue) {
                            if (ctrl.serialNumberPattern == undefined)
                                ctrl.serialNumberPattern = "";
                            ctrl.serialNumberPattern += serialNumberPatternValue;
                        };
                    };
                    var parameter = {
                        context: getContext()
                    };
                    VRModalService.showModal('/Client/Modules/VR_Voucher/Elements/VoucherCards/Directives/Templates/SerialNumberPatternHelper.html', parameter, modalSettings);
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        if (payload.data != undefined){                            
                        if (payload.data.SerialNumberPattern != undefined)
                            ctrl.serialNumberPattern = payload.data.SerialNumberPattern;
                        if(payload.data.SerialNumberPartInitialSequence!= undefined)
                            ctrl.serialNumberPartInitialSequence=payload.data.SerialNumberPartInitialSequence;
                    }
                        var promises = [];                       
                        var promise = VR_Voucher_VoucherTypeAPIService.GetVoucherCardDefinition().then(function (response) {                            
                            parts = response;
                            });
                            promises.push(promise);
                        

                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Voucher.Entities.VoucherCardSettings ,Vanrise.Voucher.Entities",
                        SerialNumberPattern: ctrl.serialNumberPattern,
                        SerialNumberPartInitialSequence: ctrl.serialNumberPartInitialSequence
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext() {
                  var  currentContext = {};
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