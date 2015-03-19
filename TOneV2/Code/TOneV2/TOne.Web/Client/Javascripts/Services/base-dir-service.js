'use strict';

app.service('BaseDirService', function () {

    return ({
        directiveMainURL: "../../Client/Templates/Directives/",
        getObjectProperty: getObjectProperty
    });

    function getObjectProperty(item, property) {
        if (property && ('function' === typeof property)) {
            return property(item);
        }
        var arr = property.split('.');
        while (arr.length) {
            item = item[arr.shift()];
        }
        return item;
    }

});