'use strict';


app.directive('vrListItem', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            
        },
        require: '^vrList',
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            element.html('<li class="list-group-item" >' + element.html() + '</li>');


            return {
                pre: function ($scope, iElem, iAttrs, choicesCtrl) {
                    //var ctrl = $scope.ctrl;

                   
                }
            }
        }
    };

    return directiveDefinitionObject;



}]);

