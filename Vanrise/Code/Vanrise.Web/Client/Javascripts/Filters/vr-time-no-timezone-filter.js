app.filter('vrtimeNoTimeZone', function () {
    return function (val, offset) {

        if (val != null && val.length > 19) {
            return val.slice(0, -6);
        }

        return val;
    };
});