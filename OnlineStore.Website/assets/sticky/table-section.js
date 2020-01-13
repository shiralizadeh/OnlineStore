/*!
 * table-section.js v0.1.0
 */

/*global jQuery, window */
(function ($) {
  'use strict';
  $.fn.tableSection = function (options) {
    var settings = $.extend({}, $.fn.tableSection.defaults, options);

    function updateTableSections() {
      $(".ts-table-section").each(function (index, table) {
        var $table = $(table),
            toffset = $(table).offset(),
            scrollTop = $(window).scrollTop() + settings.verticalOffset,
            rows = $table.find(".ts-row-section"),
            frows = $table.find(".ts-row-fixed");
        rows.each(function (index, row) {
          var $row = $(row),
              roffset = $row.offset(),
              hpoint = (index + 1 === rows.length) ? (toffset.top + $table.height())
                                                   : ($(rows[index + 1]).offset().top),
              voffset;
          if ((scrollTop > roffset.top) && (scrollTop < hpoint)) {
            voffset = Math.max(0, (scrollTop - (hpoint - $row.height())));
            $(frows[index]).css("visibility", "visible")
                           .css("top", settings.verticalOffset - voffset);
          } else {
            $(frows[index]).css("visibility", "hidden");
          }
        });
      });
    }

    function updateRowWidths() {
      $(".ts-table-section").each(function (index, table) {
        var $table = $(table),
            rows = $table.find(".ts-row-section"),
            frows = $table.find(".ts-row-fixed");
        rows.each(function (index, row) {
          var cells = $(row).find("th,td");
          $(frows[index]).find("th,td").each(function (index, ccell) {
            $(ccell).width($(cells[index]).width())
                    .height($(cells[index]).height());
          });
        });
      });
    }

    $(".ts-table-section").each(function (index, table) {
      $(table).find(".ts-row-section").each(function (index, row) {
        var $row = $(row),
            crow = $row.clone()
                       .removeClass("ts-row-section")
                       .addClass("ts-row-fixed")
                       .css("top", settings.verticalOffset)
                       .appendTo($row.parent()),
            cells = $row.find("th,td");
        $(crow).find("th,td").each(function (index, ccell) {
          $(ccell).width($(cells[index]).width())
                  .height($(cells[index]).height());
        });
      });
      // add unused row to work-around bootstrap rounded-corner of last fixed section
      $("<tr></tr>").css({
        'position': 'fixed',
        'visibility': 'hidden'
      }).appendTo(table);
    });

    $(window).on('scroll', updateTableSections)
             .on('resize', updateRowWidths)
             .trigger('scroll');
  };

  $.fn.tableSection.defaults = {
    verticalOffset: 0
  };
}(jQuery));
