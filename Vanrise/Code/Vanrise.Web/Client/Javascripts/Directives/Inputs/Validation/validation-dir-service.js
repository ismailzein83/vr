'use strict';

app.service('ValidationService', ['BaseDirService', function (BaseDirService) {

    return ({
        validate: validate
    });

    function arrIsEmpty(value) {
        if (typeof value !== 'undefined' && value.length > 0)
            return false;
        return true;
    }

    function valRequired(values) {

        if (typeof values.arrData == 'undefined' && typeof values.data == 'undefined') return false;

        if (typeof values.arrData !== 'undefined') {
            if (arrIsEmpty(values.arrData))
                return false;
        }
        else if (values.data != undefined) {
            if (values.data == '') return false;
        }

        return true;
    }

    function validate(options, values) {
        if (options.required) return valRequired(values);
    }


}]);