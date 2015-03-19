'use strict';

app.service('BaseDirService', function () {

    return ({
        directiveMainURL: "../../Client/Templates/Directives/",
        getObjectProperty: getObjectProperty,
        muteAction: muteAction,
        findExsite: findExsite
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

    function muteAction(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    function findExsite(arr, value, attname) {
        var index = -1;
        for (var i = 0; i < arr.length; i++) {
            if (arr[i][attname] == value) {
                index = i
            }
        }
        return index;
    }

});