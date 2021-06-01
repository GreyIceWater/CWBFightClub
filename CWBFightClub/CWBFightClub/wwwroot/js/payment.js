$(function () {
    $("#staticBackdrop").on("submit", "#addPaymentForm", function (e) {
        e.preventDefault();

        var form = $(this);
        $.ajax({
            url: form.attr("action"),
            method: form.attr("method"),  // post
            data: form.serialize(),
            success: function (partialResult) {
                $("#staticBackdrop").html(partialResult);
            }
        });
    });
});

function EditBalance() {
    $(".balanceInput").prop("readonly", false);
    $(".balanceSelect").prop("disabled", false);
    $("#saveBalanceButton").show();
    $("#editBalanceButton").hide();
}

function EditPayment() {
    $(".paymentInput").prop("readonly", false);
    $("#paymentEditSaveButton").show();
    $("#paymentEditEditButton").hide();
}

function ShowPaymentEdit(paymentID) {
    HideAllDivs();
    let divSelector = '#EditPaymenttDiv';
    $(divSelector).removeClass("d-none");
    $(divSelector).show();

    $.ajax({
        type: "GET",
        url: "/Payment/Edit/?id=" + paymentID,
        contentType: "application/json; charset=utf-8",
        dataType: "html"
    }).done(function (result) {
        $(divSelector).html(result);
    }).fail(function () {
        alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText);
    }).always(function () {
        $("form").each(function () { $.data($(this)[0], 'validator', false); });
        $.validator.unobtrusive.parse("form");
    });
}

function HideAllDivs() {
    $('.hideOnClick').hide();
}
