'use strict';


app.directive('vrList', [ 'MultiTranscludeService', function ( MultiTranscludeService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        transclude: true,
        scope: {
            datasource: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.itemsSortable = { animation: 150 };

            this.getdatasource = function () {
                return ctrl.datasource;
            };
           
        },
        controllerAs: 'ctrl',
        bindToController: true,
        //compile: function (tElement, tAttrs, transclude) {
        //    var rpt = document.createAttribute('ng-repeat');
        //    rpt.nodeValue = tAttrs.element;
        //    tElement[0].children[0].attributes.setNamedItem(rpt);
        //    return function (scope, element, attr) {
        //        var rhs = attr.element.split(' in ')[1];
        //        scope.items = $parse(rhs)(scope);
        //        console.log(scope.items);
        //    }        
        //},
        link: function (scope, elem, attr, ctrl, transcludeFn) {
            MultiTranscludeService.transclude(elem, transcludeFn);
        },
        template: function (element, attrs) {
            var template = '';
            template =  '<div transclude-id="header"></div>'
                      + '<div transclude-id="rowdiv"></div><div transclude-id="rowdiv"></div>';
            return template;
        }


    };

    return directiveDefinitionObject;



}]);

