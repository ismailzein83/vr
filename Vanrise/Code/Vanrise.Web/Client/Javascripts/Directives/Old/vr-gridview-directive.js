'use strict';

app.service('GridViewService', ['BaseDirService', function (BaseDirService) {

    this.dTemplate = "/Client/Javascripts/Directives/Inputs/Old/vr-gridview-standard.html";

    this.getTemplateByType = function (type) {
        return "/Client/Javascripts/Directives/Inputs/Old/vr-gridview-" + type + ".html";
    };

    

}]);



app.directive('vrGridview', ['GridViewService', 'BaseDirService', function (GridViewService, BaseDirService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            gridoptions: '=',
            datasource:'='
        },
        transclude: true,
        controller: function () {
            this.getObjectProperty = function (item, property) {
                return BaseDirService.getObjectProperty(item, property);
            };
        },
        template: "<div ng-transclude><h3>Heading</h3></div>",
        controllerAs: 'ctrl',
        bindToController: true,
        //templateUrl: function (element, attrs) {
        //    return GridViewService.dTemplate;
        //},
        compile: function (tElement, attrs) {
        }
    };

    return directiveDefinitionObject;

}]);



app.directive('vrGridCol', ['GridViewService', function (GridViewService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            gridoptions: '=',
            datasource:'='
        },
        controller: function () {
            this.getObjectProperty = function (item, property) {
                return GridViewService.getObjectProperty(item, property);
            };
        },
        controllerAs: 'ctrl',
        bindToController: true,
        //templateUrl: function (element, attrs) {
        //    return GridViewService.dTemplate;
        //},
        compile: function (tElement, attrs) {
        }
    };

    return directiveDefinitionObject;

}]);










