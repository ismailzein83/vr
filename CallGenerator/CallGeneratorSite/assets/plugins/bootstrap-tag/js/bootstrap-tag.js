/* ==========================================================
 * bootstrap-tag.js v2.2.2
 * https://github.com/fdeschenes/bootstrap-tag
 * ==========================================================
 * Copyright 2012 Francois Deschenes.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ========================================================== */

// https://github.com/soliantconsulting/tagmanager/blob/master/tagmanager.js

!function ( $ ) {
  
  'use strict' // jshint ;_;
  
  var Tag = function ( element, options ) {
    this.element = $(element)
    this.options = $.extend(true, {}, $.fn.tag.defaults, options)
    this.values = $.grep($.map(this.element.val().split(','), $.trim), function ( value ) { return value.length > 0 })
    this.show()
  }
  
  Tag.prototype = {
    constructor: Tag
    
  , show: function () {
      var that = this
      
      that.element.parent().prepend(that.element.detach().attr('type', 'hidden'))
      that.element
        .wrap($('<div class="tags ' + (that.options.containerClasses ?  that.options.containerClasses : '') + '">'))
        .parent()
        .on('click', function () {
          that.input.focus()
        })
      
      if (that.values.length) {
        $.each(that.values, function () {
          that.createBadge(this)
        })
      }


      that.input = $('<input type="text" class="' + (that.options.inputClasses ?  that.options.inputClasses : '') + '">')
        .attr('placeholder', that.options.placeholder)
        .insertAfter(that.element)
        .on('focus', function () {
          that.element.parent().addClass('tags-hover')
        })
        .on('blur', function () {
          that.process()
          that.element.parent().removeClass('tags-hover')
          that.element.siblings('.tag').removeClass('tag-important')
        })
        .on('keydown', function ( event ) {
          if ( event.keyCode == 188 || event.keyCode == 13 || event.keyCode == 9 ) {
            if ( $.trim($(this).val()) && ( !that.element.siblings('.typeahead').length || that.element.siblings('.typeahead').is(':hidden') ) ) {
              if ( event.keyCode != 9 ) event.preventDefault()
              that.process()
            } else if ( event.keyCode == 188 ) {
              if ( !that.element.siblings('.typeahead').length || that.element.siblings('.typeahead').is(':hidden') ) {
                event.preventDefault()
              } else {
                that.input.data('typeahead').select()
                event.stopPropagation()
                event.preventDefault()
              }
            }
          } else if ( !$.trim($(this).val()) && event.keyCode == 8 ) {
            var count = that.element.siblings('.tag').length
            if (count) {
              var tag = that.element.siblings('.tag:eq(' + (count - 1) + ')')
              if (tag.hasClass('tag-important')) that.remove(count - 1)
              else tag.addClass('tag-important')
            }
          } else {
            that.element.siblings('.tag').removeClass('tag-important')
          }
        })
        .typeahead({
          source: that.options.source
        , matcher: function ( value ) {
            return ~value.toLowerCase().indexOf(this.query.toLowerCase()) && (that.inValues(value) == -1 || that.options.allowDuplicates)
          }
        , updater: $.proxy(that.add, that)
        });


      //auto resize input: this option added by keenthemes for inbos compose form needs. this will enable typing a tag in hirozontal position and input auto resizes while typing
      if (that.options.autosizedInput) {
         var  input = that.input,
              minWidth = that.options.minWidth || input.width(),
              maxWidth = that.options.maxWidth || input.closest(".tags").width(),
              comfortZone = 10,
              val = '',
             
              testSubject = $('<tester/>').css({
                  position: 'absolute',
                  top: -9999,
                  left: -9999,
                  width: 'auto',
                  paddingLeft: input.css('paddingLeft'),
                  paddingRight: input.css('paddingRight'),
                  fontSize: input.css('fontSize'),
                  fontFamily: input.css('fontFamily'),
                  fontWeight: input.css('fontWeight'),
                  letterSpacing: input.css('letterSpacing'),
                  whiteSpace: 'nowrap'
              }),

              check = function() {

                  if (val === (val = input.val())) {return;}

                  // Enter new content into testSubject
                  var escaped = val.replace(/&/g, '&amp;').replace(/\s/g,' ').replace(/</g, '&lt;').replace(/>/g, '&gt;');
                  testSubject.html(escaped);

                  // Calculate new width + whether to change
                  var testerWidth = testSubject.width(),
                      newWidth = (testerWidth + comfortZone) >= minWidth ? testerWidth + comfortZone : minWidth,
                      currentWidth = input.width(),
                      isValidWidthChange = (newWidth < currentWidth && newWidth >= minWidth)
                                           || (newWidth > minWidth && newWidth < maxWidth);


                  // Animate width
                  if (isValidWidthChange) {
                      input.width(newWidth);
                  }

              };

          testSubject.insertAfter(input);

          input.bind('keydown keyup blur update', check);
        }
        //auto resize input: added by keenthemes
    }
  , inValues: function ( value ) {
      if (this.options.caseInsensitive) {
        var index = -1
        $.each(this.values, function (indexInArray, valueOfElement) {
          if ( valueOfElement.toLowerCase() == value.toLowerCase() ) {
            index = indexInArray
            return false
          }
        })
        return index
      } else {
        return $.inArray(value, this.values)
      }
    }
  , createBadge: function ( value ) {
    var that = this
    
    $('<span class="tag">')
      .text(value)
      .append($('<button type="button" class="close"></button>') //modified by keenthemes sinse close class already defined in global style(style.css)
        .on('click', function () {
          that.remove(that.element.siblings('.tag').index($(this).closest('.tag')))
        })
      )
      .insertBefore(that.element)
  }
  , add: function ( value ) {
      var that = this
      
      if ( !that.options.allowDuplicates ) {
        var index = that.inValues(value)
        if ( index != -1 ) {
          var badge = that.element.siblings('.tag:eq(' + index + ')')
          badge.addClass('tag-warning')
          setTimeout(function () {
            $(badge).removeClass('tag-warning')
          }, 500)
          return
        }
      }
      
      this.values.push(value)
      this.createBadge(value)
      
      this.element.val(this.values.join(', '))
    }
  , remove: function ( index ) {
      if ( index >= 0 ) {
        this.values.splice(index, 1)
        this.element.siblings('.tag:eq(' + index + ')').remove()
        this.element.val(this.values.join(', '))
      }
    }
  , process: function () {
      var values = $.grep($.map(this.input.val().split(','), $.trim), function ( value ) { return value.length > 0 }),
          that = this
      $.each(values, function() {
        that.add(this)
      })
      this.input.val('')
    }
  }
  
  var old = $.fn.tag
  
  $.fn.tag = function ( option ) {
    return this.each(function () {
      var that = $(this)
        , data = that.data('tag')
        , options = typeof option == 'object' && option
      if (!data) that.data('tag', (data = new Tag(this, options)))
      if (typeof option == 'string') data[option]()
    })
  }
  
  $.fn.tag.defaults = {
    allowDuplicates: false
  , caseInsensitive: true
  , placeholder: ''
  , source: []
  }
  
  $.fn.tag.Constructor = Tag
  
  $.fn.tag.noConflict = function () {
    $.fn.tag = old
    return this
  }
  
  $(window).on('load', function () {
    $('[data-provide="tag"]').each(function () {
      var that = $(this)
      if (that.data('tag')) return
      that.tag(that.data())
    })
  })
}(window.jQuery)