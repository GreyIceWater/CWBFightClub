$(function () {
    var enrollmentDetailID = $('#EnrollmentDetail').html();

    if (enrollmentDetailID != 0) {
        ShowEnrollmentDetail(enrollmentDetailID);
    }
});

function BeltEdit(achievedBeltID) {
    let dateSelector = '#achievedBeltDate_' + achievedBeltID;
    let buttonSelector = '#achievedBeltEditButton_' + achievedBeltID;

    let dateElement = $(dateSelector);
    let buttonElement = $(buttonSelector);

    if (buttonElement.html() == "Edit") {
        

        dateElement.prop("readonly", false);
        dateElement.css({
            "border-color": "green",
            "border-width": "2px",
            "border-style": "solid"
        });

        buttonElement.html("Cancel");
    } else {
        buttonElement.html("Edit");

        dateElement.prop("readonly", true);
        dateElement.css({
            "border-color": "black",
            "border-width": "1px",
            "border-style": "solid"
        });
    }
}

function ShowEnrollmentDetail(enrollmentID) {
    HideAllDivs();
    let divSelector = '#DetailsEnrollmentDiv';
    $(divSelector).removeClass("d-none");
    $(divSelector).show();

    $.ajax({
        type: "GET",
        url: "/Enrollment/Detail/?id=" + enrollmentID,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (result, status, xhr) {
            $("#DetailsEnrollmentDiv").html(result)
        },
        error: function (xhr, status, error) {
            alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
        }
    });
}

function ShowEnrollmentEdit(enrollmentID) {
    HideAllDivs();
    let divSelector = '#EditEnrollmentDiv';
    $(divSelector).removeClass("d-none");
    $(divSelector).show();

    $.ajax({
        type: "GET",
        url: "/Enrollment/Edit/?id=" + enrollmentID,
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (result, status, xhr) {
            $("#EditEnrollmentDiv").html(result)
        },
        error: function (xhr, status, error) {
            alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
        }
    });
}

function HideAllDivs() {
    $('.hideOnClick').hide();
}

function ValidateEndDate(enrollmentID) {
    let endDateSelector = '#EnrollmentEndID_' + enrollmentID;
    let startDateSelector = '#EnrollmentStartID_' + enrollmentID;
    let errorSelector = '#EnrollmentEndErrorID_' + enrollmentID;

    var endDateValue = $(endDateSelector).val();
    var startDateValue = $(startDateSelector).val();

    if (Date.parse(endDateValue) < Date.parse(startDateValue)) {
        $(errorSelector).removeClass("d-none");
        $(errorSelector).html("End date must be after start date");
        return;
    }

    $(errorSelector).addClass("d-none");
    $(errorSelector).html("");
}

function SetEnrollmentEndDateNow(enrollmentID) {
    let endDateSelector = '#EnrollmentEndID_' + enrollmentID;
    let endDateButton = '#EnrollmentEndButtonID_' + enrollmentID;
    let endDateLabel = '#EnrollmentEndLabel_' + enrollmentID;

    let now = new Date();

    let day = ("0" + now.getDate()).slice(-2);
    let month = ("0" + (now.getMonth() + 1)).slice(-2);

    let today = now.getFullYear() + "-" + (month) + "-" + (day);

    $(endDateSelector).val(today);
    
    $(endDateButton).hide();
    $(endDateSelector).removeClass("d-none");
    $(endDateSelector).show();
    $(endDateLabel).removeClass("d-none");
    $(endDateLabel).show();
}