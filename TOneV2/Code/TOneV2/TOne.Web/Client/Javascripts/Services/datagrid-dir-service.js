'use strict';

app.service('DataGridDirService', ['BaseDirService', function (BaseDirService) {

    return ({
        dTemplate: BaseDirService.directiveMainURL + "vr-datagrid.html"
    });

}]);