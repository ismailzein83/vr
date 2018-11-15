'use strict';
app.filter('vrtextOrNumber', ['$filter', function ($filter) {
    return function (input, fractionSize) {
        if (input == undefined || input == null || input === '' || input === "")
            return null;
        if (isNaN(input)) {
            return input;
        } else {
            return $filter('number')(input, fractionSize);
        }
    };
}]);