// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Adds the read only attribute to belts table on discipline delete confirm.

function BeltsTableReadOnly() {
    //$('#table-link').prop('readonly', true);
    alert('Its lit')
    var attribute = {};
    $.each("a").attributes, function (id, atr) {
        attribute[atr.nodeName] = atr.nodeValue;
    };
    $("a").replaceWith(function () {
        return $("<p />",
            attribute).append($(this).contents());
    });
}

function formValIndicator() {
    document.getElementById("validForm").innerHTML += "<p style='font-size: .7rem;' class='text-center'>  * Required Field </p>";
    $('[data-val-required]:not([type="checkbox"])').prev('label').append('<span style="color: red;"> *<span>');
}
