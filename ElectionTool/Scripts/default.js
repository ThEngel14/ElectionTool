$(document).ready(function() {
    $("#txtToken").keydown(function (event) {
        if (event.ctrlKey || event.altKey) {
            return true;
        }

        var isNumber = 96 <= event.keyCode && event.keyCode <= 105;
        var isChar = 65 <= event.keyCode && event.keyCode <= 90;

        if (isNumber || isChar) {
            var txt = $('#txtToken');
            var oldLength = txt.val().length;

            // cancel if length > 19 [4-4-4-4]
            if (oldLength >= 19) {
                return false;
            }

            // add key
            txt.val(txt.val() + event.key);

            var value = txt.val();
            var len = value.length;

            // add - if cursor is at the right position
            switch (len) {
            case 4:
            case 9:
            case 14:
                txt.val(value + "-");
            }

            // cancel insert because key is added before
            return false;
        }

        return true;
    });
});