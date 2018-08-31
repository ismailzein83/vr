'use strict';


app.directive('vrTabsHeader', ['MobileService', 'VRModalService', function (MobileService, VRModalService) {

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
                    scope.ctrl = tabsCtrl;
                    scope.isMobile = MobileService.isMobile();
                    scope.isTabHeaderHidden = function (index) {
                        return index != tabsCtrl.selectedTabIndex && scope.isMobile;
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

                    scope.hasInvalidTab = function () {
                        for (var i = 0 ; i < tabsCtrl.tabs.length ; i++) {
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
            var starttemplate = isvertical ? '' : '<vr-row removeline ><vr-columns width="fullrow" >';
            var endtemplate = isvertical ? '' : '</vr-columns> </vr-row>';
            var verticalflag = isvertical == true ? "vertical" : " ";

            var template = '<vr-tab-header-links ' + verticalflag + ' selectedindex="ctrl.selectedTabIndex" onselectionchanged="ctrl.tabSelectionChanged()" ng-if="ctrl.tabs.length > 0">'
                            + '      <vr-tab-header-link  ng-repeat="tab in ctrl.tabs" id="header-{{tab.guid}}" ng-show="tab.showTab == undefined || tab.showTab == true" isvisible="tab.showTab == undefined || tab.showTab == true"  isselected="tab.isSelected" >{{ tab.header }} <i ng-if="tab.onremove"  class="glyphicon glyphicon-remove hand-cursor tab-remove-icon" ng-click="ctrl.removeTab(tab)"></i> <span ng-if="tab.validationContext.validate() != null" class="tab-validation-sign"  title="has validation errors!">*</span></vr-tab-header-link>'
                            + '</vr-tab-header-links>';

            if (MobileService.isMobile()) {
                template = '';
                template = '<vr-tab-header-links ' + verticalflag + ' selectedindex="ctrl.selectedTabIndex" onselectionchanged="ctrl.tabSelectionChanged()" ng-if="ctrl.tabs.length > 0">'
                            + '      <vr-tab-header-link ng-click="openTabsSelectorPopup()"id="header-{{tab.guid}}" ng-repeat="tab in ctrl.tabs"  isvisible="tab.showTab == undefined || tab.showTab == true" isselected="tab.isSelected" ng-hide="isTabHeaderHidden($index)">{{ tab.header }} <i ng-if="tab.onremove"  class="glyphicon glyphicon-remove hand-cursor tab-remove-icon" ng-click="ctrl.removeTab(tab)"></i> <span ng-if="tab.validationContext.validate() != null" class="tab-validation-sign"  title="has validation errors!">*</span></vr-tab-header-link>'
                            + '      <label class="hand-cursor" ng-click="openTabsSelectorPopup()" ng-show="showTabSelctorButton()"><span class="glyphicon glyphicon-chevron-right" style="font-size:18px;"></span><span ng-if="hasInvalidTab()" class="tab-validation-sign"  title="has validation errors!">*</span></label>'
                       + '</vr-tab-header-links>';
            }
            return starttemplate + template + endtemplate;

        }
    };

    return directiveDefinitionObject;
}]);

