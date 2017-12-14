"use strict";


app.directive("vrTab", ["MultiTranscludeService", "UtilsService","VRLocalizationService", function (MultiTranscludeService, UtilsService, VRLocalizationService) {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: "E",
        scope: {
            tabobject:"="
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
                header = VRLocalizationService.getResourceValue(localizedheader, header);
            }
            tab.header = header;
            tab.data = iAttrs.data != undefined ? $scope.$parent.$eval(iAttrs.data) : header;
            if (tab.showTab==undefined)
                tab.showTab = iAttrs.showtab != undefined ? $scope.$parent.$eval(iAttrs.showtab) : undefined;
            var dontLoad = iAttrs.dontload != undefined ? $scope.$parent.$eval(iAttrs.dontload) : false;
            if (!dontLoad)
                tab.isLoaded = true;
            tab.guid = UtilsService.guid();
            tabsCtrl.addTab(tab);
           
            ctrl.getMinHeight = function () {
                return tabsCtrl.getMinHeight(ctrl);
            };           
            elem.bind("$destroy", function () {
                if (ctrl.tabobject != undefined && tab.onremove==undefined) {
                   tabsCtrl.removeTab(ctrl.tabobject);
                    ctrl.tabobject = undefined;
                }
            });
        },
        templateUrl: function (elem, attrs) {
                return "/Client/Javascripts/Directives/Tabs/Templates/TabTemplate.html";
        }
    };

    return directiveDefinitionObject;



}]);

