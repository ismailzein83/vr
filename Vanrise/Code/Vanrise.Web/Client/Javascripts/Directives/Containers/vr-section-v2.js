'use strict';

app.directive('vrSectionV2', ['UtilsService', 'MultiTranscludeService', function (UtilsService, MultiTranscludeService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            menuactions: "="
        },
        transclude: true,
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            $scope.classlevel = "1";
            $scope.header = $attrs.header;
            $scope.collapsible = $attrs.collapsible != undefined;
            if ($attrs.level != undefined && $attrs.level == "2")
                $scope.classlevel = " panel-vr-child ";
            $scope.expandname = 'expanded_' + UtilsService.replaceAll(UtilsService.guid(), '-', '');

        },
        controllerAs: 'sectionCtrl',
        bindToController: true,
        template: function (attrs) {
            var htmlTempalte = '<div class="panel-primary panel-vr" ng-class="classlevel" >'
                + '<div class="panel-heading" ng-init="expandname=true" expanded="{{expandname}}"><label><span style="padding-right: 3px;" class="hand-cursor collapsible-icon glyphicon " ng-show="collapsible" ng-click=" expandname =!expandname " ng-class=" expandname ?\'glyphicon-collapse-up\':\'glyphicon-collapse-down\'" ></span>{{header}}</label><div class="section-menu" ng-if="sectionCtrl.menuactions.length > 0"><vr-button type="SectionAction" menuactions="sectionCtrl.menuactions" isasynchronous="true" ></vr-button></div></div>'
                + '<div class="panel-body" ng-show="expandname"  ng-transclude></div></div>';
            return htmlTempalte;
        }
    };

    return directiveDefinitionObject;

}]);