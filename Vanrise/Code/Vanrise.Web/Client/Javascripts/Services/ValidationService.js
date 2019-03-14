( function ( appControllers )
{

    "use strict";
    VRValidationService.$inject = ['UtilsService', 'ValidationMessagesEnum', 'VRDateTimeService'];

    function VRValidationService( UtilsService, ValidationMessagesEnum, VRDateTimeService )
    {

        function validate( value, $scope, $attrs, validationOptions )
        {
            if ( $attrs.isrequired != undefined && ( $attrs.isrequired == "" || $scope.$parent.$eval( $attrs.isrequired ) ) )
            {
                if ( value == undefined || value.length == 0 )
                    return ValidationMessagesEnum.required;
            }

            if ( $attrs.validateanyvalue != undefined )
            {//After discussion with Ismail {
                var validateAnyValueError = $scope.$parent.$eval( $attrs.validateanyvalue );
                if ( validateAnyValueError != null )
                    return validateAnyValueError;
            }

            if ( value != undefined && value != null )
            {
                if ( validationOptions != undefined )
                {
                    if ( validationOptions.emailValidation )
                    {
                        if ( !validateEmail( value ) )
                            return ValidationMessagesEnum.invalidEmail;
                    }
                    if ( validationOptions.labledEmailValidation )
                    {
                        if ( !validateLabeledEmail( value ) )
                            return ValidationMessagesEnum.invalidEmail;
                    }
                    if ( validationOptions.ipValidation )
                    {
                        if ( !validateIp( value, validationOptions ) )
                            return ValidationMessagesEnum.invalidIp;
                    }

                    if (validationOptions.ipV6Validation) {
                        if (!validateIpV6(value, validationOptions))
                            return ValidationMessagesEnum.invalidIpV6;
                    }
                    if ( validationOptions.filenameValidation )
                    {
                        if ( !validateFileName( value, validationOptions ) )
                            return ValidationMessagesEnum.invalidFileName;
                    }
                    if ( validationOptions.minlengthValidation )
                    {
                        if ( validateMinLength( value, validationOptions ) )
                            return ValidationMessagesEnum.invalidMinLength + " " + validationOptions.minLength + " characters.";
                    }
                    if ( validationOptions.specialCharacterValidation )
                    {
                        if ( !validateSpecialCharacterMask( value ) )
                            return ValidationMessagesEnum.invalidSpecialCharacter;
                    }
                    if ( validationOptions.numberValidation )
                    {
                        var validationResult = validateNumber( value, validationOptions );
                        if ( validationResult != undefined )
                        {
                            if ( validationResult.validmin == false )
                                return ValidationMessagesEnum.invalidMin + " " + validationOptions.minNumber;
                            if ( validationResult.validmax == false )
                                return ValidationMessagesEnum.invalidMax + " " + validationOptions.maxNumber;
                            if ( validationResult.validmaxprec == false )
                                return ValidationMessagesEnum.invalidPrec + " " + validationOptions.numberPrecision + " digit(s)";
                        }
                    }
                }
                if ( $attrs.customvalidate != undefined )
                {
                    var customerValidationError = $scope.$parent.$eval( $attrs.customvalidate );
                    if ( customerValidationError != null )
                        return customerValidationError;
                }
            }

            return null;
        }

        function validateEmail( value )
        {
            var isEmail_re = /^\s*[\w\-\+_]+(\.[\w\-\+_]+)*\@[\w\-\+_]+\.[\w\-\+_]+(\.[\w\-\+_]+)*\s*$/;
            return String( value ).search( isEmail_re ) != -1 || value === "";
        }

        function validateLabeledEmail( value )
        {
            var is_labeledEmail_re = /^(\'([\s]*[\w\.\-\?\!\#\+_][\s]*)+\'|\<([\s]*[\w\.\-\?\!\#\+_][\s]*)+\>)?\s*[\w\-\+_]+(\.[\w\-\+_]+)*\@[\w\-\+_]+\.[\w\-\+_]+(\.[\w\-\+_]+)*\s*$/;
            var is_labledEmailFormat2_re = /^(\"([\s]*[\w\.\-\?\!\#\+_][\s]*)+\")?\s*\<[\w\-\+_]+(\.[\w\-\+_]+)*\@[\w\-\+_]+\.[\w\-\+_]+(\.[\w\-\+_]+)*\>\s*$/;
            return String( value ).search( is_labeledEmail_re ) != -1 || String( value ).search( is_labledEmailFormat2_re ) != -1 || value === "";
        }

        function validateIp( value )
        {
            var isIp_re = /^([0-9]{1,3})[.]([0-9]{1,3})[.]([0-9]{1,3})[.]([0-9]{1,3})$/;
            var ipParts = value.split( "." );
            var valid = true;
            if ( String( value ).search( isIp_re ) == -1 )
                valid = false;
            if ( parseInt( ipParts[0] ) > 255 || parseInt( ipParts[1] ) > 255 || parseInt( ipParts[2] ) > 255 || parseInt( ipParts[3] ) > 255 )
                valid = false;

            return valid;
            // return String(value).search(isIp_re) != -1;
        }

        function validateIpV6(value) {
            var isIpV6_re = /^\s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:)))(%.+)?\s*$/;
            return String(value).search(isIpV6_re) != -1;
        }

        function validateFileName( value )
        {
            var rg1 = /^[^\\/:\*\?"<>\|]+$/;
            var rg2 = /^\./;
            var rg3 = /^(nul|prn|con|lpt[0-9]|com[0-9])(\.|$)/i;
            return rg1.test( value ) && !rg2.test( value ) && !rg3.test( value );
        }

        function validateSpecialCharacterMask( value )
        {
            var maskRg = /^[-\w\-\_\@\.\s]+$/;
            return maskRg.test( value );
        }
        function validateMinLength( value, validationOptions )
        {
            if ( value == undefined || value == null || value == "" ) return false;
            return validationOptions.minLength > value.length;
        }
        function validateNumber( value, validationOptions )
        {

            var decimalArray = String( value ).split( "." );
            var validmax = ( validationOptions.maxNumber != undefined ) ? parseFloat( value ) <= validationOptions.maxNumber : true;
            var validmin = ( validationOptions.minNumber != undefined ) ? parseFloat( value ) >= validationOptions.minNumber : true;
            var validmaxprec = ( validationOptions.numberPrecision != undefined && decimalArray[1] != undefined ) ? decimalArray[1].length <= validationOptions.numberPrecision : true;
            if ( validmin && validmax && validmaxprec ) return;
            else return {
                validmin: validmin,
                validmax: validmax,
                validmaxprec: validmaxprec
            };
        }

        function validateTimeRange( fromDate, toDate )
        {

            var errorMessage = "Invalid time range";

            var startDate = UtilsService.getDateObject( fromDate );
            var endDate = UtilsService.getDateObject( toDate );

            if ( startDate != undefined && endDate != undefined )
            {
                if ( startDate.getTime() > endDate.getTime() )
                    return errorMessage;
            }
            else
            {
                if ( fromDate instanceof Object && toDate instanceof Object )
                {
                    if ( toDate.Hour == 0 && toDate.Minute == 0 )
                        return null;
                    else if ( fromDate.Hour > toDate.Hour || ( fromDate.Hour == toDate.Hour && fromDate.Minute > toDate.Minute ) )
                        return errorMessage;
                }
            }
            return null;
        }

        function validateTimeEqualorGreaterthanToday( currentDate )
        {
            var errorMessage = "Date cannot be in the past";
            var today = VRDateTimeService.getNowDateTime();
            today.setHours( 0, 0, 0, 0 );

            if ( currentDate instanceof Date )
            {
                if ( currentDate == undefined )
                    return null;
                var from = new Date( currentDate );
                if ( from.getTime() < today.getTime() )
                    return errorMessage;
                else
                    return null;
            }
            else if ( currentDate instanceof Object )
            {
                if ( currentDate.Hour < today.Hour || ( currentDate.Hour == today.Hour && currentDate.Minute < today.Minute ) )
                    return errorMessage;
                else
                    return null;
            }
            else if ( typeof currentDate == 'string' )
            {
                if ( currentDate == undefined )
                    return null;
                var from = new Date( currentDate );
                if ( from.getTime() < today.getTime() )
                    return errorMessage;
                else
                    return null;
            }
        }

        return ( {
            validate: validate,
            validateEmail: validateEmail,
            validateIp: validateIp,
            validateIpV6: validateIpV6,
            validateTimeRange: validateTimeRange,
            validateTimeEqualorGreaterthanToday: validateTimeEqualorGreaterthanToday
        } );
    }

    appControllers.service( 'VRValidationService', VRValidationService );
} )( appControllers );