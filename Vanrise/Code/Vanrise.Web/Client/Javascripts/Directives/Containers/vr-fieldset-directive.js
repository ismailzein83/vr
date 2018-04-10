'use strict';

app.directive('vrFieldset', ['VRLocalizationService', function (VRLocalizationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: false,
        compile: function (tElement, tAttrs) {
            var title = tAttrs.header || tAttrs.title;
            var localizedtitle = tAttrs.localizedtitle || tAttrs.localizedheader;
            title = VRLocalizationService.getResourceValue(localizedtitle, title);
            var newElement = '<div class="panel-primary fieldset-vr"><div class="panel-heading"><span class="title">' + title + '</span></div><div class="panel-body">' + tElement.html() + '</div></div>';
            tElement.html(newElement);
        }

    };

    return directiveDefinitionObject;

}]);