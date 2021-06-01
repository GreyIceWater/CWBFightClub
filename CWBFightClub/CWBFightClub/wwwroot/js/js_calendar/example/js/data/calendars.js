'use strict';

/* eslint-disable require-jsdoc, no-unused-vars */

var CalendarList = [];

function CalendarInfo() {
    this.id = null;
    this.name = null;
    this.checked = true;
    this.color = null;
    this.bgColor = null;
    this.borderColor = null;
    this.dragBgColor = null;
}

function addCalendar(calendar) {
    CalendarList.push(calendar);
}

function findCalendar(id) {
    var found;

    CalendarList.forEach(function(calendar) {
        if (calendar.id === id) {
            found = calendar;
        }
    });

    return found || CalendarList[0];
}

function hexToRGBA(hex) {
    var radix = 16;
    var r = parseInt(hex.slice(1, 3), radix),
        g = parseInt(hex.slice(3, 5), radix),
        b = parseInt(hex.slice(5, 7), radix),
        a = parseInt(hex.slice(7, 9), radix) / 255 || 1;
    var rgba = 'rgba(' + r + ', ' + g + ', ' + b + ', ' + a + ')';

    return rgba;
}

// This is where we need to set discipline ID + related values.
(function() {
    var calendar;
    var id = 0;

    var discs = [];
    $('#discIDs li').each(function () {
        discs.push($(this).html());
    });

    var names = [];
    $('#discNames li').each(function () {
        names.push($(this).html());
    });

    var colors = [];
    $('#discColors li').each(function () {
        colors.push($(this).html());
    });

    for (var count = 0; count < discs.length; count++) {
        var randColor = getRandomColor();
        calendar = new CalendarInfo();
        calendar.id = discs[count];
        calendar.name = names[count];
        calendar.color = 'white';
        calendar.bgColor = colors[count];
        calendar.dragBgColor = colors[count];
        calendar.borderColor = colors[count];
        addCalendar(calendar);
    }
})();

// Done: Update database to store color info for disciplines.
// Leaving this here in case someone needs a random color.
function getRandomColor() {
    var letters = '0123456789ABCDEF';
    var color = '#';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}