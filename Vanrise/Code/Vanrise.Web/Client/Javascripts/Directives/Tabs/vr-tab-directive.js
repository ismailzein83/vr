﻿"use strict";


app.directive("vrTab", ["MultiTranscludeService", "UtilsService", "VRLocalizationService", function (MultiTranscludeService, UtilsService, VRLocalizationService) {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: "E",
        scope: {
            tabobject: "=",
            ontabselected: '='
        },
        require: "^vrTabs",
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
        },
        controllerAs: "ctrl",
        bindToController: true,
        link: function ($scope, elem, iAttrs, tabsCtrl, transcludeFn) {

            var ctrl = $scope.ctrl;
            ctrl.vertical = elem.parent().attr("vertical") != undefined;
            if (ctrl.tabobject == undefined)
                ctrl.tabobject = {};
            var tab = ctrl.tabobject;
            tab.onremove = iAttrs.onremove != undefined ? $scope.$parent.$eval(iAttrs.onremove) : undefined;

            var header = iAttrs.header != undefined ? $scope.$parent.$eval(iAttrs.header) : iAttrs.header;
            if (iAttrs.localizedheader != undefined) {
                var localizedheader = $scope.$parent.$eval(iAttrs.localizedheader);
                if (localizedheader != undefined && VRLocalizationService.isLocalizationEnabled()) {
                    var label = VRLocalizationService.getResourceValue(localizedheader, header);
                    if (label != undefined && label != null && label != '')
                        header = label;
                }
            }
            tab.header = header;
            tab.data = iAttrs.data != undefined ? $scope.$parent.$eval(iAttrs.data) : header;
            if (tab.showTab == undefined)
                tab.showTab = iAttrs.showtab != undefined ? $scope.$parent.$eval(iAttrs.showtab) : undefined;
            var dontLoad = iAttrs.dontload != undefined ? $scope.$parent.$eval(iAttrs.dontload) : false;
            if (!dontLoad)
                tab.isLoaded = true;
            tab.guid = UtilsService.guid();
            var haspermission = iAttrs.haspermission != undefined && typeof ($scope.$parent.$eval(iAttrs.haspermission) == 'function') ? $scope.$parent.$eval(iAttrs.haspermission) : undefined;
            tab.haspermission = haspermission;
            tabsCtrl.addTab(tab);

            ctrl.getMinHeight = function () {
                return tabsCtrl.getMinHeight(ctrl);
            };
            elem.bind("$destroy", function () {
                if (ctrl.tabobject != undefined && tab.onremove == undefined) {
                    tabsCtrl.removeTab(ctrl.tabobject);
                    ctrl.tabobject = undefined;
                }
                else {
                    tabsCtrl.removeTabAndHeader(ctrl.tabobject);
                    ctrl.tabobject = undefined;
                }
            });

            $scope.$watch('ctrl.tabobject.isSelected', function (value) {
                if (ctrl.tabobject && ctrl.tabobject.isSelected) {
                    if (ctrl.ontabselected != undefined && typeof (ctrl.ontabselected) == 'function')
                        ctrl.ontabselected();
                }
            });
        },
        templateUrl: function (elem, attrs) {
            return "/Client/Javascripts/Directives/Tabs/Templates/TabTemplate.html";
        }
    };

    return directiveDefinitionObject;



}]);

