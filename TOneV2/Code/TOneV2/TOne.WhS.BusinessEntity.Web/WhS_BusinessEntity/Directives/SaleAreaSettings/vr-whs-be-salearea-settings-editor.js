'use strict';

app.directive('vrWhsBeSaleareaSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SaleAreaSettings/Templates/SaleAreaSettingsTemplate.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {
            ctrl.fixedKeywords = [];
            ctrl.mobileKeywords = [];

            function initializeController() {
                ctrl.disabledAddFixedKeyword = true;
                ctrl.disabledAddMobileKeyword = true;

                ctrl.addFixedKeyword = function () {
                    ctrl.fixedKeywords.push({ fixedKeyword: ctrl.fixedKeywordvalue });
                    ctrl.fixedKeywordvalue = undefined;
                    ctrl.disabledAddFixedKeyword = true;
                }

                ctrl.onFixedKeywordValueChange = function (value) {
                    ctrl.disabledAddFixedKeyword = (value == undefined && ctrl.fixedKeywordvalue.length - 1 < 1) || UtilsService.getItemIndexByVal(ctrl.fixedKeywords, value, "fixedKeyword") != -1;
                }

                ctrl.validateAddFixedKeyWords = function () {
                    if (ctrl.fixedKeywords != undefined && ctrl.fixedKeywords.length == 0)
                        return "Enter at least one keyword.";
                    return null;
                };


                ctrl.addMobileKeyword = function () {
                    ctrl.mobileKeywords.push({ mobileKeyword: ctrl.mobileKeywordvalue });
                    ctrl.mobileKeywordvalue = undefined;
                    ctrl.disabledAddMobileKeyword = true;
                }

                ctrl.onMobileKeywordValueChange = function (value) {
                    ctrl.disabledAddMobileKeyword = (value == undefined && ctrl.mobileKeywordvalue.length - 1 < 1) || UtilsService.getItemIndexByVal(ctrl.mobileKeywords, value, "mobileKeyword") != -1;
                }

                ctrl.validateAddMobileKeyWords = function () {
                    if (ctrl.mobileKeywords != undefined && ctrl.mobileKeywords.length == 0)
                        return "Enter at least one keyword.";
                    return null;
                };


                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined) {

                        ctrl.defaultRate = payload.data.DefaultRate;

                        angular.forEach(payload.data.FixedKeywords, function (val) {
                            ctrl.fixedKeywords.push({fixedKeyword: val});
                        });

                        angular.forEach(payload.data.MobileKeywords, function (value) {
                            ctrl.mobileKeywords.push({ mobileKeyword: value });
                        });
                    }
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.SaleAreaSettingsData, TOne.WhS.BusinessEntity.Entities",
                        FixedKeywords: UtilsService.getPropValuesFromArray(ctrl.fixedKeywords, "fixedKeyword"),
                        MobileKeywords: UtilsService.getPropValuesFromArray(ctrl.mobileKeywords, "mobileKeyword"),
                        DefaultRate: ctrl.defaultRate
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);