'use strict';


app.directive('vrTab', ['MultiTranscludeService', function (MultiTranscludeService) {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        scope: {
            tabobject:'='
        },
        require: '^vrTabs',
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
        },
        controllerAs: 'ctrl',
        bindToController: true,
        link: function ($scope, elem, iAttrs, tabsCtrl, transcludeFn) {
            var ctrl = $scope.ctrl;
            var tab = {};
            tab.header = iAttrs.header != undefined ? $scope.$parent.$eval(iAttrs.header) : iAttrs.header;
            var dontLoad = iAttrs.dontload != undefined ? $scope.$parent.$eval(iAttrs.dontload) : false;
            if (!dontLoad)
                tab.isLoaded = true;
            ctrl.tabobject = tab;
            tabsCtrl.addTab(tab);
        },
        templateUrl: '/Client/Javascripts/Directives/Tabs/Templates/TabTemplate.html'
    };

    return directiveDefinitionObject;



}]);

