'use strict';

app.service('ChartDirService', ['BaseDirService', function (BaseDirService) {

    return ({
        dTemplate: BaseDirService.directiveMainURL + "vr-chart.html"
    });

}]);