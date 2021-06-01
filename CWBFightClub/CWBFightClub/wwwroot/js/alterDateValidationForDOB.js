$.validator.methods.range = function (value, element, param) {
    if ($('#dob')) {
        var min = $(element).attr('data-val-range-min');
        var max = $(element).attr('data-val-range-max');
        var date = new Date(value).getTime();
        var minDate = new Date(min).getTime() || 0;
        var maxDate = new Date(max).getTime() || 8640000000000000;
        return this.optional(element) || (date >= minDate && date <= maxDate);
    }

    return this.optional(element) || (value >= param[0] && value <= param[1]);
};
