'use strict';

angular
  .module('datetimepicker', [])

  .provider('datetimepicker', function () {
    var default_options = {};

    this.setOptions = function (options) {
      default_options = options;
    };

    this.$get = function () {
      return {
        getOptions: function () {
          return default_options;
        }
      };
    };
  })

  .directive('datetimepicker', [
    '$timeout',
    'datetimepicker',
    function ($timeout,
              datetimepicker) {

      var default_options = datetimepicker.getOptions();

      return {
        require : '?ngModel',
        restrict: 'AE',
        scope   : {
          datetimepickerOptions: '@'
        },
        link    : function ($scope, $element, $attrs, ngModelCtrl) {
          var passed_in_options = $scope.$eval($attrs.datetimepickerOptions);
          var options = jQuery.extend({}, default_options, passed_in_options);
          options.allowInputToggle = true;
         // options.use24hours = true;
            setTimeout(function () {
                $('.datetime-controle').datetimepicker();
            }, 1000)
          $element
            .on('dp.change', function (e) {
              if (ngModelCtrl) {
                $timeout(function () {
                  ngModelCtrl.$setViewValue(e.target.value);
                });
              }
            })
            .datetimepicker(options);

          function setPickerValue() {
            var date = null;

            if (ngModelCtrl && ngModelCtrl.$viewValue) {
              date = ngModelCtrl.$viewValue;
            }

            $element
              .data('DateTimePicker')
              .date(date);
          }
          

          if (ngModelCtrl) {
            ngModelCtrl.$render = function () {
              setPickerValue();
            };
          }

          setPickerValue();
        }
      };
    }
  ]);
function toggletime(e) {
    $('.datetime-controle').data("DateTimePicker").toggle();
   
    // to open time div and close date div
    $('.date-section').removeClass('in');
    $('.time-section').addClass('in');

    // switch icon to date
    $('.btn-switcher').removeClass("glyphicon-time");
    $('.btn-switcher').addClass("glyphicon-calendar");
}
function toggledate(e) {
    $('.datetime-controle').data("DateTimePicker").toggle()

    // to open date div and close time div
    $('.time-section').removeClass('in');
    $('.date-section').addClass('in');

    // switch icon to time
    $('.btn-switcher').addClass("glyphicon-time");
    $('.btn-switcher').removeClass("glyphicon-calendar");
   
    
}