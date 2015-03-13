'use strict';

app.service('GridViewService', ['BaseDirService', function (BaseDirService) {

    this.dTemplate = BaseDirService.directiveMainURL + "vr-gridview-standard.html";

    this.getTemplateByType = function (type) {
        return BaseDirService.directiveMainURL + 'vr-gridview-' + type + '.html';
    };

    this.getObjectProperty = function (item, property) {
        if (property && ('function' === typeof property)) {
            return property(item);
        }
        var arr = property.split('.');
        while (arr.length) {
            item = item[arr.shift()];
        }
        return item;
    };

}]);



app.directive('vrGridview', ['GridViewService', function (GridViewService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            gridoptions: '=',
            datasource:'='
        },
        transclude: true,
        controller: function () {
            this.getObjectProperty = function (item, property) {
                return GridViewService.getObjectProperty(item, property);
            };
        },
        template: "<div ng-transclude><h3>Heading</h3></div>",
        controllerAs: 'ctrl',
        bindToController: true,
        //templateUrl: function (element, attrs) {
        //    return GridViewService.dTemplate;
        //},
        compile: function (tElement, attrs) {
            console.log(tElement);
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










