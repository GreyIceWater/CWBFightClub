'use strict';

/*import { data } from "jquery"; */

/*eslint-disable*/

var ScheduleList = [];
var databaseScheduleList = [];
var genereatedRecurrence = false;
var recurrenceChecked = false;
var recurrenceCheckbox = "";
var weeksKeyPressField = 1;
var scheduleOpen = false;

var SCHEDULE_CATEGORY = [
    'milestone',
    'task'
];

// Generate a GUID
function chance() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function ScheduleInfo() {

    this.id = null;
    this.calendarId = null; // DiscID

    this.title = null;
    this.body = null;
    this.isAllday = false;
    this.start = null;
    this.end = null;
    this.category = '';
    this.dueDateClass = '';

    this.color = null;
    this.bgColor = null;
    this.dragBgColor = null;
    this.borderColor = null;
    this.customStyle = '';

    this.isFocused = false;
    this.isPending = false;
    this.isVisible = true;
    this.isReadOnly = false;
    this.goingDuration = 0;
    this.comingDuration = 0;
    this.recurrenceRule = '';
    this.hasRecurrence = false;
    this.state = '';
    this.recurrenceFrequency = '';
    this.recurrenceTime = 0;

    this.raw = {
        memo: '',
        hasToOrCc: false,
        hasRecurrenceRule: false,
        location: null,
        class: 'public', // or 'private'
        creator: {
            name: '',
            avatar: '',
            company: '',
            email: '',
            phone: ''
        }
    };
}

function lightOrDark(color) {
    // Variables for red, green, blue values
    var r, g, b, hsp;

    // Check the format of the color, HEX or RGB?
    if (color != null) {
        if (color.match(/^rgb/)) {
            // If RGB --> store the red, green, blue values in separate variables
            color = color.match(/^rgba?\((\d+),\s*(\d+),\s*(\d+)(?:,\s*(\d+(?:\.\d+)?))?\)$/);

            r = color[1];
            g = color[2];
            b = color[3];
        }
        else {

            // If hex --> Convert it to RGB: http://gist.github.com/983661
            color = +("0x" + color.slice(1).replace(
                color.length < 5 && /./g, '$&$&'));
            r = color >> 16;
            g = color >> 8 & 255;
            b = color & 255;
        }

        // HSP (Highly Sensitive Poo) equation from http://alienryderflex.com/hsp.html
        hsp = Math.sqrt(
            0.299 * (r * r) +
            0.587 * (g * g) +
            0.114 * (b * b)
        );
        // Using the HSP value, determine whether the color is light or dark
        if (hsp > 127.5) {
            return 'light';
        }
        else {
            return 'dark';
        }
    }
}

function createSchedule(calendar) {

    var schedule = new ScheduleInfo();

    schedule.id = chance();
    schedule.calendarId = calendar.id // Disc ID

    schedule.title = 'Karate',
    schedule.category = 'time',
    schedule.dueDateClass = '',
    schedule.start = '2021-03-1T22:30:00+09:00',
    schedule.end = '2021-03-19T23:30:00+09:00'
}

function generateRandomSchedule() {
    if (genereatedRecurrence == false) {
        var classDataRows = $('#scheduledClasses').find('tr');
        var SCtable = $('#scheduledClasses')[0];

        // 0:ScheduledClassID, 1:DisciplineID, 2:Name, 3:Start, 4:End
        // 5:HasRecurrence, 6:RecurrenceFrequency, 7:RecurrenceTime, 8:CalendarColor
        for (var j = 0; j < classDataRows.length; j++) {
            var schedule = new ScheduleInfo();

            schedule.category = 'time';
            schedule.dueDateClass = '';
        
            var cell0 = SCtable.rows[j].cells[0];
            var $cell0 = $(cell0);
            schedule.id = $cell0.html();

            var cell1 = SCtable.rows[j].cells[1];
            var $cell1 = $(cell1);
            schedule.calendarId = $cell1.html();

            var cell2 = SCtable.rows[j].cells[2];
            var $cell2 = $(cell2);
            schedule.title = $cell2.html();

            var cell3 = SCtable.rows[j].cells[3];
            var $cell3 = $(cell3);
            schedule.start = $cell3.html();

            var cell4 = SCtable.rows[j].cells[4];
            var $cell4 = $(cell4);
            schedule.end = $cell4.html();

            var cell5 = SCtable.rows[j].cells[5];
            var $cell5 = $(cell5);
            schedule.hasRecurrence = $cell5.html() == 'True' ? true : false;

            var cell6 = SCtable.rows[j].cells[6];
            var $cell6 = $(cell6);
            schedule.recurrenceFrequency = $cell6.html();

            var cell7 = SCtable.rows[j].cells[7];
            var $cell7 = $(cell7);
            schedule.recurrenceTime = $cell7.html();

            var cell8 = SCtable.rows[j].cells[8];
            var $cell8 = $(cell8);
            var SCcolor = $cell8.html();

            schedule.color = 'white';
            schedule.bgColor = SCcolor;
            schedule.dragBgColor = SCcolor;
            schedule.borderColor = SCcolor;

            ScheduleList.push(schedule);
        }
    
        databaseScheduleList = ScheduleList.slice(0);
        checkRecurrence();
    }
}

// Checks schedule list and recurrs an event if recurrence is true
function checkRecurrence() {

    // clear temp schedule list array 
    ScheduleList = [];

    // copy temp schedule list to schedule list
    ScheduleList = databaseScheduleList.slice(0);

    var length = databaseScheduleList.length;

    for (var i = 0; i < length; i++) {

        // text color contrast check
        var contrast = lightOrDark(databaseScheduleList[i].borderColor);

        if (contrast == "dark") {
            databaseScheduleList[i].color = 'white';
        }
        else if (contrast == "light") {
            databaseScheduleList[i].color = 'black';
        }

        recurrenceCheck(databaseScheduleList[i]);
    }
}

function getWeeks() {
    return weeksKeyPressField;
}

function onKeyPressWeeks() {
    weeksKeyPressField = document.getElementById("keypressWeeks").value;
}

function removeSchedule(id) {

    var length = databaseScheduleList.length;

    for (var i = databaseScheduleList.length - 1; i >= 0; i--) {

        if (databaseScheduleList[i].id == id) {
            databaseScheduleList.splice(i, 1);
        }
    }

}

function saveNewRecurrentCheck(schedule) {

    if (recurrenceChecked == true) {
        if ($('#keypressWeeks').length > 0) {
            schedule.recurrenceTime = document.getElementById("keypressWeeks").value - 1;
        }
        else {
            schedule.recurrenceTime = 0;
        }

        //schedule.recurrenceTime = keypress - 1;
        schedule.hasRecurrence = true;
        schedule.recurrenceFrequency = "weekly";

    }

    databaseScheduleList.push(schedule);
}

function updateCalChanges(schedule, changes) {

    if (changes.title != null) {
        schedule.title = changes.title;
    }

    if (changes.calendarId != null) {
        schedule.calendarId = changes.calendarId;
        schedule.color = changes.color;
        schedule.bgcolor = changes.bgColor;
        schedule.borderColor = changes.borderColor;
        schedule.dragBgColor = changes.dragBgColor;
    }

    if (changes.start != null) {
        schedule.start = changes.start;
    }

    if (changes.end != null) {
        schedule.end = changes.end;
    }

    if (changes.isAllday != null) {
        schedule.isAllday = changes.isAllday;
    }

    return schedule;

}

function updateSchedule(schedule) {

    var length = databaseScheduleList.length;

    if ($('#keypressWeeks').length > 0) {
       // var keypress = document.getElementById("keypressWeeks").value;
    }

    for (var i = databaseScheduleList.length - 1; i >= 0; i--) {

        if (databaseScheduleList[i].id == schedule.id) {

            if (scheduleOpen == true) {

                if (recurrenceChecked == true) {
                    if ($('#keypressWeeks').length > 0) {
                        schedule.recurrenceTime = document.getElementById("keypressWeeks").value - 1;
                    }
                    else {
                        schedule.recurrenceTime = databaseScheduleList[i].recurrenceTime;
                    }

                    //schedule.recurrenceTime = keypress - 1;
                    schedule.hasRecurrence = true;
                    schedule.recurrenceFrequency = "weekly";
                }
            }
            else {
                schedule.hasRecurrence = databaseScheduleList[i].hasRecurrence;
                schedule.recurrenceFrequency = databaseScheduleList[i].recurrenceFrequency;
                schedule.recurrenceTime = databaseScheduleList[i].recurrenceTime;
            }

            databaseScheduleList.splice(i, 1);

            var updatedSchedule = new ScheduleInfo();

            updatedSchedule.id = schedule.id;
            updatedSchedule.calendarId = schedule.calendarId;

            updatedSchedule.title = schedule.title;
            updatedSchedule.category = 'time';
            updatedSchedule.dueDateClass = '';

            var sDate = new Date(schedule.start);
            var sDateString = sDate.toISOString();
            updatedSchedule.start = sDateString;

            var eDate = new Date(schedule.end);
            var eDateString = eDate.toISOString();
            updatedSchedule.end = eDateString;

            updatedSchedule.hasRecurrence = schedule.hasRecurrence;
            updatedSchedule.recurrenceFrequency = schedule.recurrenceFrequency;
            updatedSchedule.recurrenceTime = schedule.recurrenceTime;

            databaseScheduleList.push(updatedSchedule);
        }
    }
}

/* Checks if the recurrence checkbox is checked and sets recurrence variable */
function checkboxRecurrenceChecker() {
    if (document.getElementById('chkRecurrence').checked) {
        recurrenceChecked = true;
    }
    else {
        recurrenceChecked = false;
    }
}

/* Turns on the recurrence checkbox if recurrence is true when opening an event */
function recurrenceCheckboxOn(id) {

    var length = databaseScheduleList.length;

    for (var i = databaseScheduleList.length - 1; i >= 0; i--) {

        if (databaseScheduleList[i].id == id) {

            if (databaseScheduleList[i].hasRecurrence == true) {
                recurrenceCheckbox = "checked";
            }
            else {
                recurrenceCheckbox = "";
            }

        }
    }

}

function fillWeeksInput(id) {

    var length = databaseScheduleList.length;

    for (var i = databaseScheduleList.length - 1; i >= 0; i--) {

        if (databaseScheduleList[i].id == id) {

            if (databaseScheduleList[i].recurrenceFrequency == null) {
                databaseScheduleList[i].recurrenceFrequency = 0;
            }

            if (databaseScheduleList[i].recurrenceFrequency != null) {

                if ($('#keypressWeeks').length > 0) {
                    document.getElementById("keypressWeeks").value = databaseScheduleList[i].recurrenceFrequency;
                }
            }
        }
    }
}

function generateSchedule(viewName, renderStart, renderEnd) {
    generateRandomSchedule(calendar, renderStart, renderEnd);
}

function saveSchedules() {
    const classData = databaseScheduleList.map(({
        id, calendarId, title, start, end, hasRecurrence, recurrenceFrequency, recurrenceTime,
        color, bgColor, dragBgColor, borderColor
    }) => ({
        id, calendarId, title, start, end, hasRecurrence, recurrenceFrequency, recurrenceTime,
        color, bgColor, dragBgColor, borderColor
    }));

    $.ajax({
        url: "/ScheduledClass/Save",
        type: "POST",
        cache: false,
        data: { 'classData': classData },
        success: function (data) {
            if (data.success) {
                console.log("Save success!");
            }
        },
        error: function (xhr) {
            alert(xhr);
        },
        complete: function () {
            location.reload();
        }
    });
}

function eventCheck(event) {
    if (event == "tui-full-calendar-time-date-schedule-block-wrap") {
        scheduleOpen = false;
    }
    else if (event == 'tui-full-calendar-icon tui-full-calendar-ic-delete')
    {
        scheduleOpen == false;
    }
}

// Checks for recurrence, then generates schedules based on frequency (weekly, bi-weekly, monthly etc) and for how long (8 weeks etc..)
function recurrenceCheck(schedule) {

    if (schedule.hasRecurrence == true) {

            if (schedule.recurrenceFrequency == "weekly") {

                var newstartdate = new Date(schedule.start);
                var newenddate = new Date(schedule.end);

                // new copies of calendar elements so nothing is bound to new calendar
                var newtitle = schedule.title;
                var newcategory = schedule.category;
                var newbody = schedule.body;
                var newstate = schedule.state;
                var days = 7;

                var i;

                for (i = 0; i < schedule.recurrenceTime; i++) {

                    var newschedule = new ScheduleInfo();

                    newschedule.id = schedule.id;
                    newschedule.calendarId = schedule.calendarId;

                    // add a week to start date
                    var newstartdate = new Date(schedule.start);
                    newstartdate.setDate(newstartdate.getDate() + days);
                    newschedule.start = newstartdate;

                    // add a week to end date
                    var newenddate = new Date(schedule.end);
                    newenddate.setDate(newenddate.getDate() + days);
                    newschedule.end = newenddate;

                    // push recurrent dates to calendar
                    newschedule.title = newtitle;
                    newschedule.category = newcategory;
                    newschedule.body = newbody;
                    newschedule.state = newstate;

                    newschedule.color = schedule.color
                    newschedule.bgColor = schedule.bgColor;
                    newschedule.dragBgColor = schedule.dragBgColor;
                    newschedule.borderColor = schedule.borderColor;

                    ScheduleList.push(newschedule);

                    days = days + 7;
                }

            }
    }

    genereatedRecurrence = true;
}

function getMonth(numericMonth) {

    if (numericMonth == 1) {
        return "January";
    }
    else if (numericMonth == 2) {
        return "February";
    }
    else if (numericMonth == 3) {
        return "March";
    }
    else if (numericMonth == 4) {
        return "April";
    }
    else if (numericMonth == 5) {
        return "May";
    }
    else if (numericMonth == 6) {
        return "June";
    }
    else if (numericMonth == 7) {
        return "July";
    }
    else if (numericMonth == 8) {
        return "August";
    }
    else if (numericMonth == 9) {
        return "September";
    }
    else if (numericMonth == 10) {
        return "October";
    }
    else if (numericMonth == 11) {
        return "November";
    }
    else if (numericMonth == 12) {
        return "December";
    }
}

function monthSplit(montharray) {

    var formatted = "";

    if (montharray.length == 3) {
        var stringMonth1 = getMonth(montharray[1]);
        formatted = "<b>" + stringMonth1 + "</b>" + " " + montharray[2] + ", " + "<i>" + montharray[0] + "</i>";
        return formatted;
    }
    else if (montharray.length == 2) {

        var stringMonth2 = getMonth(montharray[0]);
        formatted = "<b>" + stringMonth2 + "</b>" + " " + montharray[1];
        return formatted;
    }
}

function changeDateFormat(date1, date2, select) {

    const monthOne = date1.split(".");
    const monthTwo = date2.split(".");

    if (select == 3) {
        var month = getMonth(monthOne[1]);
        return "<b>" + month + "</b>" + " " + "<i>" +  monthOne[0] + "</i>";
    }
    if (select == 4) {
        var month = getMonth(monthTwo[1]);
        return "<b>" + month + "</b>" + " " + "<i>" + monthTwo[0] + "</i>";
    }

    var dateOne = monthSplit(monthOne);
    var dateTwo = monthSplit(monthTwo);

    if (select == 1) {
        return dateOne;
    }
    else if (select == 2) {
        return dateTwo;
    }
}