'use strict';


app.directive('vrTabsHeader', ['MobileService', 'VRModalService', 'UtilsService', 'VRLocalizationService', function (MobileService, VRModalService, UtilsService, VRLocalizationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
        },
        require: '^vrTabs',
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {


            return {
                pre: function (scope, elem, attrs, tabsCtrl) {
                    scope.$on("$destroy", function () {
                        $(window).unbind('scroll', hideCurrentDropDownMenus);
                    });
                    scope.ctrl = tabsCtrl;
                    scope.isMobile = MobileService.isMobile();
                    scope.isTabHeaderHidden = function (tab) {
                        return !tab.isSelected && scope.isMobile;
                    };
                    scope.showTabSelctorButton = function () {
                        var visibletabs = 0;
                        for (var i = 0 ; i < tabsCtrl.tabs.length ; i++) {
                            var tab = tabsCtrl.tabs[i];
                            if (tab.showTab == undefined || tab.showTab == true)
                                visibletabs++;
                            if (visibletabs > 1)
                                return true;
                        }
                        return false;
                    };


                    scope.dropdownPreviousTabsId = UtilsService.replaceAll(UtilsService.guid(), '-', '');
                    scope.dropdownForwardTabsId = UtilsService.replaceAll(UtilsService.guid(), '-', '');

                    scope.initPaginitionControlsBehavior = function () {
                        setTimeout(function () {
                            $("#" + scope.dropdownPreviousTabsId).hover(function (event) {
                                var selfOffset = $(this).offset();
                                var baseleft = selfOffset.left - $(window).scrollLeft();
                                if (VRLocalizationService.isLocalizationRTL())
                                    baseleft -=160;
                                var basetop = selfOffset.top - $(window).scrollTop() + 15;
                                $("#" + scope.dropdownPreviousTabsId).find(".dropdown-menu").css({ display: "block", position: "fixed", left: baseleft, top: basetop });

                            }, function () {
                                $("#" + scope.dropdownPreviousTabsId).find(".dropdown-menu").css({ display: "none" });
                            });
                            $("#" + scope.dropdownForwardTabsId).hover(function () {

                                var selfOffset = $(this).offset();
                                var baseleft = selfOffset.left - $(window).scrollLeft();
                                 if (VRLocalizationService.isLocalizationRTL())
                                    baseleft -= 150;
                                var basetop = selfOffset.top - $(window).scrollTop() + 15;
                                $("#" + scope.dropdownForwardTabsId).find(".dropdown-menu").css({ display: "block", position: "fixed", left: baseleft, top: basetop });
                            }, function () {
                                $("#" + scope.dropdownForwardTabsId).find(".dropdown-menu").css({ display: "none" });
                            });
                            $(window).on('scroll', hideCurrentDropDownMenus);
                        }, 100);
                    };
                  
                    function hideCurrentDropDownMenus() {
                        $("#" + scope.dropdownForwardTabsId).find(".dropdown-menu").css({ display: "none" });
                        $("#" +scope.dropdownPreviousTabsId).find(".dropdown-menu").css({display: "none"});
                    }

                     scope.hasInvalidTab = function () {
                        for (var i = 0; i < tabsCtrl.tabs.length ; i++) {
                            var tab = tabsCtrl.tabs[i];
                            if (tab.validationContext != null && tab.validationContext.validate() != null && tabsCtrl.selectedTabIndex != i)
                                return true;
                     }
                         return false;
                    };
                    scope.openTabsSelectorPopup = function () {
                        var modalSettings = {
                    };
                        modalSettings.onScopeReady = function (modalScope) {
                            modalScope.ctrl = tabsCtrl;
                    };
                        VRModalService.showModal("/Client/Javascripts/Directives/Tabs/Templates/TabsSelectorModalPopup.html", null, modalSettings);
                    };

                }
            };
        },
        template: function (element) {
            var isvertical = element.parent().attr("vertical") != undefined;
            var flatflag = element.parent().attr("flat") != undefined ? "flat" : "";
            var hidepaginationcontrols = element.parent().attr("hidepaginationcontrols") != undefined ? "hidepaginationcontrols" : "";
            var starttemplate = isvertical ? '' : '<vr-row removeline ><vr-columns width="fullrow" >';
            var endtemplate = isvertical ? '' : '</vr-columns> </vr-row>';
            var verticalflag = isvertical == true ? "vertical" : " ";
             
            var template = '<vr-tab-header-links ' + verticalflag + ' ' + flatflag + ' ' + hidepaginationcontrols + '  backwardvisible="{{ctrl.isBackwardPaginationVisible()}}" forwardvisible="{{ctrl.isForwardPaginationVisible()}}" selectedindex="ctrl.selectedTabIndex" onselectionchanged="ctrl.tabSelectionChanged()" ng-if="ctrl.tabs.length > 0" >'
                + ' <span  ng-init="initPaginitionControlsBehavior()"   ng-if="!ctrl.hidepaginationcontrols" class="vr-tabs-expander previous" id="{{dropdownPreviousTabsId}}" style="position:relative" ng-show="(ctrl.tabsCountLimit > ctrl.pageSize && ctrl.tabs.length > ctrl.pageSize) ||  ( ctrl.tabs.length  <  ctrl.tabsCountLimit &&  ctrl.tabsCountStart > 0 )" >'
                                    + '  <span  data-toggle="dropdown" aria-haspopup="false" aria-expanded="false">'
                                            + ' <span class="glyphicon  glyphicon-backward hand-cursor list-view-icon" ></span>'
                                    + ' </span>'
                                    + '<ul class="dropdown-menu" >'
                                            + '<li  ng-repeat="tab in ctrl.getPreviousReversedTabs() " >'
                                                    + ' <a  ng-click="ctrl.setTabSelectedIndex(tab.orderedIndex,tab)"  orderindex="{{tab.orderedIndex}}" class="hand-cursor"><span><img class="tab-icon page-view-icon" ng-show="::tab.iconurl" ng-src="{{::tab.iconurl}}"/>'
                                                    + ' <span title="{{tab.header}}" ng-if="!ctrl.datatitlefield">{{tab.header}}</span>'
                + ' <span title="{{tab.tabItem[ctrl.datatitlefield]}}" ng-if="ctrl.datatitlefield" contenteditable="false" ng-model="tab.tabItem[ctrl.datatitlefield]"></span> '
                                                    +'</span> </a>'
                                            + '</li>'
                                    + '</ul>'
                            + '</span>'
                            + ' <span class="sortable-container" ng-sortable="ctrl.headerSortableListener" >'
	                            + ' <vr-tab-header-link  ng-repeat="tab in ctrl.tabs " id="header-{{tab.guid}}" ng-show="(tab.showTab == undefined || tab.showTab == true) && !ctrl.hideTab($index)" isvisible="(tab.showTab == undefined || tab.showTab == true) && !ctrl.hideTab($index)"  isselected="tab.isSelected" > '
		                            + ' <img class="tab-icon page-view-icon" ng-show="::tab.iconurl" ng-src="{{::tab.iconurl}}"/> '
		                            + ' <span class="vr-header-label {{tab.onremove || ctrl.sortable ?\'with-tab-control\':\'\'  }}" title="{{tab.header}}" ng-if="!ctrl.datatitlefield">{{tab.header}}</span>'
                + ' <span class="vr-header-label {{tab.onremove || ctrl.sortable ?\'with-tab-control\':\'\'  }} {{(tab.onremove || ctrl.sortable) &&  tab.iconurl ?\'icon-space\':\'\'  }} {{!tab.tabItem[ctrl.datatitlefield]?\'required-header\':\'\'}}"  title="{{tab.tabItem[ctrl.datatitlefield]}}" ng-if="ctrl.datatitlefield" contenteditable="true" ng-model="tab.tabItem[ctrl.datatitlefield]"></span> '
		                            + ' <i ng-if="tab.onremove"  class="glyphicon glyphicon-remove hand-cursor tab-remove-icon {{ctrl.oneditclicked ? \'share-space\':\'\'}}" ng-click="ctrl.removeTab(tab)"></i> '
		                            + ' <span ng-if="tab.validationContext.validate() != null" class="tab-validation-sign"  title="has validation errors!">*</span> '
		                            + ' <i ng-if="ctrl.sortable" class="glyphicon  glyphicon-option-horizontal handeldrag hand-cursor " ></i> '
		                            + ' <i ng-if="ctrl.oneditclicked" class="glyphicon   glyphicon-pencil edit  hand-cursor {{tab.onremove ? \'share-space\':\'\'}}" ng-click="ctrl.editClick(tab)"></i> '
	                            + ' </vr-tab-header-link> '
                            + '</span>'
                            + ' <span ng-if="!ctrl.hidepaginationcontrols" class="vr-tabs-expander forward" id="{{dropdownForwardTabsId}}"   ng-show="ctrl.tabs.length > ctrl.pageSize && ctrl.tabsCountLimit < ctrl.tabs.length " >'
                                + '  <span  data-toggle="dropdown" aria-haspopup="false" aria-expanded="false">'
                                        + ' <span class="glyphicon  glyphicon-forward hand-cursor list-view-icon" ></span>'
                                 + ' </span>'
                                + '<ul class="dropdown-menu" >'
                                        + '<li  ng-repeat="tab in ctrl.tabs " ng-hide="tab.orderedIndex < ctrl.tabsCountLimit">'
                                             + ' <a class="hand-cursor" ng-click="ctrl.setlastTabSelectedIndex(tab.orderedIndex,tab)"  orderindex="{{tab.orderedIndex}}" ><span><img class="tab-icon page-view-icon" ng-show="::tab.iconurl" ng-src="{{::tab.iconurl}}"/>'
                                             + ' <span title="{{tab.header}}" ng-if="!ctrl.datatitlefield">{{tab.header}}</span>'
                + ' <span title="{{tab.tabItem[ctrl.datatitlefield]}}" ng-if="ctrl.datatitlefield" contenteditable="false" ng-model="tab.tabItem[ctrl.datatitlefield]"></span> '
                                            +'</span> </a>'
                                        + '</li>'
                                + '</ul>'
                            +'</span>'
                        + '</vr-tab-header-links>';

            if (MobileService.isMobile()) {
                template = '';
                template = '<vr-tab-header-links ' + verticalflag + ' selectedindex="ctrl.selectedTabIndex" onselectionchanged="ctrl.tabSelectionChanged()" ng-if="ctrl.tabs.length > 0">'
                            + '      <vr-tab-header-link ng-click="openTabsSelectorPopup()"id="header-{{tab.guid}}" ng-repeat="tab in ctrl.tabs "  isvisible="tab.showTab == undefined || tab.showTab == true" isselected="tab.isSelected" ng-hide="isTabHeaderHidden(tab)">{{ tab.header }} <i ng-if="tab.onremove"  class="glyphicon glyphicon-remove hand-cursor tab-remove-icon" ng-click="ctrl.removeTab(tab)"></i> <span ng-if="tab.validationContext.validate() != null" class="tab-validation-sign"  title="has validation errors!">*</span></vr-tab-header-link>'
                            + '      <label class="hand-cursor" ng-click="openTabsSelectorPopup()" ng-show="showTabSelctorButton()"><span class="glyphicon glyphicon-chevron-right" style="font-size:18px;"></span><span ng-if="hasInvalidTab()" class="tab-validation-sign"  title="has validation errors!">*</span></label>'
                       + '</vr-tab-header-links>';
            }
            return starttemplate + template + endtemplate;

        }
    };

    return directiveDefinitionObject;
}]);

