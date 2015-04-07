'use strict';

app.service('ActionBarDirService', ['BaseDirService', function (BaseDirService) {

    return ({
        dTemplate: BaseDirService.directiveMainURL + "vr-actionbar.html"
    });

}]);