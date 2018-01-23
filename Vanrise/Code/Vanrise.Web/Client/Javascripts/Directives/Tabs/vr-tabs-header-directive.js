'use strict';


app.directive('vrTabsHeader', [function () {

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
                }
            };
        },
        template: function (element) {
            var isvertical = element.parent().attr("vertical") != undefined;
            var starttemplate = isvertical ? '': '<vr-row removeline ><vr-columns width="fullrow" >';
            var endtemplate = isvertical ? '' : '</vr-columns> </vr-row>';
            var verticalflag = isvertical == true ? "vertical" : " ";
            var template = '<vr-tab-header-links ' + verticalflag + ' selectedindex="ctrl.selectedTabIndex" onselectionchanged="ctrl.tabSelectionChanged()" ng-if="ctrl.tabs.length > 0">'
                            + '      <vr-tab-header-link  ng-repeat="tab in ctrl.tabs" ng-show="tab.showTab == undefined || tab.showTab == true" isselected="tab.isSelected" >{{ tab.header }} <i ng-if="tab.onremove"  class="glyphicon glyphicon-remove hand-cursor tab-remove-icon" ng-click="ctrl.removeTab(tab)"></i> <span ng-if="tab.validationContext.validate() != null" class="tab-validation-sign"  title="has validation errors!">*</span></vr-tab-header-link>'
                            + '</vr-tab-header-links>';
            return starttemplate + template +  endtemplate;

        }
    };

    return directiveDefinitionObject;
}]);

