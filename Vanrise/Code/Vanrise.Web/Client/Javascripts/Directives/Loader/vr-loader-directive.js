'use strict';


app.directive('vrLoader', ['$compile', function ($compile) {

    var directiveDefinitionObject = {
        restrict: 'A',
        scope: false,
        compile: function (tElement, tAttrs) {
            
            var loader = '<div class="loading-circles" ng-show="' + tAttrs.vrLoader + '" style="position:absolute;top:40%;left:45%;" >' +
                  '<div class="loading-circle"></div>' +
                  '<div class="loading-circle"></div>' +
                  '<div class="loading-circle"></div>' +
                  '<div class="loading-circle"></div>' +
                  '<div class="loading-circle"></div>' +
                  '<div class="loading-circle"></div>' +
                  '<div class="loading-circle"></div>' +
                  '<div class="loading-circle"></div>' +
              '</div>';
        var newElement = '<div style="position:relative">'
                            + loader// '<img src="/images/loader-mask.gif" ng-show="' + tAttrs.vrLoader + '" style="position:absolute;top:45%;left:45%;" />'


                            + '<div ng-class="{\'divLoading\': ' + tAttrs.vrLoader + '}">'
                                + tElement.html() 
                            +'</div>'
                        + '</div>';
        tElement.html(newElement);            
    }

    };
    return directiveDefinitionObject;

}]);

