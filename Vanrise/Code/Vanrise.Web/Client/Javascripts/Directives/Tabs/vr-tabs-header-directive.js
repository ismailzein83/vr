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
            }
        },
        template: function (element) {
            var template = '<vr-row removeline>'
                                + '<vr-columns width="fullrow" >'
                                + '    <vr-tab-header-links selectedindex="ctrl.selectedTabIndex" onselectionchanged="ctrl.tabSelectionChanged()">'
                                  + '      <vr-tab-header-link ng-repeat="tab in ctrl.tabs" isselected="tab.isSelected">{{ tab.header }} <span ng-if="tab.validationContext.validate() != null" style="color:#D44A47;position:relative;right: calc(100% + 7px);" title="has validation errors!">*</span></vr-tab-header-link>'
                                    + '</vr-tab-header-links>'
                                + '</vr-columns>'
                            + '</vr-row>';
            return template;

        }
    };

    return directiveDefinitionObject;
}]);

